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

        public static string GetExtensionFromBytes(byte[] data) {
            try {
                var fileExtension = "";

                var mime = GetMimeFromBytes(data);
                if (mime == "audio/aac") {
                    fileExtension = ".aac";
                }
                if (mime == "video/x-msvideo") {
                    fileExtension = ".avi";
                }
                if (mime == "image/bmp") {
                    fileExtension = ".bmp";
                }
                if (mime == "text/csv") {
                    fileExtension = ".csv";
                }

                if (mime == "application/msword") {
                    fileExtension = ".doc";
                }
                if (mime == "application/vnd.openxmlformats-officedocument.wordprocessingml.document") {
                    fileExtension = ".docx";
                }
                if (mime == "image/vnd.microsoft.icon") {
                    fileExtension = ".ico";
                }

                if (mime == "audio/midi audio/x-midi") {
                    fileExtension = ".midi";
                }
                if (mime == "audio/mpeg") {
                    fileExtension = ".mp3";
                }
                if (mime == "video/mpeg") {
                    fileExtension = ".mpeg";
                }
                if (mime == "image/x-png" || mime == "image/png") {
                    fileExtension = ".png";
                }


                if (mime == "application/pdf") {
                    fileExtension = ".pdf";
                }
                if (mime == "application/vnd.ms-powerpoint") {
                    fileExtension = ".ppt";
                }
                if (mime == "application/vnd.openxmlformats-officedocument.presentationml.presentation") {
                    fileExtension = ".pptx";
                }
                if (mime == "image/svg+xml") {
                    fileExtension = ".svg";
                }
                if (mime == "audio/wav") {
                    fileExtension = ".wav";
                }
                if (mime == "audio/webm") {
                    fileExtension = ".weba";
                }
                if (mime == "video/webm") {
                    fileExtension = ".webm";
                }
                if (mime == "image/webp") {
                    fileExtension = ".webp";
                }
                if (mime == "application/vnd.ms-excel") {
                    fileExtension = ".xls";
                }
                if (mime == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
                    fileExtension = ".xlsx";
                }
                if (mime == "image/jpeg") {
                    fileExtension = ".jpeg";
                }

                return fileExtension;
            } catch {
                return null;
            }
        }
    }
}
