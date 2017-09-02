
namespace Sparkle.Services.Main.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Filesystem abstraction. This will allow us to replace (one day) the filesystem storing (considered bad when running multiple webservers) by another kind of file store.
    /// </summary>
    public interface IFilesystemProvider
    {
        /// <summary>
        /// Creates the folders to create a valid path to a file.
        /// </summary>
        /// <param name="parts">The parts of the path, ending with a file name.</param>
        /// <returns>The concatenated file path.</returns>
        string EnsureFilePath(params string[] parts);

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns></returns>
        bool FileExists(string path);

        void WriteNewFile(string path, Stream stream);

        void WriteFile(string path, Stream stream);
    }
}
