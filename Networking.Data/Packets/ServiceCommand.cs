using System;
using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class ServiceCommand : Serializer
    {
        [ProtoMember(1)]
        public Int64 serviceId { get; private set; }

        [ProtoMember(2)]
        public string value { get; private set; }

        private ServiceCommand() { }

        public ServiceCommand(Int64 serviceId, string value)
        {
            this.serviceId = serviceId;
            this.value = value;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, serviceId);
            SerializeData(outputStream, value);
        }

        public void Deserialize(Stream inputStream)
        {
            serviceId = DeserializeInt(inputStream);
            value = DeserializeString(inputStream);
        }

        public static void Deserialize(Stream inputStream, out ServiceCommand serviceCommand)
        {
            serviceCommand = new ServiceCommand();
            serviceCommand.Deserialize(inputStream);
        }
    }
}