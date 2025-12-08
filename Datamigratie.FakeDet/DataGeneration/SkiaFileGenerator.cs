using SkiaSharp;

namespace Datamigratie.FakeDet.DataGeneration;

public static class SkiaFileGenerator
{
    private const string Arial = "Arial";

    public static Task MakePng(Stream stream, string title, int width = 1200, int height = 700)
        => MakeImage(stream, title, width, height, SKEncodedImageFormat.Png, quality: 100);

    public static Task MakeJpeg(Stream stream, string title, int width = 1200, int height = 700, int quality = 85)
        => MakeImage(stream, title, width, height, SKEncodedImageFormat.Jpeg, quality);

    private static async Task MakeImage(Stream stream, string title, int width, int height, SKEncodedImageFormat fmt, int quality)
    {
        using var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul));
        var canvas = surface.Canvas;

        canvas.Clear(SKColors.White);

        // header bar
        using (var paint = new SKPaint { Color = new SKColor(245, 245, 245), IsAntialias = true })
            canvas.DrawRect(new SKRect(0, 0, width, 110), paint);

        using var normalFace = SKTypeface.FromFamilyName(Arial, SKFontStyle.Normal);
        using var boldFace = SKTypeface.FromFamilyName(Arial, SKFontStyle.Bold);



        // big label
        using (var paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
        })
        {
            using var font = new SKFont(boldFace, 44);
            canvas.DrawText("TESTDOCUMENT", 40, 70, SKTextAlign.Left, font, paint);
        }

        // title + timestamp
        using (var paint = new SKPaint
        {
            Color = new SKColor(70, 70, 70),
            IsAntialias = true,
        })
        {
            using var font = new SKFont(normalFace, 28);
            canvas.DrawText(Truncate(title, 70), 40, 145, SKTextAlign.Left, font, paint);
            canvas.DrawText(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), 40, 190 ,SKTextAlign.Left, font, paint);
        }

        // light diagonal watermark
        using (var paint = new SKPaint
        {
            Color = new SKColor(220, 220, 220),
            IsAntialias = true,
        })
        {
            canvas.Save();
            canvas.RotateDegrees(-20, width / 2f, height / 2f);
            using var font = new SKFont(boldFace, 60);
            canvas.DrawText("NIET ECHT • TESTDATA", width * 0.12f, height * 0.65f, SKTextAlign.Left, font, paint);
            canvas.Restore();
        }

        using var image = surface.Snapshot();
        using var data = image.Encode(fmt, quality);
        await using var inputStream = data.AsStream();
        await inputStream.CopyToAsync(stream);
    }

    private static string Truncate(string s, int max)
        => string.IsNullOrWhiteSpace(s) ? "Bijlage" : (s.Length <= max ? s : s[..max] + "…");
}
