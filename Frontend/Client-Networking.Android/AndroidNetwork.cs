using System;
using System.Net;
using NetworkCommsDotNet;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using Networking.Data.Packets;
using Networking.Data;
using Networking.Data.ResponseCodes;
using System.Net.Sockets;

namespace Client_Networking.Android
{
    public class AndroidNetwork : IAppNetworking
    {
        private readonly int port = 3921;
        private ConnectionInfo connectionInfo;
        private readonly PacketManager packetManager;
        private bool packetsAppended;

        private IPAddress ipAddress;
        public event EventHandler LoginEvent;
        public event EventHandler ModuleListEvent;
        public event EventHandler AppFileEvent;
        public event EventHandler GroupList;
        public event EventHandler SessionReceived;
        public event EventHandler SessionVerificationResult;
        public event EventHandler UnAuthorized;
        public event EventHandler NetworkDisconnect;

        /// <summary>
        /// Create implementation of <see cref="IAppNetworking"/> for Android.
        /// </summary>
        public AndroidNetwork()
        {
            this.packetsAppended = false;
            this.packetManager = new PacketManager();
        }

        public void Connect(IPAddress address)
        {
            this.ipAddress = address;

            try
            {
                NetworkComms.RemoveGlobalConnectionCloseHandler(HandleConnectionClosed);

                NetworkComms.Shutdown();
            }
            catch
            {
                // Just here to avoid potential crash
            }

            if (!packetsAppended)
            {
                NetworkComms.AppendGlobalIncomingPacketHandler<Custom>("Custom", ReceiveCustomPacket);
                NetworkComms.AppendGlobalIncomingPacketHandler<int>(PacketName.ReLoginResult.ToString(), ReceiveLoginResult);
                NetworkComms.AppendGlobalIncomingPacketHandler<ModuleList>(PacketName.ReModuleList.ToString(), ModuleList);
                NetworkComms.AppendGlobalIncomingPacketHandler<AppFileData>(PacketName.ReAppFile.ToString(), AppFile);
                NetworkComms.AppendGlobalIncomingPacketHandler<GroupList>(PacketName.ReUserGroupList.ToString(), GroupListReceived);
                NetworkComms.AppendGlobalIncomingPacketHandler<int>(PacketName.ReSessionVerificationResult.ToString(), SessionVerificationResultReceived);
                NetworkComms.AppendGlobalIncomingPacketHandler<string>(PacketName.ReSessionId.ToString(), SessionIdReceived);
                NetworkComms.AppendGlobalIncomingPacketHandler<int>(PacketName.ReUnauthorized.ToString(), UnAuthorizedReceived);

                packetsAppended = true;
            }

            NetworkComms.AppendGlobalConnectionCloseHandler(HandleConnectionClosed);

            NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions(DPSManager.GetDataSerializer<NetworkCommsDotNet.DPSBase.ProtobufSerializer>(),
                NetworkComms.DefaultSendReceiveOptions.DataProcessors, NetworkComms.DefaultSendReceiveOptions.Options);

            if (Connectable(address.ToString()))
            {
                connectionInfo = new ConnectionInfo(address.ToString(), port);
            }
        }

        private bool Connectable(string ipAddress)
        {
            bool connectable = false;

            try
            {
                TcpClient client = new TcpClient();

                if (client.ConnectAsync(ipAddress, port).Wait(1000))
                {
                    connectable = true;
                }
            }
            catch
            {
                // Just here to avoid potential crash
            }

            return connectable;
        }

        private void HandleConnectionClosed(Connection connection)
        {
            InvokeDisconnect();
        }

        private void InvokeDisconnect()
        {
            if (NetworkDisconnect != null)
            {
                NetworkDisconnect.Invoke(EventArgs.Empty, EventArgs.Empty);
            }
        }

        protected void UnAuthorizedReceived(PacketHeader header, Connection connection, int code)
        {
            if (UnAuthorized != null)
            {
                UnAuthorized.Invoke(code, EventArgs.Empty);
            }
        }

        protected void SessionIdReceived(PacketHeader header, Connection connection, string sessionId)
        {
            if (SessionReceived != null)
            {
                SessionReceived.Invoke(sessionId, EventArgs.Empty);
            }
        }

        protected void SessionVerificationResultReceived(PacketHeader header, Connection connection, int result)
        {
            GenericResponse response = (GenericResponse)result;

            SessionVerificationResult.Invoke(response, EventArgs.Empty);
        }

        protected void GroupListReceived(PacketHeader header, Connection connection, GroupList groupList)
        {
            if (GroupList != null)
            {
                GroupList.Invoke(groupList.list, EventArgs.Empty);
            }
        }

        protected void AppFile(PacketHeader header, Connection connection, AppFileData file)
        {
            if (AppFileEvent != null)
            {
                AppFileEvent.Invoke(file, EventArgs.Empty);
            }
        }

        protected void ModuleList(PacketHeader header, Connection connection, ModuleList moduleList)
        {
            if (ModuleListEvent != null)
            {
                ModuleListEvent.Invoke(moduleList.ModuleInfoList, EventArgs.Empty);
            }
        }

        public void Send(Guid guid, PacketData packetData)
        {
            try
            {
                TCPConnection.GetConnection(connectionInfo).SendObject("Custom", packetData.ToPacket());
            }
            catch
            {
                InvokeDisconnect();
            }
        }

        public void Send(PacketData packetData)
        {
            try
            {
                TCPConnection.GetConnection(connectionInfo).SendObject("Custom", packetData.ToPacket());
            }
            catch
            {
                InvokeDisconnect();
            }
        }

        public void Send(string name, object obj)
        {
            try
            {
                TCPConnection.GetConnection(connectionInfo).SendObject(name, obj);
            }
            catch
            {
                InvokeDisconnect();
            }
        }

        protected void ReceiveLoginResult(PacketHeader header, Connection connection, int code)
        {
            if (LoginEvent != null)
            {
                LoginEvent.Invoke((LoginResponse)code, EventArgs.Empty);
            }
        }

        protected void ReceiveCustomPacket(PacketHeader header, Connection connection, Custom customPacket)
        {
            packetManager.Receive(connection.ConnectionInfo.NetworkIdentifier, customPacket);
        }

        public PacketManager PacketManager()
        {
            return packetManager;
        }

        public bool ReConnect()
        {
            bool connected = false;

            try
            {
                if (Connectable(ipAddress.ToString()))
                {
                    connectionInfo = new ConnectionInfo(ipAddress.ToString(), port);
                    TCPConnection.GetConnection(connectionInfo).EstablishConnection();

                    connected = true;
                }
            }
            catch
            {
                // Just here to avoid potential crash
            }

            return connected;
        }
    }
}
