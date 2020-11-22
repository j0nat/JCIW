using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class AccountList : Serializer
    {
        [ProtoMember(1)]
        public Account[] list { get; private set; }

        private AccountList() { }

        public AccountList(Account[] list)
        {
            this.list = list;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, list);
        }

        public void Deserialize(Stream inputStream)
        {
            list = DeserializeAccountArray(inputStream);
        }

        public static void Deserialize(Stream inputStream, out AccountList account)
        {
            account = new AccountList();
            account.Deserialize(inputStream);
        }
    }
}