using System;
using System.Net;
using Networking.Data.Packets;

namespace Networking.Data
{
    /// <summary>
    /// This networking interface is used by JCIW.App to implement networking for apps.
    /// </summary>
    public interface IAppNetworking
    {
        /// <summary>
        /// Event for handling of login result packets.
        /// </summary>
        event EventHandler LoginEvent;

        /// <summary>
        /// Event for handling of <see cref="ModuleList"/> packets.
        /// </summary>
        event EventHandler ModuleListEvent;

        /// <summary>
        /// Event for handling of <see cref="AppFileData"/> packets.
        /// </summary>
        event EventHandler AppFileEvent;

        /// <summary>
        /// Event for handling of <see cref="GroupList"/> packets.
        /// </summary>
        event EventHandler GroupList;

        /// <summary>
        /// Event for handling of session packets.
        /// </summary>
        event EventHandler SessionReceived;

        /// <summary>
        /// Event for handling of session verification result packets.
        /// </summary>
        event EventHandler SessionVerificationResult;

        /// <summary>
        /// Event for handling of unauthorized packets.
        /// </summary>
        event EventHandler UnAuthorized;

        /// <summary>
        /// Event for handling of network disconnects.
        /// </summary>
        event EventHandler NetworkDisconnect;

        /// <summary>
        /// Connect to server.
        /// </summary>
        /// <param name="address">The server <see cref="IPAddress"/>.</param>
        void Connect(IPAddress address);

        /// <summary>
        /// Try to reconnect to server.
        /// </summary>
        /// <returns>True if successfull and false if unsuccessfull.</returns>
        bool ReConnect();

        /// <summary>
        /// Send <see cref="PacketData"/>.
        /// </summary>
        /// <param name="packetData">The <see cref="PacketData"/> to send to the client.</param>
        void Send(PacketData packetData);

        /// <summary>
        /// Send <see cref="PacketData"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="guid">The client identifying <see cref="Guid"/>.</param>
        /// <param name="packetData">The <see cref="PacketData"/> to send to the client.</param>
        void Send(Guid guid, PacketData packetData);

        /// <summary>
        /// Send packet to server.
        /// </summary>
        /// <param name="name">The identifying name of the packet.</param>
        /// <param name="obj">A packet.</param>
        void Send(string name, object obj);

        /// <summary>
        /// Retrieve the <see cref="PacketManager"/>.
        /// </summary>
        /// <returns>The <see cref="PacketManager"/>.</returns>
        PacketManager PacketManager();
    }
}
