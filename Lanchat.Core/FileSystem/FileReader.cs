using System;
using System.IO;
using System.Linq;

namespace Lanchat.Core.FileSystem
{
    internal class FileReader : IDisposable
    {
        private readonly string path;
        private byte[] buffer;
        private int bytesRead;
        private FileStream fileStream;

        internal FileReader(string path)
        {
            this.path = path;
        }

        public void Dispose()
        {
            fileStream?.Dispose();
        }

        public bool ReadChunk(int chunkSize, out byte[] chunk)
        {
            buffer ??= new byte[chunkSize];
            fileStream ??= File.OpenRead(path);
            bytesRead = fileStream.Read(buffer, 0, buffer.Length);

            if (bytesRead <= 0)
            {
                chunk = null;
                return false;
            }

            chunk = buffer.Take(bytesRead).ToArray();
            return true;
        }
    }
}