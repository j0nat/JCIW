using System;
using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class AppFileData : Serializer
    {
        [ProtoMember(1)]
        public Int64 id { get; private set; }

        [ProtoMember(2)]
        public string path { get; private set; }

        [ProtoMember(3)]
        public byte[] data { get; private set; }

        private AppFileData() { }

        public AppFileData(Int64 id, string path, byte[] data)
        {
            this.id = id;
            this.path = path;
            this.data = data;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, id);
            SerializeData(outputStream, path);
            SerializeData(outputStream, data);
        }

        public void Deserialize(Stream inputStream)
        {
            id = DeserializeInt(inputStream);
            path = DeserializeString(inputStream);
            data = DeserializeByteArray(inputStream);
        }

        public static void Deserialize(Stream inputStream, out AppFileData appFileData)
        {
            appFileData = new AppFileData();
            appFileData.Deserialize(inputStream);
        }
    }
}