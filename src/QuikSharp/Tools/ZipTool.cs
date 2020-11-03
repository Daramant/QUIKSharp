using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace QuikSharp.Tools
{
    public static class ZipTool
    {
        /// <summary>
        /// In-memory compress
        /// </summary>
        public static byte[] Zip(byte[] bytes)
        {
            using (var inStream = new MemoryStream(bytes))
            using (var outStream = new MemoryStream())
            {
                using (var compress = new GZipStream(outStream, CompressionMode.Compress))
                {
                    inStream.CopyTo(compress);
                }

                return outStream.ToArray();
            }
        }

        /// <summary>
        /// Uses UTF8 bytes to zip
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Zip(string value)
        {
            using (var inStream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
            using (var outStream = new MemoryStream())
            {
                using (var compress = new GZipStream(outStream, CompressionMode.Compress))
                {
                    inStream.CopyTo(compress);
                }

                return outStream.ToArray();
            }
        }

        /// <summary>
        /// In-memory uncompress
        /// </summary>
        public static byte[] UnzipBytes(byte[] bytes)
        {
            using (var inStream = new MemoryStream(bytes))
            using (var outStream = new MemoryStream())
            {
                using (var deCompress = new GZipStream(inStream, CompressionMode.Decompress))
                {
                    deCompress.CopyTo(outStream);
                }

                return outStream.ToArray();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string UnzipString(byte[] bytes)
        {
            using (var inStream = new MemoryStream(bytes))
            using (var outStream = new MemoryStream())
            {
                using (var deCompress = new GZipStream(inStream, CompressionMode.Decompress))
                {
                    deCompress.CopyTo(outStream);
                }

                return Encoding.UTF8.GetString(outStream.ToArray());
            }
        }
    }
}
