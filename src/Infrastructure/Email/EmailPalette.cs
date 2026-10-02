using System.Globalization;

namespace ClassManager.Infrastructure.Email;

internal sealed record EmailPalette(string Primary, string PrimaryStrong, string PrimarySoft, string NoteLabel, string Accent)
{
    public const string DefaultThemeColor = "#0076b4";
    public const string White = "#ffffff";
    public const string Black = "#000000";

    private const double MinimumTextContrastRatio = 4.5;
    private const double AdjustmentStep = 0.06;
    private const double StrongShadeWeight = 0.2;
    private const double SoftTintWeight = 0.92;
    private const double MaximumChannelValue = 255;
    private const double LinearThreshold = 0.03928;
    private const double LinearDivisor = 12.92;
    private const double GammaOffset = 0.055;
    private const double GammaDivisor = 1.055;
    private const double GammaExponent = 2.4;
    private const double RedWeight = 0.2126;
    private const double GreenWeight = 0.7152;
    private const double BlueWeight = 0.0722;
    private const double FlareOffset = 0.05;
    private const int ChannelCount = 3;
    private const int ChannelHexLength = 2;
    private const string HexPrefix = "#";
    private const string ChannelHexFormat = "x2";

    private static readonly int MaximumAdjustmentSteps = (int)Math.Ceiling(1 / AdjustmentStep);

    public static EmailPalette From(string? themeColor, string? accentColor)
    {
        var seed = themeColor ?? DefaultThemeColor;
        var primary = AdjustUntilReadable(seed, Black, White);
        var primaryStrong = Mix(primary, Black, StrongShadeWeight);
        var primarySoft = Mix(seed, White, SoftTintWeight);
        return new EmailPalette(
            primary,
            primaryStrong,
            primarySoft,
            AdjustUntilReadable(primaryStrong, Black, primarySoft),
            accentColor is null ? primary : AdjustUntilReadable(accentColor, Black, White));
    }

    public static double ContrastRatio(string firstHexColor, string secondHexColor)
    {
        var firstLuminance = RelativeLuminance(firstHexColor);
        var secondLuminance = RelativeLuminance(secondHexColor);
        return (Math.Max(firstLuminance, secondLuminance) + FlareOffset) / (Math.Min(firstLuminance, secondLuminance) + FlareOffset);
    }

    private static string AdjustUntilReadable(string color, string towardColor, string againstColor)
    {
        for (var step = 0; step <= MaximumAdjustmentSteps; step++)
        {
            var candidate = Mix(color, towardColor, Math.Min(step * AdjustmentStep, 1));
            if (ContrastRatio(candidate, againstColor) >= MinimumTextContrastRatio)
            {
                return candidate;
            }
        }

        return towardColor;
    }

    private static string Mix(string baseHexColor, string targetHexColor, double targetWeight)
    {
        var baseChannels = Channels(baseHexColor);
        var targetChannels = Channels(targetHexColor);
        return HexPrefix + string.Concat(Enumerable.Range(0, ChannelCount).Select(index =>
            ((int)Math.Round(baseChannels[index] + ((targetChannels[index] - baseChannels[index]) * targetWeight), MidpointRounding.AwayFromZero))
                .ToString(ChannelHexFormat, CultureInfo.InvariantCulture)));
    }

    private static double RelativeLuminance(string hexColor)
    {
        var channels = Channels(hexColor).Select(ToLinearChannel).ToArray();
        return (RedWeight * channels[0]) + (GreenWeight * channels[1]) + (BlueWeight * channels[2]);
    }

    private static double ToLinearChannel(int channel)
    {
        var normalizedChannel = channel / MaximumChannelValue;
        return normalizedChannel <= LinearThreshold
            ? normalizedChannel / LinearDivisor
            : Math.Pow((normalizedChannel + GammaOffset) / GammaDivisor, GammaExponent);
    }

    private static int[] Channels(string hexColor) =>
        [.. Enumerable.Range(0, ChannelCount).Select(index =>
            int.Parse(hexColor.AsSpan(HexPrefix.Length + (index * ChannelHexLength), ChannelHexLength), NumberStyles.HexNumber, CultureInfo.InvariantCulture))];
}
