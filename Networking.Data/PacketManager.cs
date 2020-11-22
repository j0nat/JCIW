using Networking.Data.Packets;
using System;
using System.Collections.Generic;

namespace Networking.Data
{
    /// <summary>
    /// This class is used by apps and services to register packetdefinitions
    /// so they can use for communication.
    /// </summary>
    [Serializable]
    public class PacketManager
    {
        private Dictionary<string, PacketDefinition> definitions;

        /// <summary>
        /// Create <see cref="PacketManager"/>.
        /// </summary>
        public PacketManager()
        {
            definitions = new Dictionary<string, PacketDefinition>();
        }

        /// <summary>
        /// Add <see cref="PacketDefinition"/> to definitions.
        /// </summary>
        /// <param name="definition">The <see cref="PacketDefinition"/>.</param>
        public void Register(PacketDefinition definition)
        {
            lock (definition)
            {
                if (definitions.ContainsKey(definition.Name))
                {
                    throw new Exception("Packet definition name already exists.");
                }

                definitions.Add(definition.Name, definition);
            }
        }

        /// <summary>
        /// Remove <see cref="PacketDefinition"/> to definitions.
        /// </summary>
        /// <param name="definition">The <see cref="PacketDefinition"/>.</param>
        public void UnRegister(PacketDefinition definition)
        {
            lock (definition)
            {
                if (definitions.ContainsKey(definition.Name))
                {
                    definitions.Remove(definition.Name);
                }
            }
        }

        /// <summary>
        /// Receive packet and raise event.
        /// </summary>
        /// <param name="guid">The <see cref="Guid"/>.</param>
        /// <param name="packet">The <see cref="Custom"/> packet data.</param>
        public void Receive(Guid guid, Custom packet)
        {
            if (definitions.ContainsKey(packet.name))
            {
                PacketDefinition definition = definitions[packet.name];

                PacketData data = new PacketData(definition, packet);

                definition.RaiseEvent(guid, data);
            }
        }
    }
}
