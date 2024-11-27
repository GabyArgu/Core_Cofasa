using NCalc;

namespace CoreContable.Utils;

public class AccountUtils(IConfiguration configuration) {

    private readonly int _level1 = int.Parse(configuration["AccountFormatting:Level1"] ?? "0");
    private readonly int _level2 = int.Parse(configuration["AccountFormatting:Level2"] ?? "0");
    private readonly int _level3 = int.Parse(configuration["AccountFormatting:Level3"] ?? "0");
    private readonly int _level4 = int.Parse(configuration["AccountFormatting:Level4"] ?? "0");
    private readonly int _level5 = int.Parse(configuration["AccountFormatting:Level5"] ?? "0");
    private readonly int _level6 = int.Parse(configuration["AccountFormatting:Level6"] ?? "0");

    public string GetFormattedByLevel(int level, int account) {
        var raw = level switch {
            1 => account.ToString().PadLeft(_level1, '0'),
            2 => account.ToString().PadLeft(_level2, '0'),
            3 => account.ToString().PadLeft(_level3, '0'),
            4 => account.ToString().PadLeft(_level4, '0'),
            5 => account.ToString().PadLeft(_level5, '0'),
            6 => account.ToString().PadLeft(_level6, '0'),
            _ => account.ToString()
        };

        var isEmptyLevel = IsEmptyLevel(account, level);
        return isEmptyLevel && !ShowEmptyLevels(level) ? "" : $"{(level > 1 ? GetSeparator() : "")}{raw}";
    }

    private bool IsEmptyLevel(int account, int level) {
        var rawQuery = configuration[$"AccountFormatting:EmptyLevel{level}"];
        if (string.IsNullOrEmpty(rawQuery)) return false;

        var queryToEvaluate = rawQuery.Replace("#", $"{account}");
        var result = new Expression(queryToEvaluate).Evaluate();
        return (bool) (result ?? false);
    }

    public string GetSeparator() {
        return configuration["AccountFormatting:Separator"] ?? "";
    }

    private bool ShowEmptyLevels(int level) {
        return bool.Parse(configuration[$"AccountFormatting:ShowEmptyLevels{level}"] ?? "false");
    }
}