using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class ServiceLogRequest : Serializer
    {
        [ProtoMember(1)]
        public long id { get; private set; }

        [ProtoMember(2)]
        public long dateLimit { get; private set; }

        private ServiceLogRequest() { }

        public ServiceLogRequest(long id, long dateLimit)
        {
            this.id = id;
            this.dateLimit = dateLimit;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, id);
            SerializeData(outputStream, dateLimit);
        }

        public void Deserialize(Stream inputStream)
        {
            id = DeserializeInt(inputStream);
            dateLimit = DeserializeInt(inputStream);
        }

        public static void Deserialize(Stream inputStream, out ServiceLogRequest serviceLogRequest)
        {
            serviceLogRequest = new ServiceLogRequest();
            serviceLogRequest.Deserialize(inputStream);
        }
    }
}