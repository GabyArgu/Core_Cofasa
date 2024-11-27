using System.Text.Json;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace CoreContable.Controllers;

public class DetRepositoryController(
    ISecurityRepository securityRepository,
    IContaRepoRepository contaRepoRepository,
    IDetRepoRepository detRepoRepository,
    ILogger<DetRepositoryController> logger
) : Controller
{
    [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_DET_REPOSITORIO)]
    public IActionResult Index()
    {
        return View();
    }

    [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_DET_REPOSITORIO)]
    [HttpGet]
    public async Task<JsonResult> GetForDt([FromQuery] string codCia, [FromQuery] int period, 
        [FromQuery] string doctoType, [FromQuery] int numPoliza)
    {
        List<DetRepositorioResultSet>? data;

        try
        {
            data = await detRepoRepository.GetAllBy(
                codCia,
                period,
                doctoType,
                numPoliza
            );
        }
        catch (Exception e)
        {
            data = null;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DetRepositoryController), nameof(GetForDt));
        }
    
        return Json(new
        {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_DET_REPOSITORIO_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_DET_REPOSITORIO_CAN_UPDATE}")]
    [HttpPost]
    public async Task<JsonResult> SaveOrUpdate([FromForm] DetRepositorioDto data)
    {
        var result = false;
        var updateTotalResult = false;

        try
        {
            var operation = data.detOPERACION ?? "";
            var sessionUser = securityRepository.GetSessionUserName();

            result = operation switch
            {
                "ACTUALIZAR" => await detRepoRepository.UpdateOne(data),
                "ELIMINAR" => await detRepoRepository.DeleteOne(
                    data.det_COD_CIA,
                    data.det_PERIODO ?? 0,
                    data.det_TIPO_DOCTO,
                    data.det_NUM_POLIZA,
                    data.det_CORRELAT ?? 0
                ),
                _ => await detRepoRepository.SaveOne(data)
            };

            if (result)
            {
                updateTotalResult = await contaRepoRepository.UpdateTotalPoliza(
                    data.det_COD_CIA, data.det_PERIODO ?? 0,
                    data.det_TIPO_DOCTO, data.det_NUM_POLIZA);
            }
        }
        catch (Exception e)
        {
            result = false;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DetRepositoryController), nameof(SaveOrUpdate));
        }

        var message = data.detOPERACION switch
        {
            "ACTUALIZAR" => "Registro actualizado correctamente",
            "ELIMINAR" => "Registro eliminado correctamente",
            _ => "Registro guardado correctamente"
        };

        var errorMessage = data.detOPERACION switch
        {
            "ACTUALIZAR" => "Ocurrió un error al actualizar el registro",
            "ELIMINAR" => "Ocurrió un error al eliminar el registro",
            _ => "Ocurrió un error al guardar el registro"
        };

        return Json(new
        {
            success = result,
            message = result ? message : errorMessage
        });
    }

    // private async Task<bool> UpdateRepoHeaderTotal(string codCia, int periodo, string tipoDocto, int numPoliza)
    // {
    //     bool result;
    //
    //     try
    //     {
    //         var accountsByHeader = await detRepoRepository
    //             .GetAllBy(codCia, periodo, tipoDocto, numPoliza);
    //         if (accountsByHeader.Count == 0) return false;
    //
    //         var total = accountsByHeader.Sum(account => account.ABONO) ?? 0;
    //         result = await contaRepoRepository.UpdateTotalPoliza(codCia, periodo, tipoDocto, numPoliza, total);
    //     }
    //     catch (Exception e)
    //     {
    //         result = false;
    //     }
    //
    //     return result;
    // }
}