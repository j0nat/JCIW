using Networking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCIW.Data.Interfaces
{
    /// <summary>
    /// This interface is used to expose database functionality to the service developer.
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Add row in table.
        /// </summary>
        /// <param name="entity">The row to add.</param>
        void Persist(PacketData entity);

        /// <summary>
        /// Update existing rows in table.
        /// </summary>
        /// <param name="entity">The row to update.</param>
        /// <param name="keys">The row columns to update as conditional statements.</param>
        void Merge(PacketData entity, string[] keys);

        /// <summary>
        /// Delete rows from table.
        /// </summary>
        /// <param name="entity">The row to remove.</param>
        /// <param name="keys">The row columns to add as conditional statements.</param>
        void Remove(PacketData entity, string[] keys);

        /// <summary>
        /// Create a new table.
        /// </summary>
        /// <param name="packetDefinition">The table definition.</param>
        void Create(PacketDefinition packetDefinition);

        /// <summary>
        /// Read from table.
        /// </summary>
        /// <param name="packetDefinition">Table columns.</param>
        /// <param name="query">SQL Query to run.</param>
        /// <returns>PacketData array.</returns>
        PacketData[] Read(PacketDefinition packetDefinition, string query);
    }
}
