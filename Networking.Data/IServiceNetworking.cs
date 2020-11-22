using System;

namespace Networking.Data
{
    /// <summary>
    /// This interface is used to expose networking functions for services.
    /// </summary>
    public interface IServiceNetworking
    {
        /// <summary>
        /// Send <see cref="PacketData"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="guid">The client identifying <see cref="Guid"/>.</param>
        /// <param name="packetData">The <see cref="PacketData"/> to send to the client.</param>
        void Send(Guid guid, PacketData packetData);
    }
}
