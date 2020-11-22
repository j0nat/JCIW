using System;
using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class GroupRequest : Serializer
    {
        [ProtoMember(1)]
        public Int64 groupId { get; private set; }

        [ProtoMember(2)]
        public Int64 accountId { get; private set; }

        private GroupRequest() { }

        public GroupRequest(Int64 groupId, Int64 accountId)
        {
            this.groupId = groupId;
            this.accountId = accountId;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, groupId);
            SerializeData(outputStream, accountId);
        }

        public void Deserialize(Stream inputStream)
        {
            groupId = DeserializeInt(inputStream);
            accountId = DeserializeInt(inputStream);
        }

        public static void Deserialize(Stream inputStream, out GroupRequest group)
        {
            group = new GroupRequest();
            group.Deserialize(inputStream);
        }
    }
}