﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

// ReSharper disable once CheckNamespace
namespace OpenJpegDotNet.Tests
{

    public sealed partial class OpenJpegTest
    {

        #region Functions

        [Fact]
        public void StreamCreate()
        {
            var targets = new[]
            {
                new { IsReadStream = true },
                new { IsReadStream = false }
            };

            foreach (var target in targets)
            {
                var stream = OpenJpeg.StreamCreate(1024, target.IsReadStream);
                this.DisposeAndCheckDisposedState(stream);
            }
        }

        [Fact]
        public void StreamDefaultCreate()
        {
            var targets = new[]
            {
                new { IsReadStream = true },
                new { IsReadStream = false }
            };

            foreach (var target in targets)
            {
                var stream = OpenJpeg.StreamDefaultCreate(target.IsReadStream);
                this.DisposeAndCheckDisposedState(stream);
            }
        }

        [Fact]
        public void StreamSetReadFunction()
        {
            var targets = new[]
            {
                new { Name = "Bretagne1_0.j2k", IsReadStream = true },
                new { Name = "Bretagne1_0.j2k", IsReadStream = false }
            };

            foreach (var target in targets)
            {
                var path = Path.Combine(TestImageDirectory, target.Name);
                var data = File.ReadAllBytes(path);

                var userData = Marshal.AllocCoTaskMem(data.Length);
                Marshal.Copy(data, 0, userData, data.Length);

                var stream = OpenJpeg.StreamDefaultCreate(target.IsReadStream);
                var callback = new DelegateHandler<StreamRead>(StreamReadCallback);
                OpenJpeg.StreamSetReadFunction(stream, callback);
                this.DisposeAndCheckDisposedState(stream);

                Marshal.FreeCoTaskMem(userData);
            }
        }

        [Fact]
        public void StreamSetWriteFunction()
        {
            var targets = new[]
            {
                new { Name = "Bretagne1_0.j2k", IsReadStream = true },
                new { Name = "Bretagne1_0.j2k", IsReadStream = false }
            };

            foreach (var target in targets)
            {
                var path = Path.Combine(TestImageDirectory, target.Name);
                var data = File.ReadAllBytes(path);

                var userData = Marshal.AllocCoTaskMem(data.Length);
                Marshal.Copy(data, 0, userData, data.Length);

                var stream = OpenJpeg.StreamDefaultCreate(target.IsReadStream);
                var callback = new DelegateHandler<StreamWrite>(StreamWriteCallback);
                OpenJpeg.StreamSetWriteFunction(stream, callback);
                this.DisposeAndCheckDisposedState(stream);

                Marshal.FreeCoTaskMem(userData);
            }
        }

        [Fact]
        public void StreamSetSeekFunction()
        {
            var targets = new[]
            {
                new { Name = "Bretagne1_0.j2k", IsReadStream = true },
                new { Name = "Bretagne1_0.j2k", IsReadStream = false }
            };

            foreach (var target in targets)
            {
                var path = Path.Combine(TestImageDirectory, target.Name);
                var data = File.ReadAllBytes(path);

                var userData = Marshal.AllocCoTaskMem(data.Length);
                Marshal.Copy(data, 0, userData, data.Length);

                var stream = OpenJpeg.StreamDefaultCreate(target.IsReadStream);
                var callback = new DelegateHandler<StreamSeek>(StreamSeekCallback);
                OpenJpeg.StreamSetSeekFunction(stream, callback);
                this.DisposeAndCheckDisposedState(stream);

                Marshal.FreeCoTaskMem(userData);
            }
        }

        [Fact]
        public void StreamSetSkipFunction()
        {
            var targets = new[]
            {
                new { Name = "Bretagne1_0.j2k", IsReadStream = true },
                new { Name = "Bretagne1_0.j2k", IsReadStream = false }
            };

            foreach (var target in targets)
            {
                var path = Path.Combine(TestImageDirectory, target.Name);
                var data = File.ReadAllBytes(path);

                var userData = Marshal.AllocCoTaskMem(data.Length);
                Marshal.Copy(data, 0, userData, data.Length);

                var stream = OpenJpeg.StreamDefaultCreate(target.IsReadStream);
                var callback = new DelegateHandler<StreamSkip>(StreamSkipCallback);
                OpenJpeg.StreamSetSkipFunction(stream, callback);
                this.DisposeAndCheckDisposedState(stream);

                Marshal.FreeCoTaskMem(userData);
            }
        }

        [Fact]
        public void StreamSetUserData()
        {
            var targets = new[]
            {
                new { Name = "Bretagne1_0.j2k", IsReadStream = true },
                new { Name = "Bretagne1_0.j2k", IsReadStream = false }
            };

            foreach (var target in targets)
            {
                var path = Path.Combine(TestImageDirectory, target.Name);
                var data = File.ReadAllBytes(path);

                var userData = Marshal.AllocCoTaskMem(data.Length);
                Marshal.Copy(data, 0, userData, data.Length);

                var stream = OpenJpeg.StreamDefaultCreate(target.IsReadStream);
                OpenJpeg.StreamSetUserData(stream, userData);
                this.DisposeAndCheckDisposedState(stream);

                Marshal.FreeCoTaskMem(userData);
            }
        }

        [Fact]
        public void StreamSetUserDataLength()
        {
            var targets = new[]
            {
                new { IsReadStream = true },
                new { IsReadStream = false }
            };

            foreach (var target in targets)
            {
                var stream = OpenJpeg.StreamDefaultCreate(target.IsReadStream);
                OpenJpeg.StreamSetUserDataLength(stream, 1024);
                this.DisposeAndCheckDisposedState(stream);
            }
        }

        [Fact]
        public void StreamCreateFileStream()
        {
            var targets = new[]
            {
                new { Name = "Bretagne1_0.j2k", IsReadStream = true },
                //new { Name = "Bretagne1_0.j2k", IsReadStream = false }
            };

            foreach (var target in targets)
            {
                var path = Path.Combine(TestImageDirectory, target.Name);
                var stream = OpenJpeg.StreamCreateFileStream(path, 1024, target.IsReadStream);
                this.DisposeAndCheckDisposedState(stream);
            }
        }

        [Fact]
        public void StreamCreateDefaultFileStream()
        {
            var targets = new[]
            {
                new { Name = "Bretagne1_0.j2k", IsReadStream = true },
                //new { Name = "Bretagne1_0.j2k", IsReadStream = false }
            };

            foreach (var target in targets)
            {
                var path = Path.Combine(TestImageDirectory, target.Name);
                var stream = OpenJpeg.StreamCreateDefaultFileStream(path, target.IsReadStream);
                this.DisposeAndCheckDisposedState(stream);
            }
        }

        #endregion

        #region Helpers
        
        private static ulong StreamReadCallback(IntPtr buffer, ulong bytes, IntPtr userData)
        {
            unsafe
            {
                var buf = (Buffer*)userData;
                if (buf == null || buf->Data == IntPtr.Zero || buf->Length == 0)
                    return unchecked((ulong)-1);

                if (buf->Position >= buf->Length)
                    return unchecked((ulong)-1);

                var bufLength = (ulong)(buf->Length - buf->Position);
                var readLength = bytes < bufLength ? bytes : bufLength;

                System.Buffer.MemoryCopy((void*)IntPtr.Add(buf->Data, buf->Position), (void*)buffer, readLength, readLength);
                buf->Position += (int)readLength;

                return readLength;
            }
        }

        private static int StreamSeekCallback(ulong bytes, IntPtr userData)
        {
            unsafe
            {
                var buf = (Buffer*)userData;
                if (buf == null || buf->Data == IntPtr.Zero || buf->Length == 0)
                    return 0;

                buf->Position = (int)Math.Min(bytes, (ulong)buf->Length);

                return 1;
            }
        }

        private static long StreamSkipCallback(ulong bytes, IntPtr userData)
        {
            unsafe
            {
                var buf = (Buffer*)userData;
                if (buf == null || buf->Data == IntPtr.Zero || buf->Length == 0)
                    return -1;

                buf->Position = (int)Math.Min((ulong)buf->Position + bytes, (ulong)buf->Length);

                return (long)bytes;
            }
        }

        private static ulong StreamWriteCallback(IntPtr buffer, ulong bytes, IntPtr userData)
        {
            unsafe
            {
                var buf = (Buffer*)userData;
                if (buf == null || buf->Data == IntPtr.Zero || buf->Length == 0)
                    return unchecked((ulong)-1);

                if (buf->Position >= buf->Length)
                    return unchecked((ulong)-1);

                var bufLength = (ulong)(buf->Length - buf->Position);
                var writeLength = bytes < bufLength ? bytes : bufLength;

                System.Buffer.MemoryCopy((void*)buffer, (void*)IntPtr.Add(buf->Data, buf->Position), writeLength, writeLength);
                buf->Position += (int)writeLength;

                return (ulong)writeLength;
            }
        }

        #endregion

        [StructLayout(LayoutKind.Sequential)]
        internal struct Buffer
        {

            public IntPtr Data;

            public int Length;

            public int Position;

        }

    }

}
