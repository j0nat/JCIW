using System;
using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class Group : Serializer
    {
        [ProtoMember(1)]
        public Int64 id { get; private set; }

        [ProtoMember(2)]
        public string name { get; private set; }

        [ProtoMember(3)]
        public string description { get; private set; }

        private Group() { }

        public Group(Int64 id, string name, string description)
        {
            this.id = id;
            this.name = name;
            this.description = description;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, id);
            SerializeData(outputStream, name);
            SerializeData(outputStream, description);
        }

        public void Deserialize(Stream inputStream)
        {
            id = DeserializeInt(inputStream);
            name = DeserializeString(inputStream);
            description = DeserializeString(inputStream);
        }

        public static void Deserialize(Stream inputStream, out Group group)
        {
            group = new Group();
            group.Deserialize(inputStream);
        }
    }
}