using Networking.Data.Packets;
using System.Collections.Generic;

namespace Networking.Data
{
    /// <summary>
    /// Used to create packet data using <see cref="PacketDefinition"/> specific fields.
    /// To be used when sending and receiving packets via services and apps.
    /// </summary>
    public class PacketData
    {
        public PacketDefinition Definition { get; private set; }
        private readonly Dictionary<string, object> items;

        /// <summary>
        /// Creates a <see cref="PacketData"/>.
        /// </summary>
        /// <param name="definition">The <see cref="PacketDefinition"/>.</param>
        /// <param name="packet">The <see cref="Custom"/> packet.</param>
        public PacketData(PacketDefinition definition, Custom packet)
        {
            this.Definition = definition;
            this.items = new Dictionary<string, object>();

            Load(packet);
        }

        /// <summary>
        /// Creates a <see cref="PacketData"/>.
        /// </summary>
        /// <param name="definition">The <see cref="PacketDefinition"/>.</param>
        public PacketData(PacketDefinition definition)
        {
            this.Definition = definition;
            this.items = new Dictionary<string, object>();
        }

        /// <summary>
        /// Fill data from packet.
        /// </summary>
        /// <param name="data">The <see cref="Custom"/> packet receieved from networking.</param>
        private void Load(Custom data)
        {
            int currentStringIndex = 0;
            int currentNumberIndex = 0;
            int currentByteArrayIndex = 0;

            foreach (PacketDataItem dataItem in Definition.Items)
            {
                if (dataItem.Type == typeof(string))
                {
                    items.Add(dataItem.Name, data.strings[currentStringIndex]);

                    currentStringIndex++;
                }
                else
                if (dataItem.Type == typeof(double))
                {
                    items.Add(dataItem.Name, data.numbers[currentNumberIndex]);

                    currentNumberIndex++;
                }
                else
                if (dataItem.Type == typeof(byte[]))
                {
                    items.Add(dataItem.Name, data.byteArrays[currentNumberIndex]);

                    currentByteArrayIndex++;
                }
            }
        }

        /// <summary>
        /// Convert <see cref="PacketData"/> to <see cref="Custom"/>.
        /// </summary>
        /// <returns>A <see cref="Custom"/> packet.</returns>
        public Custom ToPacket()
        {
            List<string> strings = new List<string>();
            List<double> numbers = new List<double>();
            List<byte[]> byteArrays = new List<byte[]>();

            int currentStringIndex = 0;
            int currentNumberIndex = 0;
            int currentByteArrayIndex = 0;

            foreach (PacketDataItem dataItem in Definition.Items)
            {
                if (dataItem.Type == typeof(string))
                {
                    strings.Add((string)items[dataItem.Name]);

                    currentStringIndex++;
                }
                else
                if (dataItem.Type == typeof(double))
                {
                    numbers.Add((double)items[dataItem.Name]);

                    currentNumberIndex++;
                }
                else
                if (dataItem.Type == typeof(byte[]))
                {
                    byteArrays.Add((byte[])items[dataItem.Name]);

                    currentByteArrayIndex++;
                }
            }

            return new Custom(Definition.Name, strings.ToArray(), numbers.ToArray(), byteArrays.ToArray());
        }

        /// <summary>
        /// Add <see cref="object"/> to <see cref="PacketData"/>.
        /// </summary>
        /// <param name="name">The name identifying the data.</param>
        /// <param name="data">The data to store.</param>
        public void Add(string name, object data)
        {
            if (items.ContainsKey(name))
            {
                items[name] = data;
            }
            else
            {
                items.Add(name, data);
            }
        }

        /// <summary>
        /// Retrieve data from <see cref="PacketData"/>.
        /// </summary>
        /// <param name="name">The name identifying the data.</param>
        /// <returns>The stored <see cref="object"/>.</returns>
        public object Get(string name)
        {
            return items[name];
        }
    }
}
