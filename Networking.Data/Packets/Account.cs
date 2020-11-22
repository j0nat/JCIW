using System;
using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class Account : Serializer
    {
        [ProtoMember(1)]
        public Int64 id { get; private set; }

        [ProtoMember(2)]
        public string username { get; private set; }

        [ProtoMember(3)]
        public string firstname { get; private set; }

        [ProtoMember(4)]
        public string lastname { get; private set; }


        private Account() { }

        public Account(Int64 id, string username, string firstname, string lastname)
        {
            this.id = id;
            this.username = username;
            this.firstname = firstname;
            this.lastname = lastname;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, id);
            SerializeData(outputStream, username);
            SerializeData(outputStream, firstname);
            SerializeData(outputStream, lastname);
        }

        public void Deserialize(Stream inputStream)
        {
            id = DeserializeInt(inputStream);
            username = DeserializeString(inputStream);
            firstname = DeserializeString(inputStream);
            lastname = DeserializeString(inputStream);
        }

        public static void Deserialize(Stream inputStream, out Account account)
        {
            account = new Account();
            account.Deserialize(inputStream);
        }
    }
}