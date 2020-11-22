using JCIW.Data.Interfaces;
using Networking.Data;
using System;
using System.Collections.Generic;

namespace JCIW
{
    /// <summary>
    /// The base service class to be inherited when creating new services.
    /// </summary>
    public class ServiceBase
    {
        private long serviceId;
        private PacketManager packetManager;
        private List<PacketDefinition> registeredPacketDefinitions;
        private ILogManager logManager;
        public IServiceNetworking Networking;
        public IDatabase Database;
        public IServerFunctions Server;

        /// <summary>
        /// Start running service with server components.
        /// </summary>
        /// <param name="serviceId">The service ID.</param>
        /// <param name="networking">The <see cref="IServiceNetworking"/> implementation.</param>
        /// <param name="database">The <see cref="IDatabase"/> implementation.</param>
        /// <param name="packetManager">The server <see cref="PacketManager"/>.</param>
        /// <param name="logManager">The <see cref="ILogManager"/> implementation.</param>
        /// <param name="serverFunctions">The <see cref="IServerFunctions"/> implementation.</param>
        public void Initialize(long serviceId, IServiceNetworking networking, IDatabase database,
            PacketManager packetManager, ILogManager logManager, IServerFunctions serverFunctions)
        {
            this.serviceId = serviceId;
            this.logManager = logManager;
            this.registeredPacketDefinitions = new List<PacketDefinition>();
            this.packetManager = packetManager;
            this.Networking = networking;
            this.Database = database;
            this.Server = serverFunctions;

            Run();
        }

        /// <summary>
        /// Add message to service log.
        /// </summary>
        /// <param name="message">The log message.</param>
        public void AddLog(string message)
        {
            logManager.Add(serviceId, message);
        }

        /// <summary>
        /// Add <see cref="PacketDefinition"/> to <see cref="PacketManager"/>.
        /// </summary>
        /// <param name="packetDefinition">The packet definition.</param>
        public void RegisterPacket(PacketDefinition packetDefinition)
        {
            packetManager.Register(packetDefinition);
            registeredPacketDefinitions.Add(packetDefinition);
        }

        /// <summary>
        /// Remove <see cref="PacketDefinition"/> from <see cref="PacketManager"/>.
        /// </summary>
        /// <param name="packetDefinition">The packet definition.</param>
        public void UnRegisterPacket(PacketDefinition packetDefinition)
        {
            packetManager.UnRegister(packetDefinition);
        }

        /// <summary>
        ///  Create service resources.
        /// </summary>
        public virtual void Run()
        {

        }

        /// <summary>
        ///  Returns the result of command.
        /// </summary>
        /// <param name="command">The text command.</param>
        /// <returns>
        ///  The result of the command specified.
        /// </returns>
        public virtual string Command(string command)
        {
            return "No commands implemented";
        }

        /// <summary>
        ///  Service name.
        /// </summary>
        /// <returns>
        ///  The service name.
        /// </returns>
        public virtual string Name()
        {
            return "NoNameService";
        }

        /// <summary>
        ///  Service version.
        /// </summary>
        /// <returns>
        ///  The service version.
        /// </returns>
        public virtual string Version()
        {
            return "0.0.0";
        }

        /// <summary>
        ///   Unload and deactivate service.
        /// </summary>
        public void Unload()
        {
            // Remove all events and dispose controls etc.
            // Free up resources
            foreach (PacketDefinition registeredPacketDefinition in registeredPacketDefinitions)
            {
                packetManager.UnRegister(registeredPacketDefinition);
            }

            registeredPacketDefinitions.Clear();

            Dispose();
        }

        /// <summary>
        ///   Release resources used by service.
        /// </summary>
        public virtual void Dispose()
        {

        }
    }
}
