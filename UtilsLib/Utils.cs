using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UtilsLib
{
    public static class Utils
    {
        public static string FileNameForSure(string subDir,  string filename, bool isBoomNeeded, bool isSubDirAbsolute = false)
        {
            try
            {
                string folder = isSubDirAbsolute 
                    ? subDir 
                    : Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, subDir));
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fullPath = Path.GetFullPath(Path.Combine(folder, filename));
                if (File.Exists(fullPath))
                    return fullPath;
                using (FileStream fs = File.Create(fullPath))
                {
                    if (isBoomNeeded)
                    { fs.WriteByte(239); fs.WriteByte(187); fs.WriteByte(191); }
                }
                return fullPath;
            }
            catch (COMException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}