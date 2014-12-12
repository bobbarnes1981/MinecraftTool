using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;

namespace MinecraftLibrary
{
    /// <summary>
    /// http://minecraft.gamepedia.com/Region_file_format
    /// http://minecraft.gamepedia.com/Anvil_file_format
    /// </summary>
    public class AnvilStream : BaseStream
    {
        public event ProgressChangedEventHandler ProgressChanged;

        private int m_numberOfChunks = 1024;

        public AnvilStream(Stream stream)
            : base(stream)
        {
        }

        public Region Read()
        {
            Region region = new Region(m_numberOfChunks);
            for (int i = 0; i < m_numberOfChunks; i++)
            {
                region.ChunkLocations[i] = read_ChunkLocation();
            }
            for (int i = 0; i < m_numberOfChunks; i++)
            {
                region.TimeStamps[i] = read_TimeStamp();
            }
            for (int i = 0; i < m_numberOfChunks; i++)
            {
                ChunkLocation chunkLocation = region.ChunkLocations[i];
                if (chunkLocation.Offset != 0)
                {
                    seek((1024 * 4) * chunkLocation.Offset);
                    region.Chunks[i] = read_Chunk();
                    reportProgress((int)((100 / 1024f) * i));
                }
            }
            return region;
        }

        public void Write(Region region)
        {
            for (int i = 0; i < m_numberOfChunks; i++)
            {
                write_ChunkLocation(region.ChunkLocations[i]);
            }
            for (int i = 0; i < m_numberOfChunks; i++)
            {
                write_TimeStamp(region.TimeStamps[i]);
            }
            // todo: fix this - currently just rewrites object data
            for (int i = 0; i < m_numberOfChunks; i++)
            {
                // todo: keep track of number written and store chunk location (offset/sectors)
                ChunkLocation chunkLocation = region.ChunkLocations[i];
                // todo: seek to next available location
                seek((1024 * 4) * chunkLocation.Offset);
                if (region.Chunks[i] != null)
                {
                    write_Chunk(region.Chunks[i]);
                }
                reportProgress((int)((100 / 1024f) * i));
            }
        }

        private ChunkLocation read_ChunkLocation()
        {
            int data = read_Int();
            int offset = data >> 8;
            byte sectors = (byte)(data & 0xFF);
            return new ChunkLocation(offset, sectors);
        }

        private void write_ChunkLocation(ChunkLocation chunkLocation)
        {
            int data = chunkLocation.Offset << 8;
            data |= chunkLocation.Sectors;
            write_Int(data);
        }

        private int read_TimeStamp()
        {
            return read_Int();
        }

        private void write_TimeStamp(int timeStamp)
        {
            write_Int(timeStamp);
        }

        private Chunk read_Chunk()
        {
            int length = read_Int();
            byte compressionSchemeByte = read_Byte();
            if (compressionSchemeByte < 1 || compressionSchemeByte > 2)
            {
                throw new Exception(string.Format("Unrecognised compression scheme byte: 0x{0:x2}",
                    compressionSchemeByte));
            }
            CompressionScheme compressionScheme = (CompressionScheme)compressionSchemeByte;
            byte[] bytes = read(length - 1);
            MemoryStream stream = new MemoryStream(bytes);
            Stream source;
            switch (compressionScheme)
            {
                case CompressionScheme.GZip:
                    source = new GZipStream(stream, CompressionMode.Decompress);
                    break;
                case CompressionScheme.Zlib:
                    source = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(stream);
                    break;
                default:
                    throw new Exception(string.Format("Unhandled compression scheme: {0}", compressionScheme));
            }
            Tag data = new NBTStream(source).Read();

            return new Chunk(length, compressionScheme, data);
        }

        private void write_Chunk(Chunk chunk)
        {
            MemoryStream stream = new MemoryStream();
            Stream target;
            switch (chunk.CompressionScheme)
            {
                case CompressionScheme.GZip:
                    target = new GZipStream(stream, CompressionMode.Compress);
                    break;
                case CompressionScheme.Zlib:
                    target = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(stream);
                    break;
                default:
                    throw new Exception(string.Format("Unhandled compression scheme: {0}", chunk.CompressionScheme));
            }

            new NBTStream(target).Write(chunk.Data);

            switch (chunk.CompressionScheme)
            {
                case CompressionScheme.GZip:
                    //target = new GZipStream(stream, CompressionMode.Compress);
                    break;
                case CompressionScheme.Zlib:
                    ((ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream)target).Finish();
                    break;
                default:
                    throw new Exception(string.Format("Unhandled compression scheme: {0}", chunk.CompressionScheme));
            }

            write_Int((int)stream.Length + 1);
            write_Byte((byte)chunk.CompressionScheme);
            write(stream.ToArray());

            // todo: write chunkLocation information based on generated data?
        }

        private void reportProgress(int percent)
        {
            if (percent < 0 || percent > 100)
            {
                throw new Exception(string.Format("Invalid percent: {0}", percent));
            }

            if (ProgressChanged != null)
            {
                ProgressChanged(this, new ProgressChangedEventArgs(percent, null));
            }
        }
    }
}
