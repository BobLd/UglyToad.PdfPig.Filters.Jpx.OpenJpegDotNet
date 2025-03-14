﻿using System;
using System.IO;
using System.Linq;
using Xunit;

// ReSharper disable once CheckNamespace
namespace OpenJpegDotNet.Tests
{
    public sealed partial class OpenJpegTest
    {

        #region Image

        [Fact]
        public void ImageColorSpace()
        {
            using var image = CreateImage();
            foreach (var value in Enum.GetValues(typeof(ColorSpace)).Cast<ColorSpace>())
            {
                image.ColorSpace = value;
                Assert.Equal(value, image.ColorSpace);
            }
        }

        [Fact]
        public void ImageNumberOfComponents()
        {
            const uint numComps = 3;
            using var image = CreateImage(numComps);
            Assert.Equal(numComps, image.NumberOfComponents);
        }

        [Fact]
        public void ImageX0()
        {
            using var image = CreateImage();
            foreach (var value in new[] { 0u, 1u, 10u })
            {
                image.X0 = value;
                Assert.Equal(value, image.X0);
            }
        }

        [Fact]
        public void ImageX1()
        {
            using var image = CreateImage();
            foreach (var value in new[] { 0u, 1u, 10u })
            {
                image.X1 = value;
                Assert.Equal(value, image.X1);
            }
        }

        [Fact]
        public void ImageY0()
        {
            using var image = CreateImage();
            foreach (var value in new[] { 0u, 1u, 10u })
            {
                image.Y0 = value;
                Assert.Equal(value, image.Y0);
            }
        }

        [Fact]
        public void ImageY1()
        {
            using var image = CreateImage();
            foreach (var value in new[] { 0u, 1u, 10u })
            {
                image.Y1 = value;
                Assert.Equal(value, image.Y1);
            }
        }

        #endregion

        #region ImageComponentParameters

        [Fact]
        public void ImageComponentParametersSigned()
        {
            using var parameter = new ImageComponentParameters();
            foreach (var value in new[] { true, false })
            {
                parameter.Signed = value;
                Assert.Equal(value, parameter.Signed);
            }
        }

        [Fact]
        public void ImageComponentParametersBpp()
        {
            using var parameter = new ImageComponentParameters();
            foreach (var value in new[] { 0u, 1u, 10u })
            {
                parameter.Bpp = value;
                Assert.Equal(value, parameter.Bpp);
            }
        }

        [Fact]
        public void ImageComponentParametersDx()
        {
            using var parameter = new ImageComponentParameters();
            foreach (var value in new[] { 0u, 1u, 10u })
            {
                parameter.Dx = value;
                Assert.Equal(value, parameter.Dx);
            }
        }

        [Fact]
        public void ImageComponentParametersDy()
        {
            using var parameter = new ImageComponentParameters();
            foreach (var value in new[] { 0u, 1u, 10u })
            {
                parameter.Dy = value;
                Assert.Equal(value, parameter.Dy);
            }
        }

        [Fact]
        public void ImageComponentParametersHeight()
        {
            using var parameter = new ImageComponentParameters();
            foreach (var value in new[] { 0u, 1u, 10u })
            {
                parameter.Height = value;
                Assert.Equal(value, parameter.Height);
            }
        }

        [Fact]
        public void ImageComponentParametersPrecision()
        {
            using var parameter = new ImageComponentParameters();
            foreach (var value in new[] { 0u, 1u, 10u })
            {
                parameter.Precision = value;
                Assert.Equal(value, parameter.Precision);
            }
        }

        [Fact]
        public void ImageComponentParametersWidth()
        {
            using var parameter = new ImageComponentParameters();
            foreach (var value in new[] { 0u, 1u, 10u })
            {
                parameter.Width = value;
                Assert.Equal(value, parameter.Width);
            }
        }

        [Fact]
        public void ImageComponentParametersX0()
        {
            using var parameter = new ImageComponentParameters();
            foreach (var value in new[] { 0u, 1u, 10u })
            {
                parameter.X0 = value;
                Assert.Equal(value, parameter.X0);
            }
        }

        [Fact]
        public void ImageComponentParametersY0()
        {
            using var parameter = new ImageComponentParameters();
            foreach (var value in new[] { 0u, 1u, 10u })
            {
                parameter.Y0 = value;
                Assert.Equal(value, parameter.Y0);
            }
        }

        #region ImageComponent

        [Fact]
        public void ImageComponentAlpha()
        {
            // ToDo: test transparency image
            const string testImage = "Bretagne1_0.j2k";
            var path = Path.GetFullPath(Path.Combine(TestImageDirectory, testImage));
            using var image = DecodeImageFromFile(path, CodecFormat.J2k);

            const ushort expected = 0;
            Assert.Equal(expected, image.Components[0].Alpha);
        }

        [Fact]
        public void ImageComponentBpp()
        {
            const string testImage = "Bretagne1_0.j2k";
            var path = Path.GetFullPath(Path.Combine(TestImageDirectory, testImage));
            using var image = DecodeImageFromFile(path, CodecFormat.J2k);

            const uint expected = 0;
            Assert.Equal(expected, image.Components[0].Bpp);
        }

        [Fact]
        public void ImageComponentData()
        {
            const string testImage = "Bretagne1_0.j2k";
            var path = Path.GetFullPath(Path.Combine(TestImageDirectory, testImage));
            using var image = DecodeImageFromFile(path, CodecFormat.J2k);
            
            Assert.NotEqual(IntPtr.Zero, image.Components[0].Data);
        }

        [Fact]
        public void ImageComponentDx()
        {
            const string testImage = "Bretagne1_0.j2k";
            var path = Path.GetFullPath(Path.Combine(TestImageDirectory, testImage));
            using var image = DecodeImageFromFile(path, CodecFormat.J2k);

            const uint expected = 1;
            Assert.Equal(expected, image.Components[0].Dx);
        }

        [Fact]
        public void ImageComponentDy()
        {
            const string testImage = "Bretagne1_0.j2k";
            var path = Path.GetFullPath(Path.Combine(TestImageDirectory, testImage));
            using var image = DecodeImageFromFile(path, CodecFormat.J2k);

            const uint expected = 1;
            Assert.Equal(expected, image.Components[0].Dy);
        }

        [Fact]
        public void ImageComponentFactor()
        {
            const string testImage = "Bretagne1_0.j2k";
            var path = Path.GetFullPath(Path.Combine(TestImageDirectory, testImage));
            using var image = DecodeImageFromFile(path, CodecFormat.J2k);

            const uint expected = 0;
            Assert.Equal(expected, image.Components[0].Factor);
        }

        [Fact]
        public void ImageComponentHeight()
        {
            const string testImage = "Bretagne1_0.j2k";
            var path = Path.GetFullPath(Path.Combine(TestImageDirectory, testImage));
            using var image = DecodeImageFromFile(path, CodecFormat.J2k);

            const uint expected = 480;
            Assert.Equal(expected, image.Components[0].Height);
        }

        [Fact]
        public void ImageComponentPrecision()
        {
            const string testImage = "Bretagne1_0.j2k";
            var path = Path.GetFullPath(Path.Combine(TestImageDirectory, testImage));
            using var image = DecodeImageFromFile(path, CodecFormat.J2k);

            const uint expected = 8;
            Assert.Equal(expected, image.Components[0].Precision);
        }

        [Fact]
        public void ImageComponentResolutionDecoded()
        {
            const string testImage = "Bretagne1_0.j2k";
            var path = Path.GetFullPath(Path.Combine(TestImageDirectory, testImage));
            using var image = DecodeImageFromFile(path, CodecFormat.J2k);

            const uint expected = 5;
            Assert.Equal(expected, image.Components[0].ResolutionDecoded);
        }

        [Fact]
        public void ImageComponentSigned()
        {
            const string testImage = "Bretagne1_0.j2k";
            var path = Path.GetFullPath(Path.Combine(TestImageDirectory, testImage));
            using var image = DecodeImageFromFile(path, CodecFormat.J2k);

            const bool expected = false;
            Assert.Equal(expected, image.Components[0].Signed);
        }

        [Fact]
        public void ImageComponentWidth()
        {
            const string testImage = "Bretagne1_0.j2k";
            var path = Path.GetFullPath(Path.Combine(TestImageDirectory, testImage));
            using var image = DecodeImageFromFile(path, CodecFormat.J2k);

            const uint expected = 640;
            Assert.Equal(expected, image.Components[0].Width);
        }

        [Fact]
        public void ImageComponentX0()
        {
            const string testImage = "Bretagne1_0.j2k";
            var path = Path.GetFullPath(Path.Combine(TestImageDirectory, testImage));
            using var image = DecodeImageFromFile(path, CodecFormat.J2k);

            const uint expected = 0;
            Assert.Equal(expected, image.Components[0].X0);
        }

        [Fact]
        public void ImageComponentY0()
        {
            const string testImage = "Bretagne1_0.j2k";
            var path = Path.GetFullPath(Path.Combine(TestImageDirectory, testImage));
            using var image = DecodeImageFromFile(path, CodecFormat.J2k);

            const uint expected = 0;
            Assert.Equal(expected, image.Components[0].Y0);
        }

        #endregion

        #endregion

        #region Functions

        [Fact]
        public void ImageCreate()
        {
            const uint numComps = 3;
            const int numCompsMax = 4;
            const int codeBlockWidthInitial = 64;
            const int codeBlockHeightInitial = 64;
            const int imageWidth = 2000;
            const int imageHeight = 2000;
            const int tileWidth = 1000;
            const int tileHeight = 1000;
            const uint compPrec = 8;
            const bool irreversible = false;
            const uint offsetX = 0;
            const uint offsetY = 0;

            using var codec = OpenJpeg.CreateCompress(CodecFormat.Jp2);
            using var compressionParameters = new CompressionParameters();
            OpenJpeg.SetDefaultEncoderParameters(compressionParameters);

            compressionParameters.TcpNumLayers = 1;
            compressionParameters.CodingParameterFixedQuality = 1;
            compressionParameters.TcpDistoratio[0] = 20;
            compressionParameters.CodingParameterTx0 = 0;
            compressionParameters.CodingParameterTy0 = 0;
            compressionParameters.TileSizeOn = true;
            compressionParameters.CodingParameterTdx = tileWidth;
            compressionParameters.CodingParameterTdy = tileHeight;
            compressionParameters.CodeBlockWidthInitial = codeBlockWidthInitial;
            compressionParameters.CodeBlockHeightInitial = codeBlockHeightInitial;
            compressionParameters.Irreversible = irreversible;

            var parameters = new ImageComponentParameters[numCompsMax];
            for (var index = 0; index < parameters.Length; index++)
            {
                parameters[index] = new ImageComponentParameters
                {
                    Dx = 1,
                    Dy = 1,
                    Height = imageHeight,
                    Width = imageWidth,
                    Signed = false,
                    Precision = compPrec,
                    X0 = offsetX,
                    Y0 = offsetY
                };
            }

            var data = new byte[imageWidth * imageHeight];
            for (var index = 0; index < data.Length; index++)
                data[index] = (byte)(index % byte.MaxValue);

            var image = OpenJpeg.ImageCreate(numComps, parameters, ColorSpace.Srgb);

            foreach (var parameter in parameters)
                this.DisposeAndCheckDisposedState(parameter);

            this.DisposeAndCheckDisposedState(image);
        }

        [Fact]
        public void ImageDataAlloc()
        {
            var mem = OpenJpeg.ImageDataAlloc(100);
            Assert.True(mem != IntPtr.Zero);
            OpenJpeg.ImageDataFree(mem);
        }

        [Fact]
        public void ImageDataFree()
        {
            OpenJpeg.ImageDataFree(IntPtr.Zero);
        }

        #endregion

        #region Not Native Functions

        /*
        [Fact]
        public void ToBitmapFromFile()
        {
            var targets = new[]
            {
                //new { Name = "Bretagne1_0.j2k", IsReadStream = true, Format = CodecFormat.Unknown,  Result = false },
                new { Name = "Bretagne1_0.j2k", IsReadStream = true, Format = CodecFormat.J2k,      Result = true  },
                //new { Name = "Bretagne1_0.j2k", IsReadStream = true, Format = CodecFormat.Jp2,      Result = false  },
                //new { Name = "Bretagne1_0.j2k", IsReadStream = true, Format = CodecFormat.Jpp,      Result = false },
                //new { Name = "Bretagne1_0.j2k", IsReadStream = true, Format = CodecFormat.Jpt,      Result = false },
                //new { Name = "Bretagne1_0.j2k", IsReadStream = true, Format = CodecFormat.Jpx,      Result = false }
            };

            foreach (var target in targets)
            {
                var path = Path.GetFullPath(Path.Combine(TestImageDirectory, target.Name));

                var stream = OpenJpeg.StreamCreateDefaultFileStream(path, target.IsReadStream);
                var codec = OpenJpeg.CreateDecompress(target.Format);
                var decompressionParameters = new DecompressionParameters();
                OpenJpeg.SetDefaultDecoderParameters(decompressionParameters);
                Assert.True(OpenJpeg.SetupDecoder(codec, decompressionParameters) == target.Result, $"Failed to invoke {nameof(OpenJpeg.SetupDecoder)} for {target.Format} and {target.IsReadStream}");
                Assert.True(OpenJpeg.ReadHeader(stream, codec, out var image) == target.Result, $"Failed to invoke {nameof(OpenJpeg.ReadHeader)} for {target.Format} and {target.IsReadStream}");
                Assert.True(OpenJpeg.SetDecodeArea(codec, image, 0, 0, 0, 0) == target.Result, $"Failed to invoke {nameof(OpenJpeg.SetDecodeArea)} for {target.Format} and {target.IsReadStream}");
                Assert.True(OpenJpeg.Decode(codec, stream, image) == target.Result, $"Failed to invoke {nameof(OpenJpeg.Decode)} for {target.Format} and {target.IsReadStream}");
                Assert.True(OpenJpeg.EndDecompress(codec, stream) == target.Result, $"Failed to invoke {nameof(OpenJpeg.EndDecompress)} for {target.Format} and {target.IsReadStream}");

                using (var bitmap = image.ToBitmap())
                {
                    var bitmapPath = Path.ChangeExtension(path, "bmp");
                    var directory = Path.Combine(ResultDirectory, nameof(this.ToBitmapFromFile), target.Format.ToString());
                    Directory.CreateDirectory(directory);
                    bitmapPath = Path.Combine(directory, Path.GetFileName(bitmapPath));

                    // Although SkiaSharp requires working with Bitmaps, it does not support encoding them...
                    // We'll have to make do with AnyBitmap support
                    
                    //using var encData = bitmap.Encode(SKEncodedImageFormat.Bmp, 100);
                    //using var fsHandle = File.OpenWrite(bitmapPath);
                    //encData.SaveTo(fsHandle);
                    AnyBitmap anybitmap = bitmap;
                    anybitmap.SaveAs(bitmapPath);
                }

                this.DisposeAndCheckDisposedState(image);
                this.DisposeAndCheckDisposedState(stream);
                this.DisposeAndCheckDisposedState(decompressionParameters);
                this.DisposeAndCheckDisposedState(codec);
            }
        }
        */

        /*
        [Fact]
        public void ToBitmapFromMemory()
        {
            var targets = new[]
            {
                //new { Name = "Bretagne1_0.j2k", IsReadStream = true, Format = CodecFormat.Unknown,  Result = false },
                new { Name = "Bretagne1_0.j2k", IsReadStream = true, Format = CodecFormat.J2k,      Result = true  },
                //new { Name = "Bretagne1_0.j2k", IsReadStream = true, Format = CodecFormat.Jp2,      Result = false  },
                //new { Name = "Bretagne1_0.j2k", IsReadStream = true, Format = CodecFormat.Jpp,      Result = false },
                //new { Name = "Bretagne1_0.j2k", IsReadStream = true, Format = CodecFormat.Jpt,      Result = false },
                //new { Name = "Bretagne1_0.j2k", IsReadStream = true, Format = CodecFormat.Jpx,      Result = false }
            };

            foreach (var target in targets)
            {
                var path = Path.GetFullPath(Path.Combine(TestImageDirectory, target.Name));
                var data = File.ReadAllBytes(path);

                var reader = new Reader(data);
                var result = reader.ReadHeader();
                Assert.True(result, $"Failed to invoke {typeof(Reader).FullName}.{nameof(IO.Reader.ReadHeader)} for {target.Format} and {target.IsReadStream}");

                using (var bitmap = reader.DecodeToBitmap())
                {
                    var bitmapPath = Path.ChangeExtension(path, "bmp");
                    var directory = Path.Combine(ResultDirectory, nameof(this.ToBitmapFromMemory), target.Format.ToString());
                    Directory.CreateDirectory(directory);
                    bitmapPath = Path.Combine(directory, Path.GetFileName(bitmapPath));

                    // Although SkiaSharp requires working with Bitmaps, it does not support encoding them...
                    // We'll have to make do with AnyBitmap support
                    
                    //using var encData = bitmap.Encode(SKEncodedImageFormat.Bmp, 100);
                    //using var fsHandle = File.OpenWrite(bitmapPath);
                    //encData.SaveTo(fsHandle);
                    AnyBitmap anybitmap = bitmap;
                    anybitmap.SaveAs(bitmapPath);
                }
            }
        }
        */

        #endregion

        #region Helpers

        private static Image CreateImage(uint numComps = 3)
        {
            const int numCompsMax = 4;
            const int codeBlockWidthInitial = 64;
            const int codeBlockHeightInitial = 64;
            const int imageWidth = 2000;
            const int imageHeight = 2000;
            const int tileWidth = 1000;
            const int tileHeight = 1000;
            const uint compPrec = 8;
            const bool irreversible = false;
            const uint offsetX = 0;
            const uint offsetY = 0;

            using var codec = OpenJpeg.CreateCompress(CodecFormat.Jp2);
            using var compressionParameters = new CompressionParameters();
            OpenJpeg.SetDefaultEncoderParameters(compressionParameters);

            compressionParameters.TcpNumLayers = 1;
            compressionParameters.CodingParameterFixedQuality = 1;
            compressionParameters.TcpDistoratio[0] = 20;
            compressionParameters.CodingParameterTx0 = 0;
            compressionParameters.CodingParameterTy0 = 0;
            compressionParameters.TileSizeOn = true;
            compressionParameters.CodingParameterTdx = tileWidth;
            compressionParameters.CodingParameterTdy = tileHeight;
            compressionParameters.CodeBlockWidthInitial = codeBlockWidthInitial;
            compressionParameters.CodeBlockHeightInitial = codeBlockHeightInitial;
            compressionParameters.Irreversible = irreversible;

            var parameters = new ImageComponentParameters[numCompsMax];
            for (var index = 0; index < parameters.Length; index++)
            {
                parameters[index] = new ImageComponentParameters
                {
                    Dx = 1,
                    Dy = 1,
                    Height = imageHeight,
                    Width = imageWidth,
                    Signed = false,
                    Precision = compPrec,
                    X0 = offsetX,
                    Y0 = offsetY
                };
            }

            var data = new byte[imageWidth * imageHeight];
            for (var index = 0; index < data.Length; index++)
                data[index] = (byte)(index % byte.MaxValue);

            var image = OpenJpeg.ImageTileCreate(numComps, parameters, ColorSpace.Srgb);

            foreach (var parameter in parameters)
                parameter.Dispose();

            return image;
        }
        
        private static Image DecodeImageFromFile(string path, CodecFormat format)
        {
            using var stream = OpenJpeg.StreamCreateDefaultFileStream(path, true);
            using var codec = OpenJpeg.CreateDecompress(format);
            using var decompressionParameters = new DecompressionParameters();
            OpenJpeg.SetDefaultDecoderParameters(decompressionParameters);
            OpenJpeg.SetupDecoder(codec, decompressionParameters);
            OpenJpeg.ReadHeader(stream, codec, out var image);
            OpenJpeg.SetDecodeArea(codec, image, 0, 0, 0, 0);
            OpenJpeg.Decode(codec, stream, image);
            OpenJpeg.EndDecompress(codec, stream);
            return image;
        }

        #endregion

    }

}
