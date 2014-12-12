using System;
using System.Collections;

namespace MinecraftLibrary
{
    public class Tag
    {
        public event EventHandler PayloadChanged;
        private object m_payload;
        public Tag(TagType type, string name, object payload)
        {
            Type = type;
            Name = name;
            m_payload = payload;
        }
        public TagType Type { get; private set; }
        public string Name { get; private set; }
        public object Payload { get { return m_payload; } }
        public bool IsCollection
        {
            get
            {
                return Type == TagType.ByteArray && Type == TagType.Compound && Type == TagType.IntArray && Type == TagType.List;
            }
        }
        public bool ParsePayload(string input)
        {
            try
            {
                switch (Type)
                {
                    case TagType.Byte:
                        m_payload = byte.Parse(input);
                        break;
                    case TagType.Double:
                        m_payload = double.Parse(input);
                        break;
                    case TagType.Float:
                        m_payload = float.Parse(input);
                        break;
                    case TagType.Int:
                        m_payload = int.Parse(input);
                        break;
                    case TagType.Long:
                        m_payload = long.Parse(input);
                        break;
                    case TagType.Short:
                        m_payload = short.Parse(input);
                        break;
                    case TagType.String:
                        m_payload = input;
                        break;
                    default:
                        throw new Exception(string.Format("Unhandled payload type: {0}", Type));
                }
                NotifyPayloadChanged();
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        private void NotifyPayloadChanged()
        {
            if (PayloadChanged != null)
            {
                PayloadChanged(this, new EventArgs());
            }
        }
        public override string ToString()
        {
            switch (Type)
            {
                case TagType.Compound:
                    return string.Format("{0} - {1}", Type, Name);
                case TagType.List:
                    return string.Format("{0}<{1}>[{2}] - {3}", Type, Payload.GetType().GetGenericArguments()[0].GetTagTypeFromType(), ((IList)Payload).Count, Name);
                case TagType.IntArray:
                    return string.Format("{0} {1}[{2}] - {3}", Type, Payload.GetType().GetElementType(), ((int[])Payload).Length, Name);
                case TagType.ByteArray:
                    return string.Format("{0} {1}[{2}] - {3}", Type, Payload.GetType().GetElementType(), ((byte[])Payload).Length, Name);
                default:
                    return string.Format("{0} - {1} - {2}", Type, Name, (Payload ?? ""));
            }
        }
    }
}
