using OpenJpegDotNet;
using UglyToad.PdfPig.Tokens;

namespace UglyToad.PdfPig.Filters.Jpx.OpenJpeg
{
    /// <summary>
    /// JPX Filter for image data.
    /// <para>
    /// Based on <see href="https://github.com/cinderblocks/OpenJpegDotNet"/>.
    /// </para>
    /// </summary>
    public sealed class OpenJpegJpxDecodeFilter : IFilter
    {
        /// <inheritdoc/>
        public bool IsSupported => true;

        /// <inheritdoc/>
        public ReadOnlyMemory<byte> Decode(ReadOnlySpan<byte> input, DictionaryToken streamDictionary, int filterIndex)
        {
            byte[] image = input.ToArray();

            using (var reader = new OpenJpegDotNet.IO.Reader(image))
            {
                if (!reader.ReadHeader(CodecFormat.Jp2))
                {
                    throw new InvalidDataException("Invalid JPEG 2000 data: Could not read JPX header.");
                }

                using (var i = reader.Decode())
                using (var raw = i.ToRawBitmap())
                {
                    return raw.Bytes;
                }
            }
        }
    }
}