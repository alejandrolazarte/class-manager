using System.Text.RegularExpressions;

namespace ClassManager.Core.Domain.Businesses;

public static partial class GoogleMapsLink
{
    private const string ShareHost = "maps.app.goo.gl";
    private const string ShortLinkHost = "goo.gl";
    private const string MapsHost = "maps.google.com";
    private const string MapsPath = "/maps";

    public static bool IsValid(string link)
    {
        if (!Uri.TryCreate(link, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            return false;
        }

        var host = uri.Host.ToLowerInvariant();
        return host == ShareHost
            || host == MapsHost
            || (host == ShortLinkHost && IsMapsPath(uri))
            || (GoogleHostPattern().IsMatch(host) && IsMapsPath(uri));
    }

    private static bool IsMapsPath(Uri uri) =>
        uri.AbsolutePath.Equals(MapsPath, StringComparison.OrdinalIgnoreCase)
        || uri.AbsolutePath.StartsWith(MapsPath + "/", StringComparison.OrdinalIgnoreCase);

    [GeneratedRegex("^(www\\.)?google\\.[a-z]{2,3}(\\.[a-z]{2})?$")]
    private static partial Regex GoogleHostPattern();
}
