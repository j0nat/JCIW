using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class ServiceLogList : Serializer
    {
        [ProtoMember(1)]
        public ServiceLogItem[] ServiceLogItems { get; set; }

        private ServiceLogList() { }

        public ServiceLogList(ServiceLogItem[] serviceLogList)
        {
            this.ServiceLogItems = serviceLogList;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, ServiceLogItems);
        }

        public void Deserialize(Stream inputStream)
        {
            ServiceLogItems = DeserializeServiceLogItemArray(inputStream);
        }

        public static void Deserialize(Stream inputStream, out ServiceLogList serviceLogList)
        {
            serviceLogList = new ServiceLogList();
            serviceLogList.Deserialize(inputStream);
        }
    }
}
