using ProtoBuf;
using System.IO;

namespace Networking.Data.Packets
{
    /// <summary>
    /// This is a protobuf packet.
    /// </summary>
    [ProtoContract]
    public class ModuleList : Serializer
    {
        [ProtoMember(1)]
        public ModuleInfo[] ModuleInfoList { get; set; }

        private ModuleList() { }

        public ModuleList(ModuleInfo[] moduleInfoList)
        {
            this.ModuleInfoList = moduleInfoList;
        }

        public void Serialize(Stream outputStream)
        {
            SerializeData(outputStream, ModuleInfoList);
        }

        public void Deserialize(Stream inputStream)
        {
            ModuleInfoList = DeserializeModuleInfoArray(inputStream);
        }

        public static void Deserialize(Stream inputStream, out ModuleList moduleList)
        {
            moduleList = new ModuleList();
            moduleList.Deserialize(inputStream);
        }
    }
}
