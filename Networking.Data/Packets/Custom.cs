using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class Custom : Serializer
    {
        [ProtoMember(1)]
        public string name { get; private set; }

        [ProtoMember(2)]
        public string[] strings { get; private set; }

        [ProtoMember(3)]
        public double[] numbers { get; private set; }

        [ProtoMember(4)]
        public byte[][] byteArrays { get; private set; }

        private Custom() { }

        public Custom(string name, string[] strings, double[] numbers, byte[][] byteArrays)
        {
            this.name = name;
            this.strings = strings;
            this.numbers = numbers;
            this.byteArrays = byteArrays;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, name);
            SerializeData(outputStream, strings);
            SerializeData(outputStream, numbers);
            SerializeData(outputStream, byteArrays);
        }

        public void Deserialize(Stream inputStream)
        {
            name = DeserializeString(inputStream);
            strings = DeserializeStringArray(inputStream);
            numbers = DeserializeDoubleArray(inputStream);
            byteArrays = DeserializeByteArrayArray(inputStream);
        }

        public static void Deserialize(Stream inputStream, out Custom custom)
        {
            custom = new Custom();
            custom.Deserialize(inputStream);
        }
    }
}