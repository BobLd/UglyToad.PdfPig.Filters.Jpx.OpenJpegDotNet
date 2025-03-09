using System.Buffers.Binary;

namespace UglyToad.PdfPig.Filters.Jpx.OpenJpeg.Tests
{
    // https://www.exiftool.org/TagNames/Jpeg2000.html#ColorSpec

    internal static class Jpeg2000HelperLocal
    {
        /// <summary>
        /// Get bits per component values for Jp2 (Jpx) encoded images (first component).
        /// </summary>
        public static Jpeg2000ColorSpace GetColorSpace(ReadOnlySpan<byte> jp2Bytes)
        {
            // Ensure the input has at least 12 bytes for the signature box
            if (jp2Bytes.Length < 12)
            {
                throw new InvalidOperationException("Input is too short to be a valid JP2 file.");
            }

            // Verify the JP2 signature box
            uint length = BinaryPrimitives.ReadUInt32BigEndian(jp2Bytes.Slice(0, 4));
            if (length == 0xFF4FFF51)
            {
                // J2K format detected (SOC marker) (See GHOSTSCRIPT-688999-2.pdf)
                return ParseCodestreamCS(jp2Bytes);
            }

            uint type = BinaryPrimitives.ReadUInt32BigEndian(jp2Bytes.Slice(4, 4));
            uint magic = BinaryPrimitives.ReadUInt32BigEndian(jp2Bytes.Slice(8, 4));
            if (length == 0x0000000C && type == 0x6A502020 && magic == 0x0D0A870A)
            {
                // JP2 format detected
                return ParseBoxes(jp2Bytes.Slice(12));
            }

            throw new InvalidOperationException("Invalid JP2 or J2K signature.");
        }

        private static Jpeg2000ColorSpace ParseBoxes(ReadOnlySpan<byte> jp2Bytes)
        {
            int offset = 0;
            while (offset < jp2Bytes.Length)
            {
                if (offset + 8 > jp2Bytes.Length)
                {
                    throw new InvalidOperationException("Invalid JP2 or J2K box structure.");
                }

                // Read box length and type
                uint boxLength = BinaryPrimitives.ReadUInt32BigEndian(jp2Bytes.Slice(offset, 4));
                uint boxType = BinaryPrimitives.ReadUInt32BigEndian(jp2Bytes.Slice(offset + 4, 4));

                if (boxType == 0x6A703268) // 'jp2h'
                {
                    // Parse the codestream to find the SIZ marker
                    return ParseCodestreamCS(jp2Bytes.Slice(offset + 8));
                }

                // Move to the next box
                offset += (int)(boxLength > 0 ? boxLength : 8); // Box length of 0 means the rest of the file
            }
            
            throw new InvalidOperationException("Codestream box not found in JP2 or J2K file.");
        }

        private static Jpeg2000ColorSpace ParseCodestreamCS(ReadOnlySpan<byte> codestream)
        {
            int offset = 0;
            while (offset + 2 <= codestream.Length)
            {
                uint boxLength = BinaryPrimitives.ReadUInt32BigEndian(codestream.Slice(offset, 4));
                uint boxType = BinaryPrimitives.ReadUInt32BigEndian(codestream.Slice(offset + 4, 4));
                
                if (boxType == 0x636F6C72) // 'colr'
                {
                    byte method = codestream.Slice(offset + 8)[0];
                    //byte precedence = codestream.Slice(offset + 8 + 1)[0];
                    //byte approximation = codestream.Slice(offset + 8 + 2)[0];

                    switch (method)
                    {
                        case 1:
                            return (Jpeg2000ColorSpace)BinaryPrimitives.ReadUInt32BigEndian(codestream.Slice(offset + 8 + 3, 4));
                            break;
                        
                        default:
                            return Jpeg2000ColorSpace.Unknown;
                            break;
                    }
                }

                // Move to the next marker
                offset += (int)(boxLength > 0 ? boxLength : 8); // Box length of 0 means the rest of the file
            }

            throw new InvalidOperationException("'colr' tag not found in JPEG2000 codestream.");
        }

        public enum Jpeg2000ColorSpace : uint
        {
            Bi_level = 0,
            YCbCr_1 = 1,
            YCbCr_2 = 3,
            YCbCr_3 = 4,
            PhotoYCC = 9,
            CMY = 11,
            CMYK = 12,
            YCCK = 13,
            CIELab = 14,
            Bi_level_2 = 15,
            sRGB = 16,
            Grayscale = 17,
            sYCC = 18,
            CIEJab = 19,
            e_sRGB = 20,
            ROMM_RGB = 21,
            YPbPr_1125_60 = 22,
            YPbPr_1250_50 = 23,
            e_sYCC = 24,
            
            Unknown = 255
        }
    }
}
