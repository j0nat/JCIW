using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class RegisterRequest : Serializer
    {
        [ProtoMember(1)]
        public string username { get; private set; }

        [ProtoMember(2)]
        public string password { get; private set; }

        [ProtoMember(3)]
        public string firstname { get; private set; }

        [ProtoMember(4)]
        public string lastname { get; private set; }


        private RegisterRequest() { }

        public RegisterRequest(string username, string password, string firstname, string lastname)
        {
            this.username = username;
            this.password = password;
            this.firstname = firstname;
            this.lastname = lastname;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, username);
            SerializeData(outputStream, password);
            SerializeData(outputStream, firstname);
            SerializeData(outputStream, lastname);
        }

        public void Deserialize(Stream inputStream)
        {
            username = DeserializeString(inputStream);
            password = DeserializeString(inputStream);
            firstname = DeserializeString(inputStream);
            lastname = DeserializeString(inputStream);
        }

        public static void Deserialize(Stream inputStream, out RegisterRequest account)
        {
            account = new RegisterRequest();
            account.Deserialize(inputStream);
        }
    }
}