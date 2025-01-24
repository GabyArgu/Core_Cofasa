using System.Text.Json;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Controllers;

public class DmgDoctosController(
    IDmgDoctosRepository dmgDoctosRepository,
    ISecurityRepository securityRepository,
    ILogger<DmgDoctosController> logger
) : Controller
{
    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_DMGDOCTOS)]
    public IActionResult Index()
    {
        return View();
    }

    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_DMGDOCTOS)]
    [HttpGet]
    public async Task<JsonResult> GetDmgDoctos([FromQuery] string ciaCod)
    {
        List<DmgDoctosResultSet>? data;

        try
        {
            data = await dmgDoctosRepository.GetDmgDoctosByCia(ciaCod);
        }
        catch (Exception e)
        {
            data = null;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgDoctosController), nameof(GetDmgDoctos));
        }

        return Json(new
        {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_DMGDOCTOS_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_DMGDOCTOS_CAN_UPDATE}")]
    [HttpPost]
    public async Task<JsonResult> SaveOrUpdate ([FromForm] DmgDoctosDto data) {
        bool result;
        var isUpdating = false;

        try {
            // Validar si el TIPO_DOCTO ya existe en la base de datos
            var existingDocto = await dmgDoctosRepository.GetOneDmgDoctoByCia (data.COD_CIA, data.TIPO_DOCTO);

            if (existingDocto != null && data.isUpdating.IsNullOrEmpty ( )) {
                // Devolver un mensaje de error si ya existe y no es una actualización
                return Json (new {
                    success = false,
                    message = "El tipo de documento ya existe en la base de datos."
                });
            }

            if (data.isUpdating.IsNullOrEmpty ( )) {
                data.FechaCreacion = DateTime.Now;
                data.UsuarioCreacion = securityRepository.GetSessionUserName ( );
                isUpdating = false;
            }
            else {
                data.FechaModificacion = DateTime.Now;
                data.UsuarioModificacion = securityRepository.GetSessionUserName ( );
                isUpdating = true;
            }

            result = await dmgDoctosRepository.SaveOrUpdateDmgDocto (data);
        }
        catch (Exception e) {
            result = false;
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (DmgDoctosController), nameof (SaveOrUpdate));
        }

        var message = isUpdating ? "Tipo de partida actualizado correctamente"
            : "Tipo de partida guardado correctamente";

        var errorMessage = isUpdating ? "Ocurrió un error al actualizar el registro"
            : "Ocurrió un error al guardar el registro";

        return Json (new {
            success = result,
            message = result ? message : errorMessage
        });
    }



    [IsAuthorized (alias: CC.THIRD_LEVEL_PERMISSION_DMGDOCTOS_CAN_UPDATE)]
    [HttpGet]
    public async Task<JsonResult> GetOneDmgDocto([FromQuery] string ciaCod, [FromQuery] string doctoType)
    {
        DmgDoctosResultSet? data;

        try
        {
            data = await dmgDoctosRepository.GetOneDmgDoctoByCia(ciaCod, doctoType);
        }
        catch (Exception e)
        {
            data = null;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgDoctosController), nameof(GetOneDmgDocto));
        }

        return Json(new
        {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_UPDATE}," +
                         $"{CC.SECOND_LEVEL_PERMISSION_ADMIN_REPOSITORIO}," +
                         $"{CC.SECOND_LEVEL_PERMISSION_ADMIN_DMGPOLIZA}")]
    [HttpGet]
    public async Task<JsonResult> GetToSelect2([FromQuery] string q, [FromQuery] int page, [FromQuery] int pageSize)
    {
        var doctos = new List<Select2ResultSet>();
        var currentCia = securityRepository.GetSessionCiaCode();

        try
        {
            doctos = await dmgDoctosRepository.GetForSelect2(currentCia, q, page, pageSize);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgDoctosController), nameof(GetToSelect2));
        }

        return Json(new
        {
            results = doctos,
            more = doctos.Count > 0 ? doctos.Last().more : false
        });
    }
}