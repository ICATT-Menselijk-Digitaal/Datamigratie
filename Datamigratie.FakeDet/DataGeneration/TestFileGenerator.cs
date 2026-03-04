using System.Buffers;
using System.IO.Compression;
using System.Text.Unicode;
using ClosedXML.Excel;

namespace Datamigratie.FakeDet.DataGeneration;

public static class TestFileGenerator
{
    public record GeneratedFile(string FileName, string MimeType, byte[] Bytes);

    private static readonly Dictionary<string, string> MimeToExt = new()
    {
        ["application/pdf"] = ".pdf",
        ["text/plain"] = ".txt",
        ["image/png"] = ".png",
        ["image/jpeg"] = ".jpg",
        ["application/vnd.openxmlformats-officedocument.wordprocessingml.document"] = ".docx",
        ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"] = ".xlsx"
    };

    public static Task Generate(Stream stream, string mimeType, long size, string? title = null)
    {
        title ??= "Testdocument";

        return mimeType switch
        {
            "text/plain" => MakeTxt(stream, title, size),
            "application/pdf" => MakeMinimalPdf(stream, title, size),
            "image/png" => MakePaddedAsync(stream, size, s => SkiaFileGenerator.MakePng(s, title)),
            "image/jpeg" => MakePaddedAsync(stream, size, s => SkiaFileGenerator.MakeJpeg(s, title)),
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => MakePaddedAsync(stream, size, s => MakeMinimalDocx(s, title)),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => MakePaddedAsync(stream, size, s => MakeMinimalXlsx(s, title)),
            _ => throw new NotSupportedException($"Unsupported mimetype: {mimeType}")
        };
    }

    /// <summary>
    /// Generates binary content (png/jpeg/docx/xlsx) into a temp buffer, then copies it to the
    /// output stream and pads with zero bytes to reach the declared size.
    /// This ensures Content-Length matches what was promised in Documentgrootte.
    /// </summary>
    private static async Task MakePaddedAsync(Stream stream, long size, Func<Stream, Task> generator)
    {
        await using var tempBuffer = new MemoryStream();
        await generator(tempBuffer);
        tempBuffer.Seek(0, SeekOrigin.Begin);
        await tempBuffer.CopyToAsync(stream);
        var remaining = size - tempBuffer.Length;
        if (remaining > 0)
        {
            await WritePaddingAsync(stream, remaining);
        }
    }

    private static async Task MakeTxt(Stream stream, string title, long size)
    {
        using var owner = MemoryPool<byte>.Shared.Rent();

        Utf8.TryWrite(owner.Memory.Span, $"""
        {title}
        Datum: {DateTime.Now:yyyy-MM-dd}
        Kenmerk: TEST-{Guid.NewGuid():N}

        Dit is automatisch gegenereerde testdata.
        """, out var written);

        await stream.WriteAsync(owner.Memory.Slice(0, written));

        var remaining = size - written;
        if (remaining > 0)
        {
            await WritePaddingAsync(stream, remaining);
        }
    }

    // Let op: dit is een minimale PDF die in veel viewers opent (voldoende voor tests)
    private static async Task MakeMinimalPdf(Stream stream, string title, long size)
    {
        using var owner = MemoryPool<byte>.Shared.Rent();

        var text = $"{title} - testpdf";
        text = text.Replace("(", "\\(").Replace(")", "\\)");

        Utf8.TryWrite(owner.Memory.Span, $"""
        %PDF-1.4
        1 0 obj<< /Type /Catalog /Pages 2 0 R >>endobj
        2 0 obj<< /Type /Pages /Kids [3 0 R] /Count 1 >>endobj
        3 0 obj<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents 4 0 R /Resources<< /Font<< /F1 5 0 R >> >> >>endobj
        4 0 obj<< /Length 44 >>stream
        BT /F1 24 Tf 72 760 Td ({text}) Tj ET
        endstream endobj
        5 0 obj<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>endobj
        xref
        0 6
        0000000000 65535 f
        trailer<< /Root 1 0 R /Size 6 >>
        startxref
        0
        %%EOF
        """, out var bytesWritten);

        await stream.WriteAsync(owner.Memory.Slice(0, bytesWritten));

        var remaining = size - bytesWritten;
        if (remaining > 0)
        {
            await WritePaddingAsync(stream, remaining);
        }
    }

    /// <summary>
    /// Writes <paramref name="count"/> zero bytes to <paramref name="stream"/> in chunks,
    /// so that the total bytes written matches the declared Documentgrootte.
    /// </summary>
    private static async Task WritePaddingAsync(Stream stream, long count)
    {
        const int ChunkSize = 81920;
        var chunk = new byte[Math.Min(ChunkSize, count)];
        while (count > 0)
        {
            var toWrite = (int)Math.Min(chunk.Length, count);
            await stream.WriteAsync(chunk.AsMemory(0, toWrite));
            count -= toWrite;
        }
    }

    // Minimal DOCX: een zip met de minimale Office Open XML onderdelen
    private static async Task MakeMinimalDocx(Stream stream, string title)
    {
        await using var tempFile = CreateTempFile();
        await using (var zip = new ZipArchive(tempFile, ZipArchiveMode.Create, leaveOpen: true))
        {
            await Add(zip, "[Content_Types].xml", () => """
            <?xml version="1.0" encoding="UTF-8"?>
            <Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
              <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
              <Default Extension="xml" ContentType="application/xml"/>
              <Override PartName="/word/document.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml"/>
            </Types>
            """u8);

            await Add(zip, "_rels/.rels", () => """
            <?xml version="1.0" encoding="UTF-8"?>
            <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
              <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="word/document.xml"/>
            </Relationships>
            """u8);

            await Add(zip, "word/document.xml", (Span<byte> span, out int bytes) => Utf8.TryWrite(span, $"""
            <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
            <w:document xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
              <w:body>
                <w:p><w:r><w:t>{EscapeXml(title)}</w:t></w:r></w:p>
                <w:p><w:r><w:t>Automatisch gegenereerde testdata.</w:t></w:r></w:p>
                <w:p><w:r><w:t>Datum: {DateTime.Now:yyyy-MM-dd}</w:t></w:r></w:p>
              </w:body>
            </w:document>
            """, out bytes));
        }
        await tempFile.FlushAsync();
        tempFile.Seek(0, SeekOrigin.Begin);
        await tempFile.CopyToAsync(stream);
    }

    private static FileStream CreateTempFile() => File.Create(Path.GetTempFileName(), 4096, FileOptions.DeleteOnClose);

    // Minimal XLSX: ook zip-achtig met minimale parts
    private static async Task MakeMinimalXlsx(Stream stream, string title)
    {
        await using var tempFile = CreateTempFile();
        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Sample Sheet");
        worksheet.Cell("A1").Value = title;
        worksheet.Cell("A2").Value = "Datum";
        worksheet.Cell("B2").Value = DateTime.Now;
        workbook.SaveAs(tempFile);
        await tempFile.FlushAsync();
        tempFile.Seek(0, SeekOrigin.Begin);
        await tempFile.CopyToAsync(stream);
    }

    delegate bool SpanAction(Span<byte> span, out int written);

    private static Task Add(ZipArchive zip, string path, Func<ReadOnlySpan<byte>> span)
    {
        return Add(zip, path, Handle);
        bool Handle(Span<byte> output, out int written)
        {
            var input = span();
            input.CopyTo(output);
            written = input.Length;
            return true;
        }
    }

    private static async Task Add(ZipArchive zip, string path, SpanAction handler)
    {
        using var owner = MemoryPool<byte>.Shared.Rent();
        var entry = zip.CreateEntry(path, CompressionLevel.Optimal);
        await using var stream = await entry.OpenAsync();
        handler(owner.Memory.Span, out var written);
        await stream.WriteAsync(owner.Memory.Slice(0, written));
        await stream.FlushAsync();
    }

    private static string EscapeXml(string s)
        => System.Security.SecurityElement.Escape(s) ?? "";
}
