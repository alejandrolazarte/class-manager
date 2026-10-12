namespace ClassManager.Core.Domain.Products;

public static class ProductColors
{
    public const int KeyMaxLength = 20;
    public const int NameMaxLength = 20;

    public const string Black = "black";
    public const string Charcoal = "charcoal";
    public const string Gray = "gray";
    public const string LightGray = "light_gray";
    public const string White = "white";
    public const string Cream = "cream";
    public const string Beige = "beige";
    public const string Camel = "camel";
    public const string Brown = "brown";
    public const string Burgundy = "burgundy";
    public const string Red = "red";
    public const string Coral = "coral";
    public const string Pink = "pink";
    public const string Fuchsia = "fuchsia";
    public const string Lilac = "lilac";
    public const string Violet = "violet";
    public const string Navy = "navy";
    public const string Blue = "blue";
    public const string SkyBlue = "sky_blue";
    public const string Turquoise = "turquoise";
    public const string Mint = "mint";
    public const string Green = "green";
    public const string Olive = "olive";
    public const string Yellow = "yellow";
    public const string Orange = "orange";

    private static readonly IReadOnlyList<(string Key, string Name)> Palette =
    [
        (Black, "Negro"),
        (Charcoal, "Gris oscuro"),
        (Gray, "Gris"),
        (LightGray, "Gris claro"),
        (White, "Blanco"),
        (Cream, "Crema"),
        (Beige, "Beige"),
        (Camel, "Camel"),
        (Brown, "Marrón"),
        (Burgundy, "Bordó"),
        (Red, "Rojo"),
        (Coral, "Coral"),
        (Pink, "Rosa"),
        (Fuchsia, "Fucsia"),
        (Lilac, "Lila"),
        (Violet, "Violeta"),
        (Navy, "Azul marino"),
        (Blue, "Azul"),
        (SkyBlue, "Celeste"),
        (Turquoise, "Turquesa"),
        (Mint, "Verde agua"),
        (Green, "Verde"),
        (Olive, "Verde oliva"),
        (Yellow, "Amarillo"),
        (Orange, "Naranja"),
    ];

    private static readonly Dictionary<string, string> NamesByKey = Palette.ToDictionary(color => color.Key, color => color.Name);

    public static IReadOnlyList<string> Keys { get; } = [.. Palette.Select(color => color.Key)];

    public static bool IsKnown(string key) => NamesByKey.ContainsKey(key);

    public static string NameOf(string key) => NamesByKey.GetValueOrDefault(key, key);
}
