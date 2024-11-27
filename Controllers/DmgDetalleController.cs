using System.Text.Json;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace CoreContable.Controllers;

public class DmgDetalleController(
    IDmgDetalleRepository dmgDetalleRepository,
    ILogger<DmgDetalleController> logger
) : Controller
{
    [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_DMGPOLIZA_CAN_SEE_DETAIL)]
    [HttpGet]
    public async Task<JsonResult> GetForDt([FromQuery] string codCia, [FromQuery] int period,
        [FromQuery] string doctoType, [FromQuery] int numPoliza)
    {
        List<DmgDetalleResultSet>? data;

        try
        {
            data = await dmgDetalleRepository.GetAllBy(
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
                nameof(DmgDetalleController), nameof(GetForDt));
        }
    
        return Json(new
        {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }
}