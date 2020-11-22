using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class ServiceLogItem : Serializer
    {
        [ProtoMember(1)]
        public long id { get; private set; }

        [ProtoMember(2)]
        public long serviceId { get; private set; }

        [ProtoMember(3)]
        public string text { get; private set; }

        [ProtoMember(4)]
        public long date { get; private set; }

        private ServiceLogItem() { }

        public ServiceLogItem(long id, long serviceId, string text, long date)
        {
            this.id = id;
            this.serviceId = serviceId;
            this.text = text;
            this.date = date;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, id);
            SerializeData(outputStream, serviceId);
            SerializeData(outputStream, text);
            SerializeData(outputStream, date);
        }

        public void Deserialize(Stream inputStream)
        {
            id = DeserializeInt(inputStream);
            serviceId = DeserializeInt(inputStream);
            text = DeserializeString(inputStream);
            date = DeserializeInt(inputStream);
        }

        public static void Deserialize(Stream inputStream, out ServiceLogItem serviceLogItem)
        {
            serviceLogItem = new ServiceLogItem();
            serviceLogItem.Deserialize(inputStream);
        }
    }
}