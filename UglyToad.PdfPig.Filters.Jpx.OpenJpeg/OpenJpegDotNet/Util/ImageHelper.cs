// ReSharper disable once CheckNamespace
namespace OpenJpegDotNet
{

    internal static class ImageHelper
    {
        #region Methods

        public static Image FromRaw(byte[] raw, int width, int height, int stride, int channels, bool interleaved)
        {
            if (raw == null)
                throw new ArgumentNullException(nameof(raw));
            
            var byteAllocated = 1;
            var colorSpace = ColorSpace.Srgb;
            var precision = 24u / (uint)channels;
            var gap = stride - width * channels;

            using (var compressionParameters = new CompressionParameters())
            {
                OpenJpeg.SetDefaultEncoderParameters(compressionParameters);

                var subsamplingDx = compressionParameters.SubsamplingDx;
                var subsamplingDy = compressionParameters.SubsamplingDy;

                var componentParametersArray = new ImageComponentParameters[channels];
                for (var i = 0; i < channels; i++)
                {
                    componentParametersArray[i] = new ImageComponentParameters
                    {
                        Precision = precision,
                        Bpp = precision,
                        Signed = false,
                        Dx = (uint)subsamplingDx,
                        Dy = (uint)subsamplingDy,
                        Width = (uint)width,
                        Height = (uint)height
                    };
                }

                var image = OpenJpeg.ImageCreate((uint)channels, componentParametersArray, colorSpace);
                if (image == null)
                    return null;
                
                image.X0 = (uint)compressionParameters.ImageOffsetX0;
                image.Y0 = (uint)compressionParameters.ImageOffsetY0;
                image.X1 = image.X0 == 0 ? (uint)(width - 1) * (uint)subsamplingDx + 1 : image.X0 + (uint)(width - 1) * (uint)subsamplingDx + 1;
                image.Y1 = image.Y0 == 0 ? (uint)(height - 1) * (uint)subsamplingDy + 1 : image.Y0 + (uint)(height - 1) * (uint)subsamplingDy + 1;

                unsafe
                {
                    fixed (byte* pRaw = &raw[0])
                    {
                        // Bitmap data is interleave.
                        // Convert it to planer
                        if (byteAllocated == 1)
                        {
                            if (interleaved)
                            {
                                for (var i = 0; i < channels; i++)
                                {
                                    var target = image.Components[i].Data;
                                    var pTarget = (int*)target;
                                    var source = pRaw + i;
                                    for (var y = 0; y < height; y++)
                                    {
                                        for (var x = 0; x < width; x++)
                                        {
                                            *pTarget = *source;
                                            pTarget++;
                                            source += channels;
                                        }

                                        source += gap;
                                    }
                                }
                            }
                            else
                            {
                                for (var i = 0; i < channels; i++)
                                {
                                    var target = image.Components[i].Data;
                                    var pTarget = (int*)target;
                                    var source = pRaw + i * (stride * height);
                                    for (var y = 0; y < height; y++)
                                    {
                                        for (var x = 0; x < width; x++)
                                        {
                                            *pTarget = *source;
                                            pTarget++;
                                            source++;
                                        }

                                        source += gap;
                                    }
                                }
                            }
                        }
                    }
                }

                return image;
            }
        }
        #endregion
    }
}
