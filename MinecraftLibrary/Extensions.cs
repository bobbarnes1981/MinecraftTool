using System;
using System.Collections;
using System.Collections.Generic;

namespace MinecraftLibrary
{
    public static class Extensions
    {
        public static TagType GetTagTypeFromType(this Type type)
        {
            if (type == typeof(object))
            {
                return TagType.End;
            }
            if (type == typeof(byte))
            {
                return TagType.Byte;
            }
            if (type == typeof(short))
            {
                return TagType.Short;
            }
            if (type == typeof(int))
            {
                return TagType.Int;
            }
            if (type == typeof(long))
            {
                return TagType.Long;
            }
            if (type == typeof(float))
            {
                return TagType.Float;
            }
            if (type == typeof(double))
            {
                return TagType.Double;
            }
            if (type == typeof(byte[]))
            {
                return TagType.ByteArray;
            }
            if (type == typeof(string))
            {
                return TagType.String;
            }
            if (type == typeof(List<Tag>)) // List<Tag> is a list of unnamed compound tags
            {
                return TagType.Compound;
            }
            if (type == typeof(IList))
            {
                return TagType.List;
            }

            throw new Exception(string.Format("Unhandled type: {0}", type.FullName));
        }

        public static Type GetTypeFromTagType(this TagType tagType)
        {
            switch (tagType)
            {
                case TagType.End:
                    return typeof(object);
                case TagType.Byte:
                    return typeof(byte);
                case TagType.Short:
                    return typeof(short);
                case TagType.Int:
                    return typeof(int);
                case TagType.Long:
                    return typeof(long);
                case TagType.Float:
                    return typeof(float);
                case TagType.Double:
                    return typeof(double);
                case TagType.ByteArray:
                    return typeof(byte[]);
                case TagType.String:
                    return typeof(string);
                case TagType.Compound:
                    return typeof(List<Tag>); // List<Tag> is a list of unnamed compound tags
                case TagType.List:
                    return typeof(IList);
            }

            throw new Exception(string.Format("Unhandled tag type: {0}", tagType));
        }
    }
}
