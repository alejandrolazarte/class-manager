using ClassManager.Core.Abstractions.Images;
using ClassManager.Storage.Images;
using SkiaSharp;

namespace ClassManager.Infrastructure.Storage;

internal sealed class SkiaAvatarImageService : IAvatarImageService
{
    private const int WebpQuality = 80;

    public DocumentContent? ToAvatar(byte[] content, int sideInPixels)
    {
        using var data = SKData.CreateCopy(content);
        using var codec = SKCodec.Create(data);
        using var decoded = codec is null ? null : SKBitmap.Decode(codec);
        if (codec is null || decoded is null)
        {
            return null;
        }

        using var upright = Upright(decoded, codec.EncodedOrigin);
        var cropSide = Math.Min(upright.Width, upright.Height);
        var targetSide = Math.Min(sideInPixels, cropSide);
        var crop = SKRect.Create((upright.Width - cropSide) / 2f, (upright.Height - cropSide) / 2f, cropSide, cropSide);
        using var surface = SKSurface.Create(new SKImageInfo(targetSide, targetSide, SKColorType.Rgba8888, SKAlphaType.Premul));
        using (var image = SKImage.FromBitmap(upright))
        {
            surface.Canvas.DrawImage(image, crop, SKRect.Create(targetSide, targetSide), new SKSamplingOptions(SKCubicResampler.Mitchell));
        }

        using var avatar = surface.Snapshot();
        using var encoded = avatar.Encode(SKEncodedImageFormat.Webp, WebpQuality);
        return encoded is null ? null : new DocumentContent(encoded.ToArray(), ImageFormat.WebpContentType, ImageFormat.Webp.FileExtension);
    }

    private static SKBitmap Upright(SKBitmap bitmap, SKEncodedOrigin origin)
    {
        var width = bitmap.Width;
        var height = bitmap.Height;
        var isTurnedSideways = origin is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;
        var upright = isTurnedSideways ? new SKBitmap(height, width) : new SKBitmap(width, height);
        using var canvas = new SKCanvas(upright);
        using var image = SKImage.FromBitmap(bitmap);
        canvas.SetMatrix(UprightMatrix(origin, width, height));
        canvas.DrawImage(image, 0, 0, SKSamplingOptions.Default);
        return upright;
    }

    private static SKMatrix UprightMatrix(SKEncodedOrigin origin, int width, int height) => origin switch
    {
        SKEncodedOrigin.TopRight => new SKMatrix(-1, 0, width, 0, 1, 0, 0, 0, 1),
        SKEncodedOrigin.BottomRight => new SKMatrix(-1, 0, width, 0, -1, height, 0, 0, 1),
        SKEncodedOrigin.BottomLeft => new SKMatrix(1, 0, 0, 0, -1, height, 0, 0, 1),
        SKEncodedOrigin.LeftTop => new SKMatrix(0, 1, 0, 1, 0, 0, 0, 0, 1),
        SKEncodedOrigin.RightTop => new SKMatrix(0, -1, height, 1, 0, 0, 0, 0, 1),
        SKEncodedOrigin.RightBottom => new SKMatrix(0, -1, height, -1, 0, width, 0, 0, 1),
        SKEncodedOrigin.LeftBottom => new SKMatrix(0, 1, 0, -1, 0, width, 0, 0, 1),
        _ => SKMatrix.Identity,
    };
}
