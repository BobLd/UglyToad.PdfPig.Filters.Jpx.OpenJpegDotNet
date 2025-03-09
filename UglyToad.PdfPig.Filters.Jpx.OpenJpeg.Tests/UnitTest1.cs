using UglyToad.PdfPig.Tokens;

namespace UglyToad.PdfPig.Filters.Jpx.OpenJpeg.Tests
{
    public class UnitTest1
    {
        private static readonly string _path = "Documents";
        private const string _outputPath = "Output";

        public static IEnumerable<object[]> GetAllDocuments => Directory.EnumerateFiles("Documents", "*.pdf")
            .Select(p => new object[] { p });

        public UnitTest1()
        {
            Directory.CreateDirectory(_outputPath);
        }

        [Fact]
        public void FoxboxSrHdmiTest1()
        {
            Assert.True(Environment.Is64BitProcess);
            
            var parsingOption = new ParsingOptions()
            {
                UseLenientParsing = true,
                SkipMissingFonts = true,
                FilterProvider = MyFilterProvider.Instance
            };
            
            using (var doc = PdfDocument.Open(Path.Combine(_path, "68-1990-01_A.pdf"), parsingOption))
            {
                int i = 0;
                foreach (var page in doc.GetPages())
                {
                    foreach (var pdfImage in page.GetImages())
                    {
                        if (pdfImage.ImageDictionary.TryGet(NameToken.Filter, out NameToken filter))
                        {
                            if (!filter.Data.ToUpper().Contains("JPX"))
                            {
                                continue;
                            }
                        }

                        Assert.True(pdfImage.TryGetPng(out var bytes));

                        File.WriteAllBytes($"image_{i++}.png", bytes);
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetAllDocuments))]
        public void RenderImages(string docPath)
        {
            var parsingOption = new ParsingOptions()
            {
                UseLenientParsing = true,
                SkipMissingFonts = true,
                FilterProvider = MyFilterProvider.Instance
            };

            string fileRootName = Path.ChangeExtension(Path.GetFileName(docPath), "");

            bool isDocumentJpx = false;

            using (var doc = PdfDocument.Open(docPath, parsingOption))
            {
                int i = 0;
                foreach (var page in doc.GetPages())
                {
                    foreach (var pdfImage in page.GetImages())
                    {
                        bool isJpx = false;

                        if (!pdfImage.ImageDictionary.ContainsKey(NameToken.Filter))
                        {
                            continue;
                        }

                        if (pdfImage.ImageDictionary.TryGet(NameToken.Filter, out NameToken filter))
                        {
                            isJpx = filter.Data.ToUpper().Contains("JPX");
                            if (isJpx)
                            {
                                isDocumentJpx = true;
                            }
                        }
                        else if (pdfImage.ImageDictionary.TryGet(NameToken.Filter, out ArrayToken filterArray))
                        {
                            isJpx = filterArray.Data.OfType<NameToken>().Any(f => f.Data.ToUpper().Contains("JPX"));
                            if (isJpx)
                            {
                                isDocumentJpx = true;
                            }
                        }
                        else
                        {
                            throw new Exception("Could not get a valid Filter.");
                        }

                        if (!isJpx)
                        {
                            continue;
                        }

                        Assert.True(pdfImage.TryGetPng(out var bytes));

                        File.WriteAllBytes(Path.Combine(_outputPath, $"{fileRootName}_{i++}.png"), bytes);
                    }
                }
            }

            if (!isDocumentJpx)
            {
                throw new Exception("No JPX image in document.");
            }
        }

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
                var jpx = new OpenJpegJpxDecodeFilter(); // new
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
                    { NameToken.DctDecode.Data, dct },
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
    }
}