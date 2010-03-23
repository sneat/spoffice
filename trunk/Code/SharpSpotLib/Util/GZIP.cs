using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using System.IO;

namespace SharpSpotLib.Util
{
    //FIXME: Use buffer first, and then 1 byte reads to speed things up
    internal static class GZIP
    {
        private const Int32 BUFFER_SIZE = 4096;

        public static Byte[] Inflate(Byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            MemoryStream output = new MemoryStream();
            GZipInputStream gzip = new GZipInputStream(ms, BUFFER_SIZE);
            Byte[] buffer = new Byte[BUFFER_SIZE];
            try
            {
                while (gzip.Available != 0)
                {
                    Int32 bytesRead = gzip.Read(buffer, 0, buffer.Length);
                    output.Write(buffer, 0, bytesRead);
                }
            }
            catch (Exception)
            {
            }

            try
            {
                while (gzip.Available != 0)
                {
                    // 1 byte reads ???
                    Int32 b = gzip.ReadByte();
                    if (b >= 0)
                        output.WriteByte((Byte)b);
                }
            }
            catch (Exception)
            {
            }

            return output.ToArray();
        }
    }
}
