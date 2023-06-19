namespace Blaczko.Core.Utils
{
    public static class FileSystemUtil
    {
        public static string EnsureDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }
}