using System;
using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class ModuleInfo : Serializer
    {
        [ProtoMember(1)]
        public Int64 Id { get; set; }

        [ProtoMember(2)]
        public int Type { get; set; }

        [ProtoMember(3)]
        public string Name { get; set; }

        [ProtoMember(4)]
        public string Version { get; set; }

        [ProtoMember(5)]
        public string Path { get; set; }

        [ProtoMember(6)]
        public int Enabled { get; set; }

        private ModuleInfo() { }

        public ModuleInfo(long id, int type, string name, string version, string path, int enabled)
        {
            this.Id = id;
            this.Type = type;
            this.Name = name;
            this.Version = version;
            this.Path = path;
            this.Enabled = enabled;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, Id);
            SerializeData(outputStream, Type);
            SerializeData(outputStream, Name);
            SerializeData(outputStream, Version);
            SerializeData(outputStream, Path);
            SerializeData(outputStream, Enabled);
        }

        public void Deserialize(Stream inputStream)
        {
            Id = DeserializeInt(inputStream);
            Type = DeserializeInt(inputStream);
            Name = DeserializeString(inputStream);
            Version = DeserializeString(inputStream);
            Path = DeserializeString(inputStream);
            Enabled = DeserializeInt(inputStream);
        }

        public static void Deserialize(Stream inputStream, out ModuleInfo account)
        {
            account = new ModuleInfo();
            account.Deserialize(inputStream);
        }
    }
}
