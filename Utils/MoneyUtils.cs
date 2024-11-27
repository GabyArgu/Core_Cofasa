using System.Globalization;

namespace CoreContable.Utils;

public abstract class MoneyUtils {
    static MoneyUtils() {
    }

    // Modificado para devolver "0.00" en lugar de "" cuando el valor es 0
    public static string FormatToString(double? amount) {
        if (amount == 0) return "0.00";
        return amount.HasValue ? amount.Value.ToString("N2", CultureInfo.InvariantCulture) : "0.00"; // Si no tiene valor, devuelve 0.00
    }

    public static double FormatDoubleFromDecimal(decimal? value) {
        return (double)Math.Round(value ?? 0, 2);
    }

    public static double FormatDoubleFromDouble(double? value) {
        return Math.Round(value ?? 0, 2);
    }

    // Modificado para devolver "0.00" en lugar de "" cuando el valor es 0
    public static string FormatStringFromDoubleTwoDecimals(double? value) {
        if (value == 0) {
            return "0.00"; // Cambio aquí
        }

        return value.HasValue ? value.Value.ToString("N2", CultureInfo.InvariantCulture) : "0.00"; // Cambié la parte de retorno para asegurar que se devuelva 0.00 si no tiene valor
    }

    // Modificado para devolver "0.00" en lugar de "" cuando el valor es 0
    public static string GetDefaultFormatAsString(double? value) {
        if (value == 0) return "0.00"; // Cambio aquí
        return value.HasValue ? value.Value.ToString("N2", CultureInfo.InvariantCulture) : "0.00"; // Asegura que siempre devuelva un valor formateado
    }

    public static double GetDefaultFormatAsDouble(double? value) {
        return Math.Round(value ?? 0, 2);
    }

    // Modificado para devolver "0.00" en lugar de "" cuando el valor es 0 o null
    public static string GetDefaultFormatAsString(decimal? value, bool? nullAsEmpty = true) {
        if (value == 0 || value == null) return "0.00"; // Cambio aquí

        return FormatDoubleFromDecimal(value.Value)
            .ToString("N2", CultureInfo.InvariantCulture);
    }

    // Modificado para devolver "0.00" en lugar de "" cuando el valor es 0
    public static string FormatStringFromDecimalTwoDecimals(decimal? value) {
        if (value == 0) {
            return "0.00"; // Cambio aquí
        }

        return value.HasValue
            ? FormatDoubleFromDecimal(value.Value)
                .ToString("F2", CultureInfo.InvariantCulture)
            : "0.00"; // Cambié para devolver 0.00 si no tiene valor
    }

    internal static double? FormatDoubleFromDouble (decimal? saldoAnterior) {
        throw new NotImplementedException ( );
    }
}
