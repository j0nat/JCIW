using System;
using System.Text;
using System.IO;
using System.Linq;
using Networking.Data.Packets;

namespace Networking.Data
{
    /// <summary>
    /// This class is used to serialize and deserialize streams.
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// Convert object to stream.
        /// </summary>
        /// <param name="outputStream">The <see cref="Stream"/>.</param>
        /// <param name="data">The <see cref="Object"/>.</param>
        public void SerializeData(Stream outputStream, Object data)
        {
            byte[] strData = Encoding.UTF8.GetBytes(data.ToString());
            byte[] strLengthData = BitConverter.GetBytes(strData.Length);

            outputStream.Write(strLengthData, 0, strLengthData.Length);
            outputStream.Write(strData, 0, strData.Length);
        }

        /// <summary>
        /// Convert stream to <see cref="string"/>.
        /// </summary>
        /// <param name="inputStream">The <see cref="Stream"/>.</param>
        /// <returns>Converted string</returns>
        public string DeserializeString(Stream inputStream)
        {
            byte[] strLengthData = new byte[sizeof(int)]; inputStream.Read(strLengthData, 0, sizeof(int));
            byte[] strData = new byte[BitConverter.ToInt32(strLengthData, 0)]; inputStream.Read(strData, 0, strData.Length);
            return new String(Encoding.UTF8.GetChars(strData));
        }

        /// <summary>
        /// Convert stream to <see cref="int"/>.
        /// </summary>
        /// <param name="inputStream">The <see cref="Stream"/>.</param>
        /// <returns>Converted int</returns>
        public int DeserializeInt(Stream inputStream)
        {
            byte[] intLengthData = new byte[sizeof(int)]; inputStream.Read(intLengthData, 0, sizeof(int));
            byte[] intData = new byte[BitConverter.ToInt32(intLengthData, 0)]; inputStream.Read(intData, 0, intData.Length);
            return Convert.ToInt32(intData);
        }

        /// <summary>
        /// Convert stream to <see cref="bool"/>.
        /// </summary>
        /// <param name="inputStream">The <see cref="Stream"/>.</param>
        /// <returns>Converted bool</returns>
        public bool DeserializeBool(Stream inputStream)
        {
            byte[] boolLengthData = new byte[sizeof(int)]; inputStream.Read(boolLengthData, 0, sizeof(int));
            byte[] boolData = new byte[BitConverter.ToInt32(boolLengthData, 0)]; inputStream.Read(boolData, 0, boolData.Length);
            return Convert.ToBoolean(boolData);
        }

        /// <summary>
        /// Convert stream to <see cref="ModuleInfo"/> array.
        /// </summary>
        /// <param name="inputStream">The <see cref="Stream"/>.</param>
        /// <returns>Converted <see cref="ModuleInfo"/> array.</returns>
        public ModuleInfo[] DeserializeModuleInfoArray(Stream inputStream)
        {
            byte[] moduleInfoLengthData = new byte[sizeof(int)]; inputStream.Read(moduleInfoLengthData, 0, sizeof(int));
            byte[] moduleInfoData = new byte[BitConverter.ToInt32(moduleInfoLengthData, 0)]; inputStream.Read(moduleInfoData, 0, moduleInfoData.Length);

            return Deserialize.FromBytes<ModuleInfo[]>(moduleInfoData);
        }

        /// <summary>
        /// Convert stream to <see cref="ServiceLogItem"/> array.
        /// </summary>
        /// <param name="inputStream">The <see cref="Stream"/>.</param>
        /// <returns>Converted <see cref="ServiceLogItem"/> array.</returns>
        public ServiceLogItem[] DeserializeServiceLogItemArray(Stream inputStream)
        {
            byte[] moduleInfoLengthData = new byte[sizeof(int)]; inputStream.Read(moduleInfoLengthData, 0, sizeof(int));
            byte[] moduleInfoData = new byte[BitConverter.ToInt32(moduleInfoLengthData, 0)]; inputStream.Read(moduleInfoData, 0, moduleInfoData.Length);

            return Deserialize.FromBytes<ServiceLogItem[]>(moduleInfoData);
        }

        /// <summary>
        /// Convert stream to <see cref="Account"/> array.
        /// </summary>
        /// <param name="inputStream">The <see cref="Stream"/>.</param>
        /// <returns>Converted <see cref="Account"/> array.</returns>
        public Account[] DeserializeAccountArray(Stream inputStream)
        {
            byte[] moduleInfoLengthData = new byte[sizeof(int)]; inputStream.Read(moduleInfoLengthData, 0, sizeof(int));
            byte[] moduleInfoData = new byte[BitConverter.ToInt32(moduleInfoLengthData, 0)]; inputStream.Read(moduleInfoData, 0, moduleInfoData.Length);

            return Deserialize.FromBytes<Account[]>(moduleInfoData);
        }

        /// <summary>
        /// Convert stream to <see cref="Group"/> array.
        /// </summary>
        /// <param name="inputStream">The <see cref="Stream"/>.</param>
        /// <returns>Converted <see cref="Group"/> array.</returns>
        public Group[] DeserializeGroupArray(Stream inputStream)
        {
            byte[] moduleInfoLengthData = new byte[sizeof(int)]; inputStream.Read(moduleInfoLengthData, 0, sizeof(int));
            byte[] moduleInfoData = new byte[BitConverter.ToInt32(moduleInfoLengthData, 0)]; inputStream.Read(moduleInfoData, 0, moduleInfoData.Length);

            return Deserialize.FromBytes<Group[]>(moduleInfoData);
        }

        /// <summary>
        /// Convert stream to <see cref="Byte[]"/> array.
        /// </summary>
        /// <param name="inputStream">The <see cref="Stream"/>.</param>
        /// <returns>Converted <see cref="Byte[]"/> array.</returns>
        public Byte[][] DeserializeByteArrayArray(Stream inputStream)
        {
            byte[] moduleInfoLengthData = new byte[sizeof(int)]; inputStream.Read(moduleInfoLengthData, 0, sizeof(int));
            byte[] moduleInfoData = new byte[BitConverter.ToInt32(moduleInfoLengthData, 0)]; inputStream.Read(moduleInfoData, 0, moduleInfoData.Length);

            return Deserialize.FromBytes<Byte[][]>(moduleInfoData);
        }

        /// <summary>
        /// Convert stream to <see cref="string"/> array.
        /// </summary>
        /// <param name="inputStream">The <see cref="Stream"/>.</param>
        /// <returns>Converted <see cref="string"/> array.</returns>
        public string[] DeserializeStringArray(Stream inputStream)
        {
            byte[] strLengthData = new byte[sizeof(int)]; inputStream.Read(strLengthData, 0, sizeof(int));
            byte[] strData = new byte[BitConverter.ToInt32(strLengthData, 0)]; inputStream.Read(strData, 0, strData.Length);
            return strData.Select(byteValue => byteValue.ToString()).ToArray();
        }

        /// <summary>
        /// Convert stream to <see cref="double"/> array.
        /// </summary>
        /// <param name="inputStream">The <see cref="Stream"/>.</param>
        /// <returns>Converted <see cref="double"/> array.</returns>
        public double[] DeserializeDoubleArray(Stream inputStream)
        {
            byte[] strLengthData = new byte[sizeof(int)]; inputStream.Read(strLengthData, 0, sizeof(int));
            byte[] strData = new byte[BitConverter.ToInt32(strLengthData, 0)]; inputStream.Read(strData, 0, strData.Length);
            return strData.Select(byteValue => (double)byteValue).ToArray();
        }

        /// <summary>
        /// Convert stream to <see cref="byte"/> array.
        /// </summary>
        /// <param name="inputStream">The <see cref="Stream"/>.</param>
        /// <returns>Converted <see cref="byte"/> array.</returns>
        public byte[] DeserializeByteArray(Stream inputStream)
        {
            byte[] strLengthData = new byte[sizeof(int)]; inputStream.Read(strLengthData, 0, sizeof(int));
            byte[] strData = new byte[BitConverter.ToInt32(strLengthData, 0)]; inputStream.Read(strData, 0, strData.Length);
            return strData;
        }
    }
}
