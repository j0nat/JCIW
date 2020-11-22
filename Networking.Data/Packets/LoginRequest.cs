using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class LoginRequest : Serializer
    {
        [ProtoMember(1)]
        public string username { get; private set; }

        [ProtoMember(2)]
        public string password { get; private set; }

        [ProtoMember(3)]
        public bool admin { get; private set; }

        private LoginRequest() { }

        public LoginRequest(string username, string password, bool admin)
        {
            this.username = username;
            this.password = password;
            this.admin = admin;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, username);
            SerializeData(outputStream, password);
            SerializeData(outputStream, admin);
        }

        public void Deserialize(Stream inputStream)
        {
            username = DeserializeString(inputStream);
            password = DeserializeString(inputStream);
            admin = DeserializeBool(inputStream);
        }

        public static void Deserialize(Stream inputStream, out LoginRequest account)
        {
            account = new LoginRequest();
            account.Deserialize(inputStream);
        }
    }
}