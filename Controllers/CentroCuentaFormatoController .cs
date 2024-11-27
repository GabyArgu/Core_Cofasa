using System.Text.Json;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Controllers;

public class CentroCuentaFormatoController(
    ILogger<CentroCuentaFormatoController> logger,
    ICentroCuentaRepository centroCuentaRepository,
    ISecurityRepository securityRepository
) : Controller {

    [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_ADMIN_CENTRO_CUENTA)]
    public IActionResult Index() {
        return View(); // Carga la vista principal para el formato de Centro Cuenta
    }

    [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_ADMIN_CENTRO_CUENTA)]
    [HttpGet]
    public async Task<JsonResult> GetAll([FromQuery] string ciaCod, [FromQuery] string codCC) {
        List<CentroCuentaResultSet>? data;

        try {
            data = await centroCuentaRepository.GetAllByCia(ciaCod, codCC);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}", nameof(CentroCuentaFormatoController), nameof(GetAll));
            data = null;
        }

        return Json(new {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_CENTRO_CUENTA_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_CENTRO_CUENTA_CAN_UPDATE}")]
    [HttpPost]
    public async Task<JsonResult> SaveOrUpdate([FromForm] CentroCuentaDto data) {
        bool result;
        var isUpdating = false;

        try {
            if (data.ESTADO.IsNullOrEmpty()) {
                data.ESTADO = "0";
            }

            if (data.isUpdating.IsNullOrEmpty()) {
                data.FECHA_CREACION = DateTime.Now;
                data.USUARIO_CREACION = securityRepository.GetSessionUserName();
                isUpdating = false;
            }
            else {
                data.FECHA_MODIFICACION = DateTime.Now;
                data.USUARIO_MODIFICACION = securityRepository.GetSessionUserName();
                isUpdating = true;
            }

            result = await centroCuentaRepository.SaveOrUpdate(data);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}", nameof(CentroCuentaFormatoController), nameof(SaveOrUpdate));
            result = false;
        }

        var message = isUpdating ? "Cuenta de centro de costo actualizada correctamente" : "Cuenta agregada al centro de costo correctamente";
        var errorMessage = isUpdating ? "Ocurrió un error al actualizar el registro" : "Ocurrió un error al guardar el registro";

        return Json(new {
            success = result,
            message = result ? message : errorMessage
        });
    }

    [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_CENTRO_CUENTA_CAN_UPDATE)]
    [HttpGet]
    public async Task<JsonResult> GetOne([FromQuery] string ciaCod, [FromQuery] string codCC, [FromQuery] string cta1,
        [FromQuery] string cta2, [FromQuery] string cta3, [FromQuery] string cta4, [FromQuery] string cta5,
        [FromQuery] string cta6) {
        CentroCuentaResultSet? result;

        try {
            result = await centroCuentaRepository.GetOne(ciaCod, codCC, cta1, cta2, cta3, cta4, cta5, cta6);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}", nameof(CentroCuentaFormatoController), nameof(GetOne));
            result = null;
        }

        return Json(new {
            success = true,
            message = "Access data",
            data = result
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }
}
