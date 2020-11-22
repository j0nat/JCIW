using Networking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCIW
{
    /// <summary>
    /// This class exposes networking functionality to the app developer.
    /// </summary>
    public class AppNetworking
    {
        private IAppNetworking appNetworking;

        /// <summary>
        /// Create <see cref="AppNetworking"/>.
        /// </summary>
        /// <param name="appNetworking">The <see cref="IAppNetworking"/> implementation.</param>
        public AppNetworking(IAppNetworking appNetworking)
        {
            this.appNetworking = appNetworking;
        }

        /// <summary>
        /// Send to server.
        /// </summary>
        /// <param name="packetData">The <see cref="PacketData"/>.</param>
        public void Send(PacketData packetData)
        {
            appNetworking.Send(packetData);
        }
    }
}
