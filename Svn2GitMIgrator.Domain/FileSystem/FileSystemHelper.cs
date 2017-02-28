using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svn2GitMIgrator.Domain.FileSystem
{
    public class FileSystemHelper
    {
        
        public static DirectoryInfo EnsureFolderExists(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            
            return directoryInfo;
        }

        public static void ClearFolder(DirectoryInfo folder)
        {
            foreach (var file in folder.GetFiles())
            {
                file.Delete();
            }
            foreach (var directory in folder.GetDirectories())
            {
                directory.Delete(true);
            }
        }
    }
}
