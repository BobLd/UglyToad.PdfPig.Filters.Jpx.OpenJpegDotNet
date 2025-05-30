using System;
using System.Collections.Generic;
using Xunit;

namespace OpenJpegDotNet.Tests
{

    public abstract class TestBase
    {

        #region Methods

        internal void DisposeAndCheckDisposedState(OpenJpegObject obj)
        {
            if (obj == null)
                return;

            obj.Dispose();
            Assert.True(obj.IsDisposed);
            Assert.True(obj.NativePtr == IntPtr.Zero);
        }

        internal void DisposeAndCheckDisposedStates(IEnumerable<OpenJpegObject> objs)
        {
            foreach (var obj in objs)
                this.DisposeAndCheckDisposedState(obj);
        }

        #endregion

    }

}
