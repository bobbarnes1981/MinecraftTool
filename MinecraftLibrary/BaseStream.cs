using System;
using System.IO;
using System.Linq;

namespace MinecraftLibrary
{
    public abstract class BaseStream
    {
        private Stream m_stream;

        public BaseStream(Stream stream)
        {
            m_stream = stream;
        }

        protected byte[] read(int length)
        {
            byte[] bytes = new byte[length];
            m_stream.Read(bytes, 0, length);
            return bytes;
        }

        protected void write(byte[] bytes)
        {
            m_stream.Write(bytes, 0, bytes.Length);
        }

        protected void seek(long offset)
        {
            m_stream.Seek(offset, 0);
        }

        protected byte[] read_Value(int length)
        {
            byte[] buffer = new byte[length];
            m_stream.Read(buffer, 0, length);
            return buffer.Reverse().ToArray();
        }

        protected void write_Value(int length, byte[] input)
        {
            m_stream.Write(input.Reverse().ToArray(), 0, length);
        }

        protected byte read_Byte()
        {
            return read_Value(1)[0];
        }

        protected void write_Byte(byte input)
        {
            write_Value(1, new byte[] { input });
        }

        protected short read_Short()
        {
            return BitConverter.ToInt16(read_Value(2), 0);
        }

        protected void write_Short(short input)
        {
            write_Value(2, BitConverter.GetBytes(input));
        }

        protected int read_Int()
        {
            return BitConverter.ToInt32(read_Value(4), 0);
        }

        protected void write_Int(int input)
        {
            write_Value(4, BitConverter.GetBytes(input));
        }

        protected long read_Long()
        {
            return BitConverter.ToInt64(read_Value(8), 0);
        }

        protected void write_Long(long input)
        {
            write_Value(8, BitConverter.GetBytes(input));
        }

        protected float read_Float()
        {
            return BitConverter.ToSingle(read_Value(4), 0);
        }

        protected void write_Float(float input)
        {
            write_Value(4, BitConverter.GetBytes(input));
        }

        protected double read_Double()
        {
            return BitConverter.ToDouble(read_Value(8), 0);
        }

        protected void write_Double(double input)
        {
            write_Value(8, BitConverter.GetBytes(input));
        }

        protected string read_String()
        {
            short length = read_Short();
            string output = string.Empty;
            for (short i = 0; i < length; i++)
            {
                output += (char)m_stream.ReadByte();
            }
            return output;
        }

        protected void write_String(string input)
        {
            write_Short((short)input.Length);
            foreach (char c in input)
            {
                m_stream.WriteByte((byte)c);
            }
        }
    }
}
