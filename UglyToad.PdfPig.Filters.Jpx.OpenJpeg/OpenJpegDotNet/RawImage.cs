using System.Collections.ObjectModel;

namespace OpenJpegDotNet
{

    /// <summary>
    /// Defines the raw bitmap image data. This class cannot be inherited.
    /// </summary>
    public sealed class RawImage : OpenJpegObject
    {
        public enum ImgType {
            Bitmap,
            Targa
        }

        #region Constructors

        internal RawImage(IList<byte> data, int width, int height, int channel, ImgType type)
        {
            this.Data = new ReadOnlyCollection<byte>(data);
            this.Width = width;
            this.Height = height;
            this.Channel = channel;
            this.Type = type;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of channels, of this <see cref="RawImage"/>.
        /// </summary>
        /// <returns>The number of channels, of this <see cref="RawImage"/>.</returns>
        public int Channel
        {
            get;
        }

        /// <summary>
        /// Gets the pixel data of this <see cref="RawImage"/>.
        /// </summary>
        /// <returns>The pixel data.</returns>
        public ReadOnlyCollection<byte> Data
        {
            get;
        }

        /// <summary>
        /// Gets the height, in pixels, of this <see cref="RawImage"/>.
        /// </summary>
        /// <returns>The height, in pixels, of this <see cref="RawImage"/>.</returns>
        public int Height
        {
            get;
        }

        /// <summary>
        /// Gets the width, in pixels, of this <see cref="RawImage"/>.
        /// </summary>
        /// <returns>The width, in pixels, of this <see cref="RawImage"/>.</returns>
        public int Width
        {
            get;
        }

        /// <summary>
        /// Gets the specified <see cref="RawImage.ImgType"/> for the object.
        /// </summary>
        /// <returns>The type of image storeed as <see cref="RawImage.ImgType"/></returns>
        public ImgType Type
        {
            get;
        }

        public byte[] Bytes
        {
            get
            {
                return Data.ToArray();
            }
        }

        #endregion

    }

}