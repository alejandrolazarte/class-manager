namespace ClassManager.ImportExport.Tabular;

public static class FormulaEscaping
{
    public const char EscapeCharacter = '\'';

    private static readonly char[] FormulaStartCharacters = ['=', '+', '-', '@', '\t', '\r'];
    private static readonly char[] SignCharacters = ['+', '-'];
    private static readonly char[] PhoneSeparatorCharacters = [' ', '-', '.', '(', ')'];

    public static string Escape(string value) =>
        StartsLikeFormula(value) && !IsSignedNumber(value) ? EscapeCharacter + value : value;

    public static string Unescape(string value) =>
        value.Length > 1 && value[0] == EscapeCharacter && StartsLikeFormula(value[1..]) ? value[1..] : value;

    private static bool StartsLikeFormula(string value) =>
        value.Length > 0 && FormulaStartCharacters.Contains(value[0]);

    private static bool IsSignedNumber(string value) =>
        SignCharacters.Contains(value[0])
        && value[1..].All(character => char.IsAsciiDigit(character) || PhoneSeparatorCharacters.Contains(character));
}
