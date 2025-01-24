using System.Text.Json;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Controllers;

public class CentroCostoController (
    ILogger<CentroCostoController> logger,
    ICentroCostoRepository centroCostoRepository,
    ISecurityRepository securityRepository
) : Controller {
    [IsAuthorized (alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_CENTROCOSTO)]
    public IActionResult Index ( ) {
        return View ( );
    }

    [IsAuthorized (alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_CENTROCOSTO)]
    [HttpGet]
    public async Task<JsonResult> GetAll ([FromQuery] string ciaCod) {
        List<CentroCostoResultSet>? data;

        try {
            data = await centroCostoRepository.GetAllBy (ciaCod, null);
        }
        catch (Exception e) {
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (CentroCostoController), nameof (GetAll));
            data = null;
        }

        return Json (new {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized (alias: $"{CC.THIRD_LEVEL_PERMISSION_CENTROCOSTO_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_CENTROCOSTO_CAN_UPDATE}")]
    [HttpPost]
    public async Task<JsonResult> SaveOrUpdate ([FromForm] CentroCostoDto data) {
        bool result;
        var isUpdating = false;

        try
        {
            if (data.ACEPTA_DATOS.IsNullOrEmpty ( )) {
                data.ACEPTA_DATOS = "N";
            }

            if (data.isUpdating.IsNullOrEmpty ( )) {
                data.FECHA_CREACION = DateTime.Now;
                data.USUARIO_CREACION = securityRepository.GetSessionUserName ( );
                isUpdating = false;
            }
            else {
                data.FECHA_MODIFICACION = DateTime.Now;
                data.USUARIO_MODIFICACION = securityRepository.GetSessionUserName ( );
                isUpdating = true;
            }

            result = await centroCostoRepository.SaveOrUpdate (data);
        }
        catch (Exception e) {
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (CentroCostoController), nameof (SaveOrUpdate));
            result = false;
        }

        var message = isUpdating ? "Centro de costo actualizado correctamente"
            : "Centro de costo guardado correctamente";

        var errorMessage = isUpdating ? "Ocurrió un error al actualizar el registro"
            : "Ocurrió un error al guardar el registro";

        return Json (new {
            success = result,
            message = result ? message : errorMessage
        });
    }

    [IsAuthorized (alias: CC.THIRD_LEVEL_PERMISSION_CENTROCOSTO_CAN_UPDATE)]
    [HttpGet]
    public async Task<JsonResult> GetOne ([FromQuery] string codCia, [FromQuery] string codCentroCosto) {
        CentroCostoResultSet? result;

        try {
            result = await centroCostoRepository.GetOne (codCia, codCentroCosto);
        }
        catch (Exception e) {
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (CentroCostoController), nameof (GetOne));
            result = null;
        }

        return Json (new {
            success = true,
            message = "Access data",
            data = result
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized (alias: $"{CC.FIST_LEVEL_PERMISSION_REPORTS}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_UPDATE}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_CENTROCOSTO_CAN_COPY}")]
    [HttpGet]
    public async Task<JsonResult> GetToSelect2 ([FromQuery] string q, [FromQuery] int page, [FromQuery] int pageSize) {
        var ccostos = new List<Select2ResultSet> ( );
        var currentCia = securityRepository.GetSessionCiaCode ( );

        try {
            ccostos = await centroCostoRepository.GetForSelect2 (currentCia, q, page, pageSize);
        }
        catch (Exception e) {
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (CentroCostoController), nameof (GetToSelect2));
        }

        return Json (new {
            results = ccostos,
            more = ccostos.Count > 0 ? ccostos.Last ( ).more : false
        });
    }

    [IsAuthorized (alias: $"{CC.THIRD_LEVEL_PERMISSION_CENTROCOSTO_CAN_COPY}")]
    [HttpPost]
    public async Task<JsonResult> DoCopy ([FromForm] CopyCentroCostoDto data) {
        bool result;

        try {
            result = await centroCostoRepository.DoCopy (data.centroCosto, data.centroCosto2);
        }
        catch (Exception e) {
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (CentroCostoController), nameof (DoCopy));
            result = false;
        }

        var message = result ? "Centro de costo copiado correctamente"
            : "Ocurrió un error al copiar el centro de costo";

        return Json (new {
            success = result,
            message
        });
    }
}