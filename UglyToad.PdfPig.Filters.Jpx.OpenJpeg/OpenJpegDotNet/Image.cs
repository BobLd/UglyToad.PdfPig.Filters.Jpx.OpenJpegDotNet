using System.Runtime.InteropServices;

namespace OpenJpegDotNet
{
    /// <summary>
    /// Defines the image data and characteristics. This class cannot be inherited.
    /// </summary>
    public sealed class Image : OpenJpegObject
    {

        #region Constructors

        internal Image(IntPtr ptr)
        {
            this.NativePtr = ptr;
        }

        internal Image(RawImage rawImage)
        {
            switch (rawImage.Type)
            {
                case RawImage.ImgType.Targa:
                    using (CompressionParameters cp = new CompressionParameters())
                    {
                        NativeMethods.openjpeg_openjp2_opj_set_default_encoder_parameters(cp.NativePtr);
                        NativeMethods.openjpeg_openjp2_extensions_tgatoimage(rawImage.NativePtr,
                                                                             this.NativePtr,
                                                                             cp.NativePtr);
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the horizontal offset from the origin of the reference grid to the left side of the image area.
        /// </summary>
        /// <exception cref="ObjectDisposedException">This object is disposed.</exception>
        public uint X0
        {
            get
            {
                this.ThrowIfDisposed();
                return NativeMethods.openjpeg_openjp2_opj_image_t_get_x0(this.NativePtr);
            }
            set
            {
                this.ThrowIfDisposed();
                NativeMethods.openjpeg_openjp2_opj_image_t_set_x0(this.NativePtr, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the reference grid.
        /// </summary>
        /// <exception cref="ObjectDisposedException">This object is disposed.</exception>
        public uint X1
        {
            get
            {
                this.ThrowIfDisposed();
                return NativeMethods.openjpeg_openjp2_opj_image_t_get_x1(this.NativePtr);
            }
            set
            {
                this.ThrowIfDisposed();
                NativeMethods.openjpeg_openjp2_opj_image_t_set_x1(this.NativePtr, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical offset from the origin of the reference grid to the top side of the image area.
        /// </summary>
        /// <exception cref="ObjectDisposedException">This object is disposed.</exception>
        public uint Y0
        {
            get
            {
                this.ThrowIfDisposed();
                return NativeMethods.openjpeg_openjp2_opj_image_t_get_y0(this.NativePtr);
            }
            set
            {
                this.ThrowIfDisposed();
                NativeMethods.openjpeg_openjp2_opj_image_t_set_y0(this.NativePtr, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the reference grid.
        /// </summary>
        /// <exception cref="ObjectDisposedException">This object is disposed.</exception>
        public uint Y1
        {
            get
            {
                this.ThrowIfDisposed();
                return NativeMethods.openjpeg_openjp2_opj_image_t_get_y1(this.NativePtr);
            }
            set
            {
                this.ThrowIfDisposed();
                NativeMethods.openjpeg_openjp2_opj_image_t_set_y1(this.NativePtr, value);
            }
        }

        /// <summary>
        /// Gets the number of components in the image.
        /// </summary>
        /// <exception cref="ObjectDisposedException">This object is disposed.</exception>
        public uint NumberOfComponents
        {
            get
            {
                this.ThrowIfDisposed();
                return NativeMethods.openjpeg_openjp2_opj_image_t_get_numcomps(this.NativePtr);
            }
            //set
            //{
            //    this.ThrowIfDisposed();
            //    NativeMethods.openjpeg_openjp2_opj_image_t_set_numcomps(this.NativePtr, value);
            //}
        }

        /// <summary>
        /// Gets or sets the color space.
        /// </summary>
        /// <exception cref="ObjectDisposedException">This object is disposed.</exception>
        public ColorSpace ColorSpace
        {
            get
            {
                this.ThrowIfDisposed();
                return NativeMethods.openjpeg_openjp2_opj_image_t_get_color_space(this.NativePtr);
            }
            set
            {
                this.ThrowIfDisposed();
                NativeMethods.openjpeg_openjp2_opj_image_t_set_color_space(this.NativePtr, value);
            }
        }

        /// <summary>
        /// Gets the image components.
        /// </summary>
        /// <exception cref="ObjectDisposedException">This object is disposed.</exception>
        public ImageComponent[] Components
        {
            get
            {
                this.ThrowIfDisposed();

                var components = new List<ImageComponent>();
                for (var index = 0; ; index++)
                {
                    var ptr = NativeMethods.openjpeg_openjp2_opj_image_t_get_comps_by_index(this.NativePtr, (uint)index);
                    if (ptr == IntPtr.Zero)
                        break;

                    components.Add(new ImageComponent(ptr));
                }

                return components.ToArray();
            }
        }

        /// <summary>
        /// Gets the 'restricted' ICC profile.
        /// </summary>
        /// <exception cref="ObjectDisposedException">This object is disposed.</exception>
        public byte[] IccProfile
        {
            get
            {
                this.ThrowIfDisposed();
                var length = NativeMethods.openjpeg_openjp2_opj_image_t_get_icc_profile_len(this.NativePtr);
                var buffer = NativeMethods.openjpeg_openjp2_opj_image_t_get_icc_profile_buf(this.NativePtr);
                if (buffer == IntPtr.Zero || length == 0)
                    return null;

                var icc = new byte[length];
                Marshal.Copy(buffer, icc, 0, (int)length);

                return icc;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts this <see cref="Image"/> to a Bitmap <see cref="RawImage"/>.
        /// </summary>
        /// <param name="alpha">Export alpha channel</param>
        /// <returns>A <see cref="RawImage"/> that represents the converted <see cref="Image"/>.</returns>
        /// <exception cref="NotSupportedException">This object is not supported.</exception>
        public RawImage ToRawBitmap(bool alpha = true)
        {
            this.ThrowIfDisposed();

            var ret = NativeMethods.openjpeg_openjp2_extensions_imagetobmp(this.NativePtr,
                                                                           out var planes,
                                                                           out var width,
                                                                           out var height,
                                                                           out var channel,
                                                                           out var pixel);
            if (ret != NativeMethods.ErrorType.OK)
            {
                if (planes != IntPtr.Zero)
                    NativeMethods.stdlib_free(planes);

                throw new NotSupportedException("This object is not supported.");
            }

            var raw = new byte[width * height * channel];

            try
            {

                switch (pixel)
                {
                    case 8:
                        switch (channel)
                        {
                            case 1:
                                {
                                    unsafe
                                    {
                                        fixed (byte* dst = &raw[0])
                                        {
                                            var stride = (int)(width * channel);
                                            for (var y = 0; y < height; y++)
                                            {
                                                var src = IntPtr.Add(planes, (int)(y * width));
                                                var dest = IntPtr.Add((IntPtr)dst, y * stride);
                                                NativeMethods.cstd_memcpy(dest, src, (int)width);
                                            }
                                        }
                                    }
                                }
                                break;
                            case 3:
                                {
                                    unsafe
                                    {
                                        fixed (byte* dst = &raw[0])
                                        {
                                            var pSrc = (byte*)planes;
                                            var pDest = dst;
                                            var size = width * height;
                                            for (var y = 0; y < height; y++)
                                            {
                                                for (var x = 0; x < width; x++)
                                                {
                                                    pDest[2] = pSrc[0];
                                                    pDest[1] = pSrc[0 + size];
                                                    pDest[0] = pSrc[0 + size * 2];

                                                    pSrc += 1;
                                                    pDest += channel;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case 4:
                                {
                                    unsafe
                                    {
                                        fixed (byte* dst = &raw[0])
                                        {
                                            var pSrc = (byte*)planes;
                                            var pDest = dst;
                                            var size = width * height;
                                            for (var y = 0; y < height; y++)
                                            {
                                                for (var x = 0; x < width; x++)
                                                {
                                                    pDest[3] = alpha ? pSrc[0 + size * 3] : (byte)255;
                                                    pDest[2] = pSrc[0];
                                                    pDest[1] = pSrc[0 + size];
                                                    pDest[0] = pSrc[0 + size * 2];

                                                    pSrc += 1;
                                                    pDest += channel;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            default:
                                throw new NotSupportedException($"Unsupported number of channels: ${channel}.");
                        }

                        break;
                    default:
                        throw new NotSupportedException($"Unsupported pixel depth: ${pixel}.");
                }
            }
            finally
            {
                if (planes != IntPtr.Zero)
                    NativeMethods.stdlib_free(planes);
            }

            return new RawImage(raw, (int) width, (int) height, (int) channel, RawImage.ImgType.Bitmap);
        }

        /// <summary>
        /// Converts this <see cref="Image"/> to a TARGA <see cref="RawImage"/>.
        /// </summary>
        /// <returns>A <see cref="RawImage"/> that represents the converted <see cref="Image"/>.</returns>
        /// <exception cref="NotSupportedException">This object is not supported.</exception>
        public RawImage ToTarga()
        {
            this.ThrowIfDisposed();

            var ret = NativeMethods.openjpeg_openjp2_extensions_imagetotga(this.NativePtr,
                                                                           out var tga,
                                                                           out var tga_size,
                                                                           out var width,
                                                                           out var height,
                                                                           out var channel);
            if (ret != NativeMethods.ErrorType.OK)
            {
                if (tga != IntPtr.Zero) { NativeMethods.stdlib_free(tga); }

                throw new NotSupportedException("This object is not supported.");
            }

            var raw = new byte[tga_size];
            try
            {
                unsafe
                {
                    fixed (byte* dst = &raw[0])
                    {
                        NativeMethods.cstd_memcpy((IntPtr)dst, tga, (int)tga_size);
                    }
                }
            }
            finally
            {
                if (tga != IntPtr.Zero) { NativeMethods.stdlib_free(tga); }
            }
            return new RawImage(raw, (int)width, (int)height, (int)channel, RawImage.ImgType.Targa);
        }

        #endregion

        #region Overrides 

        /// <summary>
        /// Releases all unmanaged resources.
        /// </summary>
        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();

            if (this.NativePtr == IntPtr.Zero)
                return;

            NativeMethods.openjpeg_openjp2_opj_image_t_destroy(this.NativePtr);
        }

        #endregion
    }
}