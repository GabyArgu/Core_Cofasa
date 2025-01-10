using System.Text.Json;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Controllers;

public class DmgCuentasController(
    IDmgCuentasRepository dmgCuentasRepository,
    ISecurityRepository securityRepository,
    ILogger<DmgCuentasController> logger
) : Controller
{
    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_DMGCUENTAS)]
    public IActionResult Index()
    {
        return View();
    }

    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_DMGCUENTAS)]
    [HttpGet]
    public async Task<JsonResult> GetAll([FromQuery] string ciaCod)
    {
        List<DmgCuentasResultSet>? data;

        try
        {
            data = await dmgCuentasRepository.GetAllByCia(ciaCod);
        }
        catch (Exception e)
        {
            data = null;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgCuentasController), nameof(GetAll));
        }

        return Json(new
        {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }


    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_DMGCUENTAS_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_DMGCUENTAS_CAN_UPDATE}")]
    [HttpPost]
    public async Task<JsonResult> SaveOrUpdate([FromForm] DmgCuentasDto data)
    {
        bool result;
        var isUpdating = false;
        
        try
        {
            if (data.isUpdating.IsNullOrEmpty())
            {
                data.FechaCreacion = DateTime.Now;
                data.UsuarioCreacion = securityRepository.GetSessionUserName();
                data.FechaModificacion = DateTime.Now;
                data.UsuarioModificacion = securityRepository.GetSessionUserName ( );
                isUpdating = false;
            }
            else
            {
                data.FechaModificacion = DateTime.Now;
                data.UsuarioModificacion = securityRepository.GetSessionUserName();
                isUpdating = true;
            }

            result = await dmgCuentasRepository.SaveOrUpdate(data);
        }
        catch (Exception e)
        {
            result = false;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgCuentasController), nameof(SaveOrUpdate));
        }
        
        var message = isUpdating ? "Cuenta actualizada correctamente"
            : "Cuenta guardada correctamente";
        
        var errorMessage = isUpdating ? "Ocurrió un error al actualizar el registro"
            : "Ocurrió un error al guardar el registro";

        return Json(new
        {
            success = result,
            message = result ? message : errorMessage
        });
    }

    [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_DMGCUENTAS_CAN_UPDATE)]
    [HttpPost]
    public async Task<JsonResult> GetOne([FromForm] GetAccountNameDto data)
    {
        DmgCuentasResultSet? result;

        try
        {
            result = await dmgCuentasRepository.GetOne(data);
        }
        catch (Exception e)
        {
            result = null;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgCuentasController), nameof(GetOne));
        }

        return Json(new
        {
            success = true,
            message = "Access data",
            data = result
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }
}