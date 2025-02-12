using System.Runtime.InteropServices;

namespace OpenJpegDotNet
{
    public sealed class DelegateHandler<T>
    {
        #region Fields

        private readonly T _Delegate;

        #endregion

        #region Constructors

        public DelegateHandler(T @delegate)
        {
            this._Delegate = @delegate;
            this.Handle = Marshal.GetFunctionPointerForDelegate(this._Delegate);
        }

        #endregion

        #region Properties

        public IntPtr Handle
        {
            get;
        }

        #endregion
    }
}