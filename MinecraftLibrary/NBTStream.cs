using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace MinecraftLibrary
{
    /// <summary>
    /// http://web.archive.org/web/20110723210920/http://www.minecraft.net/docs/NBT.txt
    /// </summary>
    public class NBTStream : BaseStream
    {
        public event ProgressChangedEventHandler ProgressChanged;

        public NBTStream(Stream stream)
            : base(stream)
        {
        }

        public Tag Read()
        {
            return read_Tag();
        }

        public void Write(Tag tag)
        {
            write_Tag(tag);
        }

        private Tag read_Tag()
        {
            TagType tag = read_TagType();
            string name = string.Empty;
            // end tags do not have a name
            if (tag != TagType.End)
            {
                name = read_String();
            }
            object payload = read_Payload(tag);
            return new Tag(tag, name, payload);
        }

        private void write_Tag(Tag tag)
        {
            write_TagType(tag.Type);
            if (tag.Type != TagType.End)
            {
                write_String(tag.Name);
            }
            write_Payload(tag.Type, tag.Payload);
        }

        /// <summary>
        /// Reads the payload.
        /// </summary>
        /// <returns>The payload.</returns>
        /// <param name="type">Type.</param>
        private object read_Payload(TagType type)
        {
            switch (type)
            {
                case TagType.End:
                    //read_End();
                    return null;
                case TagType.Byte:
                    return read_Byte();
                case TagType.Short:
                    return read_Short();
                case TagType.Int:
                    return read_Int();
                case TagType.Long:
                    return read_Long();
                case TagType.Float:
                    return read_Float();
                case TagType.Double:
                    return read_Double();
                case TagType.ByteArray:
                    return read_ByteArray();
                case TagType.String:
                    return read_String();
                case TagType.List:
                    return read_List();
                case TagType.Compound:
                    return read_Compound();
                case TagType.IntArray:
                    return read_IntArray();
                default:
                    throw new Exception(string.Format("Unhandled tag: 0x{0:x2} {1}", (byte)type, type));
            }
        }

        private void write_Payload(TagType type, object payload)
        {
            switch (type)
            {
                case TagType.End:
                    //write_End();
                    break;
                case TagType.Byte:
                    write_Byte((byte)payload);
                    break;
                case TagType.Short:
                    write_Short((short)payload);
                    break;
                case TagType.Int:
                    write_Int((int)payload);
                    break;
                case TagType.Long:
                    write_Long((long)payload);
                    break;
                case TagType.Float:
                    write_Float((float)payload);
                    break;
                case TagType.Double:
                    write_Double((double)payload);
                    break;
                case TagType.ByteArray:
                    write_ByteArray((byte[])payload);
                    break;
                case TagType.String:
                    write_String((string)payload);
                    break;
                case TagType.List:
                    write_List((System.Collections.IList)payload);
                    break;
                case TagType.Compound:
                    write_Compound((List<Tag>)payload);
                    break;
                case TagType.IntArray:
                    write_IntArray((int[])payload);
                    break;
                default:
                    throw new Exception(string.Format("Unhandled tag: 0x{0:x2} {1}", (byte)type, type));
            }
        }

        private TagType read_TagType()
        {
            int tag = read_Byte();
            if (tag < 0 || tag > 11)
            {
                throw new Exception(string.Format("Invalid tag: 0x{0:x2}", tag));
            }
            return (TagType)tag;
        }

        private void write_TagType(TagType type)
        {
            write_Byte((byte)type);
        }

        private byte[] read_ByteArray()
        {
            int length = read_Int();
            byte[] output = new byte[length];
            for (int i = 0; i < length; i++)
            {
                output[i] = read_Byte();
            }
            return output;
        }

        private void write_ByteArray(byte[] input)
        {
            write_Int(input.Length);
            foreach (byte item in input)
            {
                write_Byte(item);
            }
        }

        private IList read_List()
        {
            TagType tag = read_TagType();
            Type t = tag.GetTypeFromTagType();
            IList output = null;
            int length = read_Int();
            //if (t == typeof(IList))
            //{
            //    output = new List<IList>();
            //    for (int i = 0; i < length; i++)
            //    {
            //        output.Add(read_List());
            //    }
            //}
            //else
            {
                Type listType = typeof(List<>).MakeGenericType(t);
                ConstructorInfo constructor = listType.GetConstructor(Type.EmptyTypes);
                output = (IList)constructor.Invoke(null);
                for (int i = 0; i < length; i++)
                {
                    output.Add(read_Payload(tag));
                }
            }
            return output;
        }

        private void write_List(IList input)
        {
            TagType type = input.GetType().GetGenericArguments()[0].GetTagTypeFromType();
            write_TagType(type);
            write_Int(input.Count);
            foreach (object payload in input)
            {
                write_Payload(type, payload);
            }
        }

        private List<Tag> read_Compound()
        {
            List<Tag> output = new List<Tag>();
            Tag t;
            do
            {
                t = read_Tag();
                output.Add(t);
            } while (t.Type != TagType.End);
            return output;
        }

        private void write_Compound(List<Tag> input)
        {
            foreach (Tag tag in input)
            {
                write_Tag(tag);
            }
        }

        private int[] read_IntArray()
        {
            int length = read_Int();
            int[] output = new int[length];
            for (int i = 0; i < length; i++)
            {
                output[i] = read_Int();
            }
            return output;
        }

        private void write_IntArray(int[] input)
        {
            write_Int(input.Length);
            foreach (int item in input)
            {
                write_Int(item);
            }
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
