using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ManyHelpers.Media {
    public class MimeHelper {
        public static int MimeSampleSize = 256;

        public static string DefaultMimeType = "application/octet-stream";

        [DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
        static extern int FindMimeFromData(IntPtr pBC,
           [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
           [MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.I1, SizeParamIndex=3)]
            byte[] pBuffer,
           int cbSize,
           [MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
           int dwMimeFlags,
           out IntPtr ppwzMimeOut,
           int dwReserved);

        public static string GetMimeFromBytes(byte[] data) {
            try {
                IntPtr mimeType;
                FindMimeFromData(IntPtr.Zero, null, data, data.Length, null, 0, out mimeType, 0);

                var mimePointer = mimeType;
                var mime = Marshal.PtrToStringUni(mimePointer);
                Marshal.FreeCoTaskMem(mimePointer);

                return mime ?? DefaultMimeType;
            } catch {
                return DefaultMimeType;
            }
        }
    }
}
