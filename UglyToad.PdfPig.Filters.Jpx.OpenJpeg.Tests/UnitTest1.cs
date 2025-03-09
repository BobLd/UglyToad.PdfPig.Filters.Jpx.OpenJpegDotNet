using System.Reflection.PortableExecutable;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Graphics.Colors;
using UglyToad.PdfPig.Images;
using UglyToad.PdfPig.Tokens;
using UglyToad.PdfPig.XObjects;

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
        public void Jpex16bitTest()
        {
            var parsingOption = new ParsingOptions()
            {
                UseLenientParsing = true,
                SkipMissingFonts = true,
                FilterProvider = MyFilterProvider.Instance
            };

            using (var doc = PdfDocument.Open(Path.Combine(_path, "GHOSTSCRIPT-696875-3.pdf"), parsingOption))
            {
                var page = doc.GetPage(1);
                var image = page.GetImages().ElementAt(0);
                bool success = image.TryGetPng(out var bytes);
                
                PngFromPdfImageFactory.TryGenerate(image, out var test);

                Assert.True(success);
                Assert.NotNull(bytes);
            }
        }

        internal static class PngFromPdfImageFactory
        {
            public static bool TryGenerate(IPdfImage image, out byte[]? bytes)
            {
                bytes = null;

                var colorSpace = image.ColorSpaceDetails;
                
                if (colorSpace is null && image is XObjectImage { IsJpxEncoded: true } xoi)
                {
                    var cs = Jpeg2000HelperLocal.GetColorSpace(image.RawBytes);
                    switch (cs)
                    {
                        case Jpeg2000HelperLocal.Jpeg2000ColorSpace.Grayscale:
                            colorSpace = DeviceGrayColorSpaceDetails.Instance;
                            break;
                        
                        default:
                            throw new Exception(cs.ToString());
                    }
                }

                var hasValidDetails = colorSpace != null && !(colorSpace is UnsupportedColorSpaceDetails);

                var isColorSpaceSupported = hasValidDetails && colorSpace!.BaseType != ColorSpace.Pattern;

                if (!isColorSpaceSupported || !image.TryGetBytesAsMemory(out var imageMemory))
                {
                    return false;
                }

                var bytesPure = imageMemory.Span;

                try
                {
                    bytesPure = ColorSpaceDetailsByteConverter.Convert(colorSpace!, bytesPure,
                        image.BitsPerComponent, image.WidthInSamples, image.HeightInSamples);
                    var numberOfComponents = colorSpace!.BaseNumberOfColorComponents;

                    var is3Byte = numberOfComponents == 3;

                    //var builder = PngBuilder.Create(image.WidthInSamples, image.HeightInSamples, false);

                    var requiredSize = (image.WidthInSamples * image.HeightInSamples * numberOfComponents);

                    var actualSize = bytesPure.Length;
                    var isCorrectlySized = bytesPure.Length == requiredSize ||
                        // Spec, p. 37: "...error if the stream contains too much data, with the exception that
                        // there may be an extra end-of-line marker..."
                        (actualSize == requiredSize + 1 && bytesPure[actualSize - 1] == ReadHelper.AsciiLineFeed) ||
                        (actualSize == requiredSize + 1 && bytesPure[actualSize - 1] == ReadHelper.AsciiCarriageReturn) ||
                        // The combination of a CARRIAGE RETURN followed immediately by a LINE FEED is treated as one EOL marker.
                        (actualSize == requiredSize + 2 &&
                            bytesPure[actualSize - 2] == ReadHelper.AsciiCarriageReturn &&
                            bytesPure[actualSize - 1] == ReadHelper.AsciiLineFeed);

                    if (!isCorrectlySized)
                    {
                        return false;
                    }

                    if (colorSpace.BaseType == ColorSpace.DeviceCMYK || numberOfComponents == 4)
                    {
                        int i = 0;
                        for (int col = 0; col < image.HeightInSamples; col++)
                        {
                            for (int row = 0; row < image.WidthInSamples; row++)
                            {
                                /*
                                 * Where CMYK in 0..1
                                 * R = 255 × (1-C) × (1-K)
                                 * G = 255 × (1-M) × (1-K)
                                 * B = 255 × (1-Y) × (1-K)
                                 */

                                double c = (bytesPure[i++] / 255d);
                                double m = (bytesPure[i++] / 255d);
                                double y = (bytesPure[i++] / 255d);
                                double k = (bytesPure[i++] / 255d);
                                var r = (byte)(255 * (1 - c) * (1 - k));
                                var g = (byte)(255 * (1 - m) * (1 - k));
                                var b = (byte)(255 * (1 - y) * (1 - k));

                                //builder.SetPixel(r, g, b, row, col);
                            }
                        }
                    }
                    else if (is3Byte)
                    {
                        int i = 0;
                        for (int col = 0; col < image.HeightInSamples; col++)
                        {
                            for (int row = 0; row < image.WidthInSamples; row++)
                            {
                                //builder.SetPixel(bytesPure[i++], bytesPure[i++], bytesPure[i++], row, col);
                            }
                        }
                    }
                    else
                    {
                        int i = 0;
                        for (int col = 0; col < image.HeightInSamples; col++)
                        {
                            for (int row = 0; row < image.WidthInSamples; row++)
                            {
                                byte pixel = bytesPure[i++];
                                //builder.SetPixel(pixel, pixel, pixel, row, col);
                            }
                        }
                    }

                    //bytes = builder.Save();
                    return true;
                }
                catch
                {
                    // ignored.
                }

                return false;
            }
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
                        Assert.NotNull(bytes);

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