using System.Text.Json;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace CoreContable.Controllers;

public class ParamsController(
    ILogger<ParamsController> logger,
    IParamsRepository paramsRepository,
    ISecurityRepository securityRepository,
    IDmgCieCierreRepository dmgCieCierreRepository
) : Controller
{
    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_PARAMS_CONTA)]
    public IActionResult Conta()
    {
        return View();
    }

    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_PARAMS_CONTA)]
    [HttpGet]
    public async Task<JsonResult> GetContaParams([FromQuery] string ciaCod)
    {
        bool result;
        ContaParamsResultSet? data;

        try
        {
            data = await paramsRepository.GetContaParamsByCia(ciaCod);
            result = true;
        }
        catch (Exception e)
        {
            result = false;
            data = null;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(ParamsController), nameof(GetContaParams));
        }

        return Json(new
        {
            success = result,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_PARAMS_CONTA_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_PARAMS_CONTA_CAN_UPDATE}")]
    [HttpPost]
    public async Task<JsonResult> SaveContaParams([FromForm] ContaParamsDto data)
    {
        bool result;
        var isUpdating = false;

        try
        {
            var paramsResult = await paramsRepository.GetContaParamsByCia(data.COD_CIA);

            if (paramsResult == null)
            {
                result = await paramsRepository.SaveContaParams(data);
            }
            else
            {
                isUpdating = true;
                result = await paramsRepository.UpdateContaParams(data);
            }
        }
        catch (Exception e)
        {
            result = false;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(ParamsController), nameof(SaveContaParams));
        }

        var message = isUpdating ? "Paramétros actualizados correctamente"
            : "Paramétros guardados correctamente";

        var errorMessage = isUpdating ? "Ocurrió un error al actualizar el registro"
            : "Ocurrió un error al guardar el registro";

        return Json(new
        {
            success = result,
            message = result ? message : errorMessage
        });
    }

    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_PARAMS_PERIOD)]
    public IActionResult Period()
    {
        return View();
    }

    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_PARAMS_PERIOD)]
    [HttpGet]
    public async Task<JsonResult> GetPeriodParams([FromQuery] string ciaCod, [FromQuery] int period)
    {
        bool result;
        DmgPeriodResultSet? data;

        try
        {
            data = await paramsRepository.GetPeriodParamsByCiaAndPeriod(ciaCod, period);
            result = true;
        }
        catch (Exception e)
        {
            result = false;
            data = null;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(ParamsController), nameof(GetPeriodParams));
        }

        return Json(new
        {
            success = result,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    private static DateTime? GetStartDate(int period, string startMonth)
        => DateTimeUtils.ParseFromString($"01/{startMonth}/{period}");

    private DateTime? GetFinishDate(int period, string finishMonth)
    {
        try {
            var lastDayInFinishMonth = DateTime.DaysInMonth(period, int.Parse(finishMonth));
            return DateTimeUtils.ParseFromString($"{lastDayInFinishMonth}/{finishMonth}/{period}");
        } catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(ParamsController), nameof(GetFinishDate));
            return null;
        }
    }

    /// <summary>
    /// Saves period params, using start and finish date to calculate a list of months to fill period params table.
    /// </summary>
    /// <param name="data">
    /// Is a form data.
    /// - CodCia (CHAR3): Compay code.
    /// - Period (INT): Current year as int, example: 2024.
    /// - StartMonth (INT): Start month as int, example: 1.
    /// - FinishMonth (INT): Finish month as int, example: 12.
    /// - Status (CHAR): Current period status, it can be: "A" for Active and "C" for finished.
    /// </param>
    /// <returns></returns>
    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_PARAMS_PERIOD_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_PARAMS_PERIOD_CAN_UPDATE}")]
    [HttpPost]
    public async Task<JsonResult> SavePeriodParams([FromForm] DmgPeriodDto data)
    {
        bool result;
        var isUpdating = false;

        try
        {
            var oldValue = await paramsRepository.GetPeriodParamsByCiaAndPeriod(data.CodCia, data.Period);
            data.Opened = GetStartDate(data.Period, data.StartMonth);
            data.Closed = GetFinishDate(data.Period, data.FinishMonth);
            
            // Get a list of months between start and finish month.
            var monthRange = GetNumbersBetween(int.Parse(data.StartMonth), int.Parse(data.FinishMonth));
            isUpdating = oldValue != null;

            if (!isUpdating)
            {
                data.CreatedAt = DateTime.Now;
                data.CreatedBy = securityRepository.GetSessionUserName();
                result = await paramsRepository.SavePeriodParams(data);
                if (result && monthRange.Count > 0)
                {
                    result = await dmgCieCierreRepository.SaveList(data.CodCia, data.Period, monthRange);
                }
            }
            else
            {
                data.UpdatedAt = DateTime.Now;
                data.UpdatedBy = securityRepository.GetSessionUserName();
                result = await paramsRepository.UpdatePeriodParams(data);

                if (result && (data.StartMonth != oldValue?.StartMonth || data.FinishMonth != oldValue?.FinishMonth) &&
                    monthRange.Count > 0)
                {
                    result = await ProcessDmgCieCierre(data, oldValue);
                }
                // {
                //     result = await dmgCieCierreRepository.SaveList(data.CodCia, data.Period, monthRange);
                // }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(ParamsController), nameof(SavePeriodParams));
            result = false;
        }
        
        var message = isUpdating ? "Paramétros actualizados correctamente"
            : "Paramétros guardados correctamente";

        var errorMessage = isUpdating ? "Ocurrió un error al actualizar el registro"
            : "Ocurrió un error al guardar el registro";

        return Json(new
        {
            success = result,
            message = result ? message : errorMessage
        });
    }
    
    private async Task<bool> ProcessDmgCieCierre(DmgPeriodDto data, DmgPeriodResultSet oldValue)
    {
        var monthRange = GetNumbersBetween(int.Parse(data.StartMonth), int.Parse(data.FinishMonth));
        var oldMonthRange = GetNumbersBetween(int.Parse(oldValue.StartMonth), int.Parse(oldValue.FinishMonth));

        if (monthRange.Count==0 && oldMonthRange.Count==0)
        {
            return true;
        }
        
        // Caso 1: las listas tienen los mismo valores.
        if (monthRange.All(oldMonthRange.Contains) && monthRange.Count == oldMonthRange.Count)
        {
            return true;
        }

        // Caso 2: La lista nueva tiene valores pero la lista de valores anteriores no.
        if (monthRange.Count > 0 && oldMonthRange.Count == 0)
        {
            return await dmgCieCierreRepository.SaveList(data.CodCia, data.Period, monthRange);
        }
        
        // Caso 3: La lista de roles anteriores tiene valores pero la del formulario no.
        // En este caso se remueven todos los meses.
        if (oldMonthRange.Count > 0 && monthRange.Count == 0)
        {
            return await dmgCieCierreRepository.DeleteList(data.CodCia, data.Period, oldMonthRange);
        }
        
        // Caso 4: Ambas listas tienen valores pero presentan diferente lenght.
        // Se asume que se elimo o se agrego un rol o ambos casos.
        if (monthRange.Count != oldMonthRange.Count || monthRange.Count == oldMonthRange.Count)
        {
            var monthsToAdd = monthRange.Except(oldMonthRange).ToList();
            var monthsToDelete = oldMonthRange.Except(monthRange).ToList();

            var result1 = monthsToAdd.Count <= 0 || await dmgCieCierreRepository.SaveList(data.CodCia, data.Period, monthsToAdd);
            var result2 = monthsToDelete.Count <= 0 || await dmgCieCierreRepository.DeleteList(data.CodCia, data.Period, monthsToDelete);

            return result1 && result2;
        }

        // if (monthRange.Count == 0 || oldMonthRange.Count == 0) return false;
        //
        // var monthsToAdd = monthRange.Except(oldMonthRange).ToList();
        // var monthsToDelete = oldMonthRange.Except(monthRange).ToList();
        //
        // if (monthsToAdd.Count > 0)
        // {
        //     await dmgCieCierreRepository.SaveList(data.CodCia, data.Period, monthsToAdd);
        // }
        //
        // if (monthsToDelete.Count > 0)
        // {
        //     await dmgCieCierreRepository.DeleteList(data.CodCia, data.Period, monthsToDelete);
        // }

        return true;
    }

    /// <summary>
    /// Returns a list of month between open and closed period dates.
    /// </summary>
    /// <param name="ciaCod"></param>
    /// <param name="period"></param>
    /// <returns></returns>
    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_PARAMS_PERIOD)]
    [HttpGet]
    public async Task<JsonResult> GetDmgCieCierreForDt([FromQuery] string ciaCod, [FromQuery] int period)
    {
        bool success = false;
        List<DmgCieCierreResultSet> cieCierreList = [];

        try
        {
            cieCierreList = await dmgCieCierreRepository.GetDmgCieCierreForDt(ciaCod, period);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(ParamsController), nameof(GetDmgCieCierreForDt));
        }

        success = cieCierreList.Count > 0;
        var message = success ? "Access data" : "No data found";

        return Json(new
        {
            success = success,
            message = message,
            data = cieCierreList
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_PARAMS_PERIOD_CAN_CHANGE_STATUS)]
    [HttpPost]
    public async Task<JsonResult> ChangeDmgCieCierreStatus([FromForm] DmgCieCierreDto data)
    {
        bool result;

        try
        {
            result = await paramsRepository.ChangeStatusDmgCieCierre(data);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(ParamsController), nameof(ChangeDmgCieCierreStatus));
            result = false;
        }

        var message = result ? "Estado de cierre actualizado correctamente"
            : "Ocurrió un error al actualizar el registro";

        return Json(new
        {
            success = result,
            message
        });
    }

    private static List<int> GetNumbersBetween(int start, int finish)
    {
        if (start > finish) return [];
        if (start < 1 || finish > 12) return [];

        var numbers = new List<int>();
        for (var i = start; i <= finish; i++)
        {
            numbers.Add(i);
        }

        return numbers;
    }
}