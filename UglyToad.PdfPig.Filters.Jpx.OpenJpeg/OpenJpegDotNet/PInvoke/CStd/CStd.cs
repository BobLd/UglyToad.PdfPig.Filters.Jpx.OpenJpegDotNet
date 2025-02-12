﻿using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace OpenJpegDotNet
{
    internal sealed partial class NativeMethods
    {
        #region cstd

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern IntPtr cstd_memcpy(IntPtr dest, IntPtr src, int count);

        #endregion
    }
}