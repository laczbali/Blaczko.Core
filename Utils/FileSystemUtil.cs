namespace Blaczko.Core.Utils
{
    public class FileSystemUtil
    {
        public string EnsureDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }
}