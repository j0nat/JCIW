using System;

namespace Networking.Data
{
    /// <summary>
    /// This class is used to define the <see cref="PacketDataItem"/> fields and name
    /// to use with app / service communication.
    /// </summary>
    public class PacketDefinition
    {
        public string Name { get; private set; }
        public readonly PacketDataItem[] Items;

        public delegate void PacketReceived(Guid guid, PacketData data);
        public event PacketReceived PacketReceivedEvent;

        /// <summary>
        /// Create a <see cref="PacketDefinition"/>.
        /// </summary>
        /// <param name="name">The name of the packet definition.</param>
        /// <param name="items">The <see cref="PacketDataItem"/> packet definitions.</param>
        public PacketDefinition(string name, PacketDataItem[] items)
        {
            this.Name = name;
            this.Items = items;
        }

        /// <summary>
        /// Raise PacketReceivedEvent.
        /// </summary>
        /// <param name="guid">The <see cref="Guid"/>.</param>
        /// <param name="packet">The <see cref="PacketData"/>.</param>
        public void RaiseEvent(Guid guid, PacketData packet)
        {
            if (PacketReceivedEvent != null)
            {
                PacketReceivedEvent.Invoke(guid, packet);
            }
        }
    }
}
