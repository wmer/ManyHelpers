using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyHelpers.Directories {
    public class DirectoryHelper {

        public string CreateDirectory(string folderName, string? path = null) {
            if (string.IsNullOrEmpty(path)) {
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            path = $"{path}\\{folderName}";

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            return path;
        }

    }
}
