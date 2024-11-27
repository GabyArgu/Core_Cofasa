using CoreContable.Models;
using CoreContable.Services;
using CoreContable.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Controllers;

public class SecurityController(
    ISecurityRepository securityRepository,
    ICiasRepository ciasRepository,
    ICoreHash coreHash,
    ILogger<SecurityController> logger
) : Controller {
    [AllowAnonymous]
    public async Task<IActionResult> Login(string? ReturnUrl = null) {
        await securityRepository.DestroySession();
        var model = new LoginViewModel {
            Cias = await ciasRepository.GetCias()
        };

        ViewData["ReturnUrl"] = ReturnUrl;
        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> Login(LoginViewModel model, string? returnUrl) {
        model.Cias = await ciasRepository.GetCias();

        if (!ModelState.IsValid) {
            return View(model);
        }

        var user = await securityRepository.Authenticate(model.UserName!, model.Password!);
        var userAccessList = await securityRepository.GetLoginUser(model.UserName!, model.CiaCode!);

        // El usuario no existe, no tiene ningun rol o su rol aun no posee permisos asignados.
        if (user == null || userAccessList.IsNullOrEmpty()) {
            ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña incorrectos");
            return View(model);
        }

        // var result = await securityRepository.SaveSession(userAccessList);
        var result = userAccessList != null && await securityRepository.SaveSession(userAccessList);
        if (!result) {
            ModelState.AddModelError(string.Empty, "Ocurrió un error al crear la sesión");
            return View(model);
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) {
            return Redirect(returnUrl);
        }

        return RedirectToAction(
            CC.DashboardActionName,
            CC.HomeControllerName
        );
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LogOut() {
        await securityRepository.DestroySession();
        return RedirectToAction(
            CC.LoginActionName,
            CC.SecurityControllerName
        );
    }

#if DEBUG
    [AllowAnonymous]
    [HttpGet]
    public JsonResult DoEncrypt([FromQuery] string pwd) {
        bool result;
        string data;

        try {
            data = coreHash.Hash(pwd);
            result = true;
        }
        catch (Exception) {
            result = false;
            data = "";
        }

        return Json(new {
            success = result,
            message = result ? "Access data" : "Ocurrió un error al procesar la solicitud",
            data
        });
    }
#endif
}