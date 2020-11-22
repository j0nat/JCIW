using Networking.Data;
using Repository.Interface;
using System;
using System.Collections.Generic;
using JCIW.Data.Interfaces;

namespace Server
{
    /// <summary>
    /// This class implements <see cref="IDatabase"/>.
    /// </summary>
    [Serializable]
    class CustomPacketDatabase : IDatabase
    {
        private ICustomRepository customRepository;

        public CustomPacketDatabase(ICustomRepository customRepository)
        {
            this.customRepository = customRepository;
        }

        public void Create(PacketDefinition packetDefinition)
        {
            Dictionary<string, Type> columns = new Dictionary<string, Type>();

            foreach (PacketDataItem column in packetDefinition.Items)
            {
                columns.Add(column.Name, column.Type);
            }

            customRepository.CreateTable(packetDefinition.Name, columns);
        }

        public void Merge(PacketData entity, string[] keys)
        {
            // UPDATE
            Dictionary<string, string> commandNames = new Dictionary<string, string>();
            Dictionary<string, object> commands = new Dictionary<string, object>();

            string query = "UPDATE " + entity.Definition.Name + " SET ";

            int count = 0;
            for (int i = 0; i < entity.Definition.Items.Length; i++)
            {
                query += entity.Definition.Items[i].Name + "=" + "@" + count.ToString();
                commands.Add("@" + count.ToString(), entity.Get(entity.Definition.Items[i].Name));
                commandNames.Add(entity.Definition.Items[i].Name, "@" + count.ToString());

                if ((i + 1) < (entity.Definition.Items.Length))
                {
                    query += ", ";
                }

                count++;
            }

            query += " WHERE ";

            for (int i = 0; i < keys.Length; i++)
            {
                string key = keys[i];

                query += key + "=" + commandNames[key];

                if ((i + 1) < (keys.Length))
                {
                    query += " AND ";
                }
            }

            customRepository.ExecuteQuery(query, commands);
        }

        public void Persist(PacketData entity)
        {
            // INSERT
            Dictionary<string, object> commands = new Dictionary<string, object>();

            string query = "INSERT INTO " + entity.Definition.Name + " VALUES(";

            int count = 0;
            for (int i = 0; i < entity.Definition.Items.Length; i++)
            {
                query += "@" + count.ToString();
                commands.Add("@" + count.ToString(), entity.Get(entity.Definition.Items[i].Name));

                if ((i + 1) < (entity.Definition.Items.Length))
                {
                    query += ", ";
                }

                count++;
            }

            query += ")";

            customRepository.ExecuteQuery(query, commands);
        }

        public PacketData[] Read(PacketDefinition packetDefinition, string query)
        {
            List<PacketData> packetDataArray = new List<PacketData>();
            Dictionary<string, string>[] rows = customRepository.Read(query);

            foreach (Dictionary<string, string> columns in rows)
            {
                PacketData packetData = new PacketData(packetDefinition);

                foreach (KeyValuePair<string, string> row in columns)
                {
                    packetData.Add(row.Key, row.Value);
                }

                packetDataArray.Add(packetData);
            }

            return packetDataArray.ToArray();
        }

        public void Remove(PacketData entity, string[] keys)
        {
            // DELETE
            Dictionary<string, object> commands = new Dictionary<string, object>();

            string query = "DELETE FROM " + entity.Definition.Name + " WHERE ";

            int count = 0;
            for (int i = 0; i < keys.Length; i++)
            {
                query += keys[i] + "=" + "@" + count.ToString();
                commands.Add("@" + count.ToString(), entity.Get(keys[i]));

                if ((i + 1) < (keys.Length))
                {
                    query += " AND ";
                }

                count++;
            }

            customRepository.ExecuteQuery(query, commands);
        }
    }
}
