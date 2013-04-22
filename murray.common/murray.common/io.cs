using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace murray.common
{
    /// <summary>
    /// Helper methods related to path and file I/O
    /// </summary>
    public static class io
    {
        /// <summary>
        /// Writes a string to a text file.
        /// Will throw exceptions
        /// </summary>
        /// <param name="pPath">file to be written</param>
        /// <param name="pText">text to be written</param>
        public static void WriteTextFile(string pPath, string pText)
        {
            FileStream fs = File.Open(pPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            var sw = new StreamWriter(fs);
            sw.Write(pText);
            sw.Close();
        }

        /// <summary>
        /// This method uses about 50% less RAM than the regular ReadTextFile...
        /// </summary>
        /// <param name="pPath">file to be read</param>
        /// <param name="pByteThresholdBeforeTriggeringGarbageCollection">If the file is larger than this number of bytes, manually trigger GC</param>
        /// <returns>a string containing the contents of the text file</returns>
        public static string ReadASCIITextFile(string pPath, int pByteThresholdBeforeTriggeringGarbageCollection = 1048576)
        {
            using (FileStream fs = File.Open(pPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] buffer = ReadFullStream(fs);
                string result = Encoding.ASCII.GetString(buffer);
                buffer = null;

                if (result.Length > pByteThresholdBeforeTriggeringGarbageCollection) // ReadFullStream creates a large temporary buffer and we don't want to wait too long to get rid of it.
                    GC.Collect();
                return result;
            }
        }

        /// <summary>
        /// Returns a byte array with the entire contents of the stream
        /// </summary>
        public static byte[] ReadFullStream(Stream pStream)
        {
            if (pStream == null)
                throw new ArgumentException("stream cannot be null", "pStream");

            if (pStream is MemoryStream)
                return ((MemoryStream)pStream).ToArray();

            using (var ms = new MemoryStream())
            {
                pStream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// There are different ways to get the "current" directory - this abstracts the various tests that need to be made in an exception-safe way.
        /// If no current directory can be determined, it returns the path to a temporary directory that can be written to.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDirectory()
        {
            string path = GetCurrentDirectoryFromAssembly(Assembly.GetExecutingAssembly());

            if (string.IsNullOrWhiteSpace(path))
                path = Path.GetTempPath(); //fallback

            return path.Replace("file:\\", string.Empty);
        }

        /// <summary>
        /// There are different ways to get the "current" directory - this abstracts the various tests that need to be made in an exception-safe way.
        /// If no current directory can be determined, it returns the path to a temporary directory that can be written to.
        /// Does NOT throw exceptions
        /// </summary>
        /// <returns>null of path cannot be determined by assembly. Returns raw path - no protocol filtering</returns>
        public static string GetCurrentDirectoryFromAssembly(Assembly pAssembly)
        {
            if (pAssembly == null)
                return null;

            string path = null;
            try
            {
                path = Path.GetDirectoryName(pAssembly.Location);
            }
            catch
            {
                try
                {
                    path = Path.GetDirectoryName(pAssembly.GetName().CodeBase);
                }
                catch { }
            }
            return path;
        }

        /// <summary>
        /// Ensures that the full path (all directories) to the given filename exists,
        /// so that the file can be created without any problems.
        /// This usually takes under 5ms, but can take up to 80ms or more.
        /// Will throw exceptions if the path is invalid, 
        /// or if security prevents the directory from being created.
        /// </summary>
        /// <param name="pPathAndFilename">path to create directories for. With or without filename</param>
        /// <param name="pUseCachedResultsMs">If > 0, then will use cached results for up to X ms (useful when doing a lot of file processing in a single folder). Using cache saves an avg of 1 second per 300 calls</param>
        public static void EnsurePathToFile(string pPathAndFilename, int pUseCachedResultsMs = 0)
        {
            //don't do any arg checking...let exceptions get thrown
            var dir = Path.GetDirectoryName(pPathAndFilename);

            if (pUseCachedResultsMs > 0 //we want to use cache
                && dir != null          //don't throw exceptions from a dictionary fail...wait and throw later with a more relevant directory exception
                && _EnsurePathToFileCache.ContainsKey(dir) //we have a cached result
                && DateTime.Now.Subtract(_EnsurePathToFileCache[dir]).TotalMilliseconds <= pUseCachedResultsMs //it was created within our acceptable cache length window
                )
            {
                return;
            }

            if (!Directory.Exists(dir))
                //this check saves over 50% of the overhead vs calling CreateDirectory() blindly
                Directory.CreateDirectory(dir);

            if (pUseCachedResultsMs > 0) //&& dir != null ... we may want to cache a null dir so that we don't tap the disk so much
                _EnsurePathToFileCache[dir] = DateTime.Now; //only save this if we're interested in using the cache...otherwise we'll fill up from calls that don't want to use cache
        }

        /// <summary>
        /// Key is path (no filename)
        /// Value is when path was last verified
        /// </summary>
        private static IDictionary<string, DateTime> _EnsurePathToFileCache = new ConcurrentDictionary<string, DateTime>();

    }
}
