using System.Diagnostics;
using System.Text.Json;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace CoreContable.Controllers;

public class DmgPolizaController(
    ILogger<DmgPolizaController> logger,
    ISecurityRepository securityRepository,
    IDmgPolizaRepository dmgPolizaRepository,
    IDmgDoctosRepository dmgDoctosRepository
) : CrudController
{
    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_DMGPOLIZA)]
    public IActionResult Index()
    {
        return View();
    }

    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_DMGPOLIZA)]
    [HttpPost]
    public async Task<JsonResult> GetForDt(string? fechaInicio, string? fechaFin, string? doctoType)
    {
        DataTableResultSet<List<DmgPolizaResultSet>>? dataTableResultSet;

        try
        {
            var currentCia = securityRepository.GetSessionCiaCode();
            var dtParams = GetDtParams(Request);
            dataTableResultSet = await dmgPolizaRepository.GetAllBy(dataTabletDto: dtParams, codCia: currentCia,
                fechaInicio: fechaInicio, fechaFin: fechaFin, tipoDocto: doctoType);
        }
        catch (Exception e)
        {
            dataTableResultSet = null;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgPolizaController), nameof(GetForDt));
        }

        return Json(dataTableResultSet, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_DMGPOLIZA_CAN_SEE_DETAIL)]
    [HttpGet]
    public async Task<JsonResult> GetOneBy([FromQuery] string codCia, [FromQuery] int period,
        [FromQuery] string doctoType, [FromQuery] int numPoliza)
    {
        DmgPolizaResultSet? data;

        try
        {
            data = await dmgPolizaRepository.GetOneBy(
                codCia,
                period,
                doctoType,
                numPoliza
            );
    
            if (data != null)
            {
                var currentCia = securityRepository.GetSessionCiaCode();
                var dmgDocto = await dmgDoctosRepository.GetOneDmgDoctoByCia(currentCia, doctoType);
                data.selTIPO_DOCTO = new Select2ResultSet
                {
                    id = dmgDocto?.TIPO_DOCTO ?? "",
                    text = dmgDocto?.DESCRIP_TIPO  ?? ""
                };

                // Getting STAT_POLIZA to select2.
                data.selSTAT_POLIZA = new Select2ResultSet()
                {
                    id = data.STAT_POLIZA,
                    text = data.STAT_POLIZA == "G" ? "Grabada" : "Revisada"
                };
            }
        }
        catch (Exception e)
        {
            data = null;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgPolizaController), nameof(GetOneBy));
        }

        return Json(new
        {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_DMGPOLIZA_CAN_UNCAPITALIZE)]
    [HttpPost]
    public async Task<JsonResult> UncapitalizeAccounts([FromForm] CapitalizeAccountDto data)
    {
        bool result;

        try
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            result = await dmgPolizaRepository.UncapitalizeAccounts(data);
            stopwatch.Stop();

            // Si el proceso dura menos de 1 segundo el dialog de loading no se oculta en el frontend.
            var elapsed = stopwatch.ElapsedMilliseconds;
            if (elapsed < 1000)
            {
                var milliseconds = 1000 - (int) elapsed;
                Thread.Sleep(milliseconds);
            }
        }
        catch (Exception e)
        {
            result = false;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgPolizaController), nameof(UncapitalizeAccounts));
        }

        var message = result ? "Asientos desmayorizados correctamente"
            : "Ocurrió un error al desmayorizar los asientos";

        return Json(new
        {
            success = result, message
        });
    }
}