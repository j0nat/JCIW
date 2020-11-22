using System;

namespace Networking.Data
{
    /// <summary>
    /// Used to define data types in <see cref="PacketDefinition"/>.
    /// </summary>
    public class PacketDataItem
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }

        /// <summary>
        /// Create a <see cref="PacketDataItem"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="PacketDataItem"/>.</param>
        /// <param name="type">The <see cref="Type"/> of the <see cref="PacketDataItem"/>.</param>
        public PacketDataItem(string name, Type type)
        {
            this.Name = name;
            this.Type = type;
        }
    }
}
