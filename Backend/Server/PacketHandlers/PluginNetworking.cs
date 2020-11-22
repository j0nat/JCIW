using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using Networking.Data;
using Networking.Data.Packets;
using System;
using System.Collections.Generic;

namespace Server.PacketHandlers
{
    /// <summary>
    /// This class implements <see cref="IServiceNetworking"/>.
    /// </summary>
    [Serializable]
    class PluginNetworking : IServiceNetworking
    {
        private PacketManager packetManager;
        private AccountManager accountManager;

        public PluginNetworking(PacketManager packetManager, AccountManager accountManager)
        {
            this.packetManager = packetManager;
            this.accountManager = accountManager;

            NetworkComms.AppendGlobalIncomingPacketHandler<Custom>("Custom", ReceiveCustomPacket);
        }

        protected void ReceiveCustomPacket(PacketHeader header, Connection connection, Custom customPacket)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!accountManager.Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            packetManager.Receive(connection.ConnectionInfo.NetworkIdentifier, customPacket);
        }

        public void Send(Guid guid, PacketData packetData)
        {
            List<Connection> connections = NetworkComms.GetExistingConnection(guid, ConnectionType.TCP);

            foreach (Connection connection in connections)
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject("Custom", packetData.ToPacket());
            }
        }
    }
}
