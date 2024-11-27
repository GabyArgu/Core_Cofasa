using CoreContable.Models;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace CoreContable.Controllers;

[Authorize]
public class HomeController(
    ISecurityRepository securityRepository,
    IUserRepository userRepository,
    ICiasRepository ciasRepository,
    IContaRepoRepository contaRepoRepository,
    IDmgPolizaRepository dmgPolizaRepository,
    ILogger<HomeController> logger
) : Controller
{
    [IsAuthorized(alias: CC.FIST_LEVEL_PERMISSION_DASHBOARD)]
    public IActionResult Index()
    {
        return View();
    }

    // [AllowAnonymous]
    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ChangeCia(string newCiaCode)
    {
        try
        {
            var currentCia = securityRepository.GetSessionCiaCode();

            if (newCiaCode == currentCia)
            {
                return View("Index");
            }

            securityRepository.ChangeSessionCia(newCiaCode);
            var userName = securityRepository.GetSessionUserName();
            var userAccessList = await securityRepository.GetLoginUser(userName, newCiaCode);

            if (userAccessList.IsNullOrEmpty())
            {
                securityRepository.DestroySession();
                return RedirectToAction(
                    CC.LogOutActionName,
                    CC.SecurityControllerName
                );
            }

            var result = await securityRepository.SaveSession(userAccessList!);
            if (result) return RedirectToAction(CC.DashboardActionName);
            throw new SecurityTokenException();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(HomeController), nameof(ChangeCia));
            await securityRepository.DestroySession();
            return RedirectToAction(
                CC.LoginActionName,
                CC.SecurityControllerName
            );
        }
    }

    [IsAuthorized(alias: CC.FIST_LEVEL_PERMISSION_DASHBOARD)]
    [HttpGet]
    public async Task<JsonResult> DashboardStats()
    {
        bool success;
        var ciasCount = 0;
        var usersCount = 0;
        
        var repositoryNumber = 0;
        var repositoryCanUpperNumber = 0;
        var repositoryNotEqualsNumber = 0;
        
        var dmgPolizaNumber = 0;
        var dmgPolizaPrinted = 0;
        var dmgPolizaNotPrinted = 0;

        try
        {
            var currentCia = securityRepository.GetSessionCiaCode();
            ciasCount = await ciasRepository.GetCount();
            usersCount = await userRepository.GetCount();

            repositoryNumber = await contaRepoRepository.GetAllCountByCia(currentCia);
            repositoryCanUpperNumber = await contaRepoRepository.GetCanBeUpperCountByCia(currentCia);
            repositoryNotEqualsNumber = await contaRepoRepository.GetCantBeUpperCountByCia(currentCia);
            
            dmgPolizaNumber = await dmgPolizaRepository.GetAllCountByCia(currentCia);
            dmgPolizaPrinted = await dmgPolizaRepository.GetPrintedCountByCia(currentCia, true);
            dmgPolizaNotPrinted = await dmgPolizaRepository.GetPrintedCountByCia(currentCia, false);

            success = true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(HomeController), nameof(DashboardStats));
            success = false;
        }

        var message = success ? "Access data"
            : "No access data";

        return Json(new
        {
            success,
            message,
            data = new
            {
                ciasCount,
                usersCount,

                repositoryNumber,
                repositoryCanUpperNumber,
                repositoryNotEqualsNumber,

                dmgPolizaNumber,
                dmgPolizaPrinted,
                dmgPolizaNotPrinted
            }
        });
    }
}