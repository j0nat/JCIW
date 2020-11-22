using JCIW.Data;
using JCIW.Data.Drawing;
using JCIW.Data.Interfaces;
using Networking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCIW
{
    /// <summary>
    /// The base app class to be inherited when creating new apps.
    /// </summary>
    public class AppBase
    {
        private PacketManager packetManager;
        private List<PacketDefinition> registeredPacketDefinitions;
        public Frame Frame;
        public AppNetworking Networking;
        public Platform Platform;
        public IPlatformFunctions PlatformFunctions;
        public string[] Groups { get; private set; }

        /// <summary>
        /// Start app.
        /// </summary>
        /// <param name="frame">The graphical <see cref="Frame"/>.</param>
        /// <param name="networking">The <see cref="IAppNetworking"/> implementation.</param>
        /// <param name="platformFunctions">The <see cref="IPlatformFunctions"/> implementation.</param>
        /// <param name="platform">The <see cref="Platform"/> type.</param>
        /// <param name="groups">The user's groups.</param>
        public void Initialize(Frame frame, IAppNetworking networking, IPlatformFunctions platformFunctions, Platform platform, string[] groups)
        {
            this.Groups = groups;
            this.PlatformFunctions = platformFunctions;
            this.Frame = frame;
            this.Platform = platform;
            this.packetManager = networking.PacketManager();
            this.Networking = new AppNetworking(networking);
            this.registeredPacketDefinitions = new List<PacketDefinition>();

            Run();
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
        ///  Create app resources.
        /// </summary>
        public virtual void Run()
        {

        }

        /// <summary>
        ///  Called when graphics engine draws to screen.
        /// </summary>
        public virtual void Draw()
        {

        }

        /// <summary>
        ///  App name.
        /// </summary>
        /// <returns>
        ///  The app name.
        /// </returns>
        public virtual string Name()
        {
            return "NoNameApp";
        }

        /// <summary>
        ///  App version.
        /// </summary>
        /// <returns>
        ///  The app version.
        /// </returns>
        public virtual string Version()
        {
            return "0.0.0";
        }

        /// <summary>
        ///   Unload and deactivate app.
        /// </summary>
        public void Unload()
        {
            // Remove all events and dispose controls etc.
            // Free up resources

            Frame.ClearGameComponents();

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
