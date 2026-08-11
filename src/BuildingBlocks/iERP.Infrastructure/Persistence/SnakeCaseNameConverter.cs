using System.Text.RegularExpressions;

namespace iERP.Infrastructure.Persistence;

public static partial class SnakeCaseNameConverter
{
    public static string ToSnakeCase(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        var snake = CamelBoundary().Replace(name, "$1_$2");
        return snake.ToLowerInvariant();
    }

    [GeneratedRegex("([a-z0-9])([A-Z])")]
    private static partial Regex CamelBoundary();
}
