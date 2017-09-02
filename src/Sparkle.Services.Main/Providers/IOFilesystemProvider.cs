
namespace Sparkle.Services.Main.Providers
{
    using Sparkle.Services.Main.Internal;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Default implementation of <see cref="IFilesystemProvider"/> that uses the standard filesystem access.
    /// </summary>
    public class IOFilesystemProvider : IFilesystemProvider
    {
        public string EnsureFilePath(params string[] parts)
        {
            string path = parts[0];
            for (int i = 0; i < parts.Length; i++)
            {
                if (i == 0)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                else if (i < (parts.Length - 1))
                {
                    path = Path.Combine(path, parts[i]);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                else
                {
                    path = Path.Combine(path, parts[i]);
                }
            }

            return path;
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public void WriteNewFile(string path, Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            using (var fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.CopyTo(fileStream);
            }
        }

        public void WriteFile(string path, Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                stream.CopyTo(fileStream);
            }
        }
    }
}
