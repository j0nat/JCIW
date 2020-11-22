using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class GroupList : Serializer
    {
        [ProtoMember(1)]
        public Group[] list { get; private set; }

        private GroupList() { }

        public GroupList(Group[] list)
        {
            this.list = list;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, list);
        }

        public void Deserialize(Stream inputStream)
        {
            list = DeserializeGroupArray(inputStream);
        }

        public static void Deserialize(Stream inputStream, out GroupList groupList)
        {
            groupList = new GroupList();
            groupList.Deserialize(inputStream);
        }
    }
}