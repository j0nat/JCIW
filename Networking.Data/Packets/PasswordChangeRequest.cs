using System;
using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class PasswordChangeRequest : Serializer
    {
        [ProtoMember(1)]
        public Int64 accountId { get; private set; }

        [ProtoMember(2)]
        public string password { get; private set; }

        private PasswordChangeRequest() { }

        public PasswordChangeRequest(Int64 accountId, string password)
        {
            this.accountId = accountId;
            this.password = password;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, accountId);
            SerializeData(outputStream, password);
        }

        public void Deserialize(Stream inputStream)
        {
            accountId = DeserializeInt(inputStream);
            password = DeserializeString(inputStream);
        }

        public static void Deserialize(Stream inputStream, out PasswordChangeRequest passwordChangeRequest)
        {
            passwordChangeRequest = new PasswordChangeRequest();
            passwordChangeRequest.Deserialize(inputStream);
        }
    }
}