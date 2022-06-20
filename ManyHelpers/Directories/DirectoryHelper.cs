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

        public void LimparDiretorio(string path, DateTime? untilTheDate  = null) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            var dilesInPath = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Select(x => new FileInfo(x)).ToList();

            if (untilTheDate != null) {
                dilesInPath = dilesInPath.Where(x => x.CreationTime.Date < untilTheDate?.Date).ToList();
            }

            foreach (var file in dilesInPath) {
                try {
                    File.Delete(file.FullName);
                    Console.WriteLine($"{file.Name} Apagado...");
                } catch (Exception e) {
                    Console.WriteLine("");
                    Console.WriteLine($"Um erro aconteceu: {e.Message}");
                }
            }
        }

    }
}
