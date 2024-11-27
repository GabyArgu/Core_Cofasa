using System.Text.Json;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Controllers;

public class CurrencyController(
    ICurrencyRepository currencyRepository,
    ISecurityRepository securityRepository,
    ILogger<CurrencyController> logger
) : Controller
{
    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_CURRENCY)]
    public IActionResult Index()
    {
        return View();
    }
    
    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_CURRENCY)]
    [HttpGet]
    public async Task<JsonResult> GetToDt()
    {
        List<CurrencyResultSet>? data;

        try
        {
            data = await currencyRepository.GetCurrencies();
        }
        catch (Exception e)
        {
            data = null;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(CurrencyController), nameof(GetToDt));
        }

        return Json(new
        {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_CURRENCY_CAN_UPDATE)]
    [HttpGet]
    public async Task<JsonResult> GetOneCurrency([FromQuery] string codCurrency)
    {
        CurrencyResultSet? data;

        try
        {
            data = await currencyRepository.GetOneCurrency(codCurrency);
        }
        catch (Exception e)
        {
            data = null;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(CurrencyController), nameof(GetOneCurrency));
        }

        return Json(new
        {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_CURRENCY_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_CURRENCY_CAN_UPDATE}")]
    [HttpPost]
    public async Task<JsonResult> SaveOrUpdate([FromForm] CurrencyDto data)
    {
        bool result;
        var isUpdating = false;

        try
        {
            if (data.isUpdating.IsNullOrEmpty())
            {
                data.FechaCreacion = DateTime.Now;
                data.UsuarioCreacion = securityRepository.GetSessionUserName();
                isUpdating = false;
            }
            else
            {
                data.FechaModificacion = DateTime.Now;
                data.UsuarioModificacion = securityRepository.GetSessionUserName();
                isUpdating = true;
            }

            result = await currencyRepository.SaveOrUpdateCurrency(data);
        }
        catch (Exception e)
        {
            result = false;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(CurrencyController), nameof(SaveOrUpdate));
        }
        
        var message = isUpdating ? "Moneda actualizada correctamente"
            : "Moneda guardada correctamente";
        
        var errorMessage = isUpdating ? "Ocurrió un error al actualizar el registro"
            : "Ocurrió un error al guardar el registro";

        return Json(new
        {
            success = result,
            message = result ? message : errorMessage
        });
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_UPDATE}" +
                         $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_COPY}")]
    [HttpGet]
    public async Task<JsonResult> GetCurrenciesToS2()
    {
        var currencies = new List<Select2ResultSet>();

        try
        {
            currencies = await currencyRepository.CallGetCurrencies();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(CurrencyController), nameof(GetCurrenciesToS2));
        }

        return Json(new
        {
            results = currencies
        });
    }
}