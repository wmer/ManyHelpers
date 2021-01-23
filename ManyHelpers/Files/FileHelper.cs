using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ManyHelpers.Files {
    public class FileHelper {
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
    }
}
