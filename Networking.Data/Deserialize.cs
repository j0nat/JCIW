using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Networking.Data
{
    /// <summary>
    /// This is an extension class to byte[]. Used to convert to objects.
    /// </summary>
    public static class Deserialize
    {
        public static T FromBytes<T>(this byte[] inputSource)
        {
            using (MemoryStream memoryStream = new MemoryStream(inputSource))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                memoryStream.Seek(0, SeekOrigin.Begin);

                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}
