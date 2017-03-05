using System;
using System.IO;

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

        public static string GetFilePath(string name)
        {
            var appDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            if (appDirectory.Parent != null)
            {
                return  Path.Combine(appDirectory.FullName, name);
            }

            return null;
        }
    }
}
