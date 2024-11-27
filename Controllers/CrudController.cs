using CoreContable.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace CoreContable.Controllers;

public abstract class CrudController : Controller
{
    protected static string GetQueryParamAsString(HttpRequest request, string key, string defaultValue = "")
    {
        return request.Query[key].FirstOrDefault() ?? defaultValue;
    }

    protected static int GetQueryParamAsInt(HttpRequest request, string key)
    {
        return int.Parse(GetQueryParamAsString(request, key));
    }

    protected static double GetQueryParamAsDouble(HttpRequest request, string key)
    {
        return double.Parse(GetQueryParamAsString(request, key));
    }

    protected static string GetFormValueAsString(HttpRequest request, string key, string defaultValue = "")
    {
        return request.Form[key].FirstOrDefault() ?? defaultValue;
    }

    protected static int GetFormValueAsInt(HttpRequest request, string key)
    {
        return int.Parse(GetFormValueAsString(request, key));
    }

    protected static double GetFormValueAsDouble(HttpRequest request, string key)
    {
        return double.Parse(GetFormValueAsString(request, key));
    }

    /// <summary>
    /// For POST request.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected static DataTabletDto GetDtParams(HttpRequest request)
    {
        var start = GetFormValueAsInt(request, "start");
        var length = GetFormValueAsInt(request, "length");
        var draw = GetFormValueAsInt(request, "draw");
        var search = GetFormValueAsString(request, "search[value]");
        var orderIndex = GetFormValueAsInt(request, "order[0][column]");
        var orderDirection = GetFormValueAsString(request, "order[0][dir]", "asc");
        var pageIndex = start / length;

        if (orderIndex > 0) orderIndex+=-1;

        return new DataTabletDto
        {
            draw = draw,
            start = start,
            length = length,
            search = search,
            orderIndex = orderIndex,
            orderDirection = orderDirection,
            pageIndex = pageIndex
        };
    }

    /// <summary>
    /// For GET request.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected static DataTabletDto GetDtParamsFromQuery(HttpRequest request)
    {
        var start = GetQueryParamAsInt(request, "start");
        var length = GetQueryParamAsInt(request, "length");
        var draw = GetQueryParamAsInt(request, "draw");
        var search = GetQueryParamAsString(request, "search[value]");
        var orderIndex = GetQueryParamAsInt(request, "order[0][column]");
        var orderDirection = GetQueryParamAsString(request, "order[0][dir]", "asc");
        var pageIndex = start / length;

        if (orderIndex > 0) orderIndex+=-1;

        return new DataTabletDto
        {
            draw = draw,
            start = start,
            length = length,
            search = search,
            orderIndex = orderIndex,
            orderDirection = orderDirection,
            pageIndex = pageIndex
        };
    }
}