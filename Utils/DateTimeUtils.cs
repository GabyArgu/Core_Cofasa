using System.Globalization;

namespace CoreContable.Utils;

public abstract class DateTimeUtils
{
    static DateTimeUtils()
    {
    }

    // public static DateTime? ParseFromString(string rawDate)
    // {
    //     try
    //     {
    //         return string.IsNullOrEmpty(rawDate) ? null : DateTime.ParseExact(rawDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
    //     }
    //     catch (Exception e)
    //     {
    //         return null;
    //     }
    // }
    
    public static DateTime? ParseFromString(string rawDate)
    {
        if (string.IsNullOrEmpty(rawDate)) return null;

        var formats = new[] { "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "dd/MM/yyyy" };
        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(rawDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }
        }

        return null;
    }

    public static string FormatToString(DateTime? date)
    {
        return date.HasValue ? date.Value.ToString("dd/MM/yyyy") : "";
    }
    
    public static string GetMonthName(int month)
    {
        var dateTimeFormatInfo = new CultureInfo("es-ES", false).DateTimeFormat;
        return dateTimeFormatInfo.GetMonthName(month);
    }
    
    public static string GetMonthNameFromString(string rawDate)
    {
        return GetMonthName(ParseFromString(rawDate).Value.Month);
    }
    
    public static string GetYearFromString(string rawDate)
    {
        return ParseFromString(rawDate).Value.Year.ToString();
    }
    
    public static string GetDayFromString(string rawDate)
    {
        return ParseFromString(rawDate).Value.Day.ToString();
    }
    
    public static string GetMonthFromString(string rawDate)
    {
        return ParseFromString(rawDate).Value.Month.ToString();
    }
    
    public static string GetCurrentTimeSpanAsStringForFileName()
    {
        return DateTime.Now.ToString("ddMMyyyy-HHmmss");
    }
    
    public static string GetCurrentTimeSpanAsString()
    {
        return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }
    
    public static int getYearFromStringDate(string rawDate)
    {
        return ParseFromString(rawDate).Value.Year;
    }
    
    public static int getMonthFromStringDate(string rawDate)
    {
        return ParseFromString(rawDate).Value.Month;
    }
}