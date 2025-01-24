using System.Text.Json;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Controllers;

public class CentroCuentaController (
    ILogger<CentroCuentaController> logger,
    IContaRepoRepository contaRepoRepository,
    ICentroCuentaRepository centroCuentaRepository,
    ISecurityRepository securityRepository
) : Controller {
    [IsAuthorized (alias: CC.THIRD_LEVEL_PERMISSION_ADMIN_CENTRO_CUENTA)]
    [HttpGet]
    public async Task<JsonResult> GetAll ([FromQuery] string codCia, [FromQuery] string codCC) {
        List<CentroCuentaResultSet>? data;

        try {
            data = await centroCuentaRepository.GetAllByCia (codCia, codCC);
        }
        catch (Exception e) {
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (CentroCuentaController), nameof (GetAll));
            data = null;
        }

        return Json (new {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized (alias: $"{CC.THIRD_LEVEL_PERMISSION_CENTRO_CUENTA_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_CENTRO_CUENTA_CAN_UPDATE}")]
    [HttpPost]
    public async Task<JsonResult> SaveOrUpdate ([FromForm] CentroCuentaDto data) {
        bool result;
        var isUpdating = false;

        var allCuentas = await centroCuentaRepository.GetAllByCia(data.COD_CIA, data.CENTRO_COSTO);

        bool existeCuenta = allCuentas.Any(p => p.CuentaContable == data.CENTRO_CUENTA);

        if (existeCuenta)
        {
            return Json(new
            {
                success = false,
                message = "Ya existe la cuenta en este centro de costo"
            });
        }

        try
        {
            if (data.ESTADO.IsNullOrEmpty ( )) {
                data.ESTADO = "I";
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

            result = await centroCuentaRepository.SaveOrUpdate (data);
        }
        catch (Exception e) {
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (CentroCuentaController), nameof (SaveOrUpdate));
            result = false;
        }

        var message = isUpdating ? "Cuenta de centro de costo actualizada correctamente"
            : "Cuenta agregada al centro de costo correctamente";

        var errorMessage = isUpdating ? "Ocurrió un error al actualizar el registro"
            : "Ocurrió un error al guardar el registro";

        return Json (new {
            success = result,
            message = result ? message : errorMessage
        });
    }

    [IsAuthorized (alias: CC.THIRD_LEVEL_PERMISSION_CENTRO_CUENTA_CAN_UPDATE)]
    [HttpGet]
    public async Task<JsonResult> GetOne ([FromQuery] string codCia, [FromQuery] string codCC, [FromQuery] string cta1,
        [FromQuery] string cta2, [FromQuery] string cta3, [FromQuery] string cta4, [FromQuery] string cta5,
        [FromQuery] string cta6) {
        CentroCuentaResultSet? result;

        try {
            result = await centroCuentaRepository
                .GetOne (codCia, codCC, cta1, cta2, cta3, cta4, cta5, cta6);
        }
        catch (Exception e) {
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (CentroCuentaController), nameof (GetOne));
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
                       $"{CC.THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_UPDATE}")]
    [HttpGet]
    public async Task<JsonResult> GetToSelect2 (
    [FromQuery] string codCia,
    [FromQuery] string centroCosto,
    [FromQuery] string q = "",
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10) {
        var centroCuentaList = new List<Select2ResultSet> ( );

        // Normalizamos el término de búsqueda, eliminando ceros adicionales
        //var normalizedQuery = string.IsNullOrEmpty (q) ? "" : q.Replace ("0", string.Empty); // Elimina ceros si hay búsqueda

        try {
            // Llamar al repositorio para obtener los resultados con el término de búsqueda normalizado
            centroCuentaList = await centroCuentaRepository
                .GetForSelect2 (codCia, centroCosto, q, page, pageSize);

            // Si no se encuentran resultados, devuelve un mensaje adecuado
            if (centroCuentaList == null || !centroCuentaList.Any ( )) {
                return Json (new { success = false, message = "No se encontraron resultados." });
            }

            // Devuelve los resultados del Select2 con los ceros ya aplicados a través del modelo
            return Json (new {
                success = true,
                results = centroCuentaList.Select (x => new {
                    id = x.id,
                    text = x.FormattedText  // Usamos el FormattedText en lugar de text
                }),
                more = centroCuentaList.Count > 0 ? centroCuentaList.Last ( ).more : false
            });
        }
        catch (Exception e) {
            // Captura y log de cualquier error en el proceso
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}", nameof (CentroCuentaController), nameof (GetToSelect2));
            return Json (new { success = false, message = "Ocurrió un error al procesar la solicitud." });
        }
    }
}