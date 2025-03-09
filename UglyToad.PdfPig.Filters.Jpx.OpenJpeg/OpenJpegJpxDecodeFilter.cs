using System.Buffers.Binary;
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
        public ReadOnlyMemory<byte> Decode(ReadOnlySpan<byte> input, DictionaryToken streamDictionary, IFilterProvider filterProvider, int filterIndex)
        {
            using (var reader = new OpenJpegDotNet.IO.Reader(input))
            {
                var codecFormat = GetCodecFormat(input);

                if (!reader.ReadHeader(codecFormat))
                {
                    throw new InvalidDataException($"Invalid JPEG 2000 (JPF filter) data: Could not read '{codecFormat}' header.");
                }

                using (var i = reader.Decode())
                using (var raw = i.ToRawBitmap())
                {
                    return raw.Bytes;
                }
            }
        }

        /// <summary>
        /// Get bits per component values for Jp2 (Jpx) encoded images (first component).
        /// </summary>
        private static CodecFormat GetCodecFormat(ReadOnlySpan<byte> jp2Bytes)
        {
            // Ensure the input has at least 12 bytes for the signature box
            if (jp2Bytes.Length < 12)
            {
                throw new InvalidOperationException("Input is too short to be a valid JPEG2000 file.");
            }

            // Verify the JP2 signature box
            uint length = BinaryPrimitives.ReadUInt32BigEndian(jp2Bytes.Slice(0, 4));
            if (length == 0xFF4FFF51)
            {
                // J2K format detected (SOC marker) (See GHOSTSCRIPT-688999-2.pdf)
                return CodecFormat.J2k;
            }

            uint type = BinaryPrimitives.ReadUInt32BigEndian(jp2Bytes.Slice(4, 4));
            uint magic = BinaryPrimitives.ReadUInt32BigEndian(jp2Bytes.Slice(8, 4));
            if (length == 0x0000000C && type == 0x6A502020 && magic == 0x0D0A870A)
            {
                // JP2 format detected
                return CodecFormat.Jp2;
            }

            if (length == 0x0000000C && type == 0x6A502058 && magic == 0x0D0A870A)
            {
                // JPX format detected
                return CodecFormat.Jpx;
            }

            throw new InvalidOperationException("Invalid JP2, J2K or JPX signature.");
        }
    }
}