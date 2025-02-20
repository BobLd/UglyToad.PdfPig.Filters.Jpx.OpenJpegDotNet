# UglyToad.PdfPig.Filters.Jpx.OpenJpegDotNet
PdfPig implementation of the JPX (Jpeg2000) filter, based on OpenJpegDotNet (cinderblocks' fork OpenJpegDotNet https://github.com/cinderblocks/OpenJpegDotNet/tree/ee6168b0a92f1632f2276108638fecdb665779ce)

## Usage
```csharp
// Create your filter provider
public sealed class MyFilterProvider : BaseFilterProvider
{
    /// <summary>
    /// The single instance of this provider.
    /// </summary>
    public static readonly IFilterProvider Instance = new MyFilterProvider();

    /// <inheritdoc/>
    private MyFilterProvider() : base(GetDictionary())
    {
    }

    private static Dictionary<string, IFilter> GetDictionary()
    {
        var ascii85 = new Ascii85Filter();
        var asciiHex = new AsciiHexDecodeFilter();
        var ccitt = new CcittFaxDecodeFilter();
        var dct = new DctDecodeFilter();
        var flate = new FlateFilter();
        var jbig2 = new Jbig2DecodeFilter();
        var jpx = new OpenJpegJpxDecodeFilter(); // new filter
        var runLength = new RunLengthFilter();
        var lzw = new LzwFilter();

        return new Dictionary<string, IFilter>
        {
            { NameToken.Ascii85Decode.Data, ascii85 },
            { NameToken.Ascii85DecodeAbbreviation.Data, ascii85 },
            { NameToken.AsciiHexDecode.Data, asciiHex },
            { NameToken.AsciiHexDecodeAbbreviation.Data, asciiHex },
            { NameToken.CcittfaxDecode.Data, ccitt },
            { NameToken.CcittfaxDecodeAbbreviation.Data, ccitt },
            { NameToken.DctDecode.Data, dct }, // new filter
            { NameToken.DctDecodeAbbreviation.Data, dct },
            { NameToken.FlateDecode.Data, flate },
            { NameToken.FlateDecodeAbbreviation.Data, flate },
            { NameToken.Jbig2Decode.Data, jbig2 },
            { NameToken.JpxDecode.Data, jpx },
            { NameToken.RunLengthDecode.Data, runLength },
            { NameToken.RunLengthDecodeAbbreviation.Data, runLength },
            { NameToken.LzwDecode.Data, lzw },
            { NameToken.LzwDecodeAbbreviation.Data, lzw }
        };
    }
}



var parsingOption = new ParsingOptions()
{
	UseLenientParsing = true,
	SkipMissingFonts = true,
	FilterProvider = MyFilterProvider.Instance
};

using (var doc = PdfDocument.Open("test.pdf", parsingOption))
{
	int i = 0;
	foreach (var page in doc.GetPages())
	{
		foreach (var pdfImage in page.GetImages())
		{
			Assert.True(pdfImage.TryGetPng(out var bytes));

			File.WriteAllBytes($"image_{i++}.png", bytes);
		}
	}
}

```
