using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ManyHelpers.Files {
    public class FileHelper {
        public static int MimeSampleSize = 256;

        public static string DefaultMimeType = "application/octet-stream";

        public static async Task<byte[]> GetByteArray(IFormFile formFile) {
            byte[] avatarFile = null;

            if (formFile != null && formFile.Length > 0) {
                var path = Path.Combine(Path.GetTempPath(), formFile.FileName);

                using (var stream = new FileStream(path, FileMode.Create)) {
                    await formFile.CopyToAsync(stream);
                }
                if (System.IO.File.Exists(path)) {
                    avatarFile = System.IO.File.ReadAllBytes(path);
                }
            }

            return avatarFile;
        }

        public static byte[] GetByteArray(string file) {
            byte[] avatarFile = null;

            if (File.Exists(file)) {
                avatarFile = File.ReadAllBytes(file);
            }

            return avatarFile;
        }

        public static string GetBase64String(string file) {
            var base64String = "";

            if (File.Exists(file)) {
                base64String = Convert.ToBase64String(GetByteArray(file)); 
            }

            return base64String;
        }

        public static string GetMimeFromFile(string file) {
            var base64String = "";

            if (File.Exists(file)) {
                base64String = GetMimeFromBytes(GetByteArray(file));
            }

            return base64String;
        }

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
    }
}
