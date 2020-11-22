using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using NetworkCommsDotNet.DPSBase;
using System;
using Networking.Data.ResponseCodes;
using Networking.Data.Packets;
using Networking.Data;
using System.Windows;

namespace Client_Admin
{
    /// <summary>
    /// This class is used to connecting to server and to send and handle incoming packets.
    /// </summary>
    public class AdminNetwork
    {
        public static ConnectionInfo ConnectionInfo { get; set; }
        public static string IpAddress { get; set; }
        private static int port = 3921;

        /// <summary>
        /// Configure connection settings and register packet handlers.
        /// </summary>
        public static void Initialize()
        {
            ConnectionInfo = new ConnectionInfo(IpAddress, port);

            NetworkComms.AppendGlobalIncomingPacketHandler<ModuleList>(
                PacketName.ReModuleList.ToString(), ModuleList);

            NetworkComms.AppendGlobalIncomingPacketHandler<ModuleList>(
                PacketName.ReAllAppList.ToString(), AppList);

            NetworkComms.AppendGlobalIncomingPacketHandler<int>(
                PacketName.ReLoginResult.ToString(), LoginResult);

            NetworkComms.AppendGlobalIncomingPacketHandler<ServiceCommand>(
                PacketName.ReServiceResponse.ToString(), ServiceResponse);

            NetworkComms.AppendGlobalIncomingPacketHandler<ServiceLogList>(
                PacketName.ReServiceLog.ToString(), ServiceLog);

            NetworkComms.AppendGlobalIncomingPacketHandler<int>(
                PacketName.ReRegisterAccount.ToString(), RegisterAccount);

            NetworkComms.AppendGlobalIncomingPacketHandler<AccountList>(
                PacketName.ReAccountList.ToString(), AccountListReceived);

            NetworkComms.AppendGlobalIncomingPacketHandler<GroupList>(
                PacketName.ReUserGroupList.ToString(), GroupListReceived);

            NetworkComms.AppendGlobalIncomingPacketHandler<GenericResponse>(
                PacketName.ReUpdateAccountInformation.ToString(), AccountInformationResponse);

            NetworkComms.AppendGlobalIncomingPacketHandler<GenericResponse>(
                PacketName.ReUpdatePassword.ToString(), PasswordChangeResponse);

            NetworkComms.AppendGlobalIncomingPacketHandler<GenericResponse>(
                PacketName.ReDeleteGroupFromUser.ToString(), DeleteGroupFromUserResponse);

            NetworkComms.AppendGlobalIncomingPacketHandler<GenericResponse>(
                PacketName.ReAddGroupToUser.ToString(), AddGroupToUserReceived);

            NetworkComms.AppendGlobalIncomingPacketHandler<GenericResponse>(
                PacketName.ReDeleteAccount.ToString(), DeleteAccount);

            NetworkComms.AppendGlobalIncomingPacketHandler<GenericResponse>(
                PacketName.ReDeleteGroup.ToString(), DeleteGroup);

            NetworkComms.AppendGlobalIncomingPacketHandler<GenericResponse>(
                PacketName.ReAddGroup.ToString(), AddGroup);

            NetworkComms.AppendGlobalIncomingPacketHandler<GroupList>(
                PacketName.ReAppGroupList.ToString(), AppGroupList);

            NetworkComms.AppendGlobalIncomingPacketHandler<int>(
                PacketName.ReUnauthorized.ToString(), UnAuthorized);

            NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(),
                NetworkComms.DefaultSendReceiveOptions.DataProcessors, NetworkComms.DefaultSendReceiveOptions.Options);
        }

        #region Packet Handlers
        protected static void UnAuthorized(PacketHeader header, Connection connection, int code)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show("You are not authorized. Closing.", "Authorization error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }));
        }

        protected static void AppGroupList(PacketHeader header, Connection connection, GroupList groupList)
        {
            NetworkingEvents.RaiseAppGroupListResultEvent(groupList.list);
        }

        protected static void DeleteGroup(PacketHeader header, Connection connection, GenericResponse response)
        {
            NetworkingEvents.RaiseAddRemoveGroupReceivedEvent(response);
        }

        protected static void AddGroup(PacketHeader header, Connection connection, GenericResponse response)
        {
            NetworkingEvents.RaiseAddRemoveGroupReceivedEvent(response);
        }

        protected static void DeleteAccount(PacketHeader header, Connection connection, GenericResponse response)
        {
            NetworkingEvents.RaiseDeleteAccountResultEvent(response);
        }

        protected static void RegisterAccount(PacketHeader header, Connection connection, int code)
        {
            NetworkingEvents.RaiseRegisterResultEvent((RegisterResponse)code);
        }

        protected static void AccountListReceived(PacketHeader header, Connection connection, AccountList accountList)
        {
            NetworkingEvents.RaiseAccountListResultEvent(accountList.list);
        }

        protected static void GroupListReceived(PacketHeader header, Connection connection, GroupList groupList)
        {
            NetworkingEvents.RaiseGroupListResultEvent(groupList.list);
        }

        protected static void AddGroupToUserReceived(PacketHeader header, Connection connection, GenericResponse response)
        {
            NetworkingEvents.RaiseGroupAddedToUserResultEvent(response);
        }

        protected static void ModuleList(PacketHeader header, Connection connection, ModuleList moduleList)
        {
            NetworkingEvents.RaiseModuleListReceivedEvent(moduleList);
        }

        protected static void AppList(PacketHeader header, Connection connection, ModuleList moduleList)
        {
            NetworkingEvents.RaiseAppListReceivedEvent(moduleList);
        }

        protected static void ServiceLog(PacketHeader header, Connection connection, ServiceLogList response)
        {
            NetworkingEvents.RaiseServiceLogListReceivedEvent(response);
        }

        protected static void ServiceResponse(PacketHeader header, Connection connection, ServiceCommand response)
        {
            NetworkingEvents.RaiseServiceCommandResultEvent(response);
        }

        protected static void LoginResult(PacketHeader header, Connection connection, int responseCode)
        {
            LoginResponse response = (LoginResponse)responseCode;

            NetworkingEvents.RaiseLoginResultEvent(response);
        }

        protected static void PasswordChangeResponse(PacketHeader header, Connection connection, GenericResponse response)
        {
            NetworkingEvents.RaisePasswordChangeResultEvent(response);
        }

        protected static void AccountInformationResponse(PacketHeader header, Connection connection, GenericResponse response)
        {
            NetworkingEvents.RaiseAccountUpdatedResultEvent(response);
        }

        protected static void DeleteGroupFromUserResponse(PacketHeader header, Connection connection, GenericResponse response)
        {
            NetworkingEvents.RaiseUserGroupDeletedResultEvent(response);
        }
        #endregion

        #region Connection method
        /// <summary>
        /// Get the active active <see cref="ConnectionInfo"/>.
        /// </summary>
        /// <returns>The active <see cref="ConnectionInfo"/>.</returns>
        public static Connection Connection()
        {
            return TCPConnection.GetConnection(ConnectionInfo);
        }
        #endregion

        #region Send methods
        public static void SendDeleteGroupFromUser(long accountId, long groupId)
        {
            GroupRequest groupRequest = new GroupRequest(groupId, accountId);

            Send(PacketName.RequestDeleteGroupFromUser.ToString(), groupRequest);
        }

        public static void SendServiceLogRequest(long serviceID, long dateLimit)
        {
            Send(PacketName.RequestServiceLog.ToString(), new ServiceLogRequest(serviceID, dateLimit));
        }

        public static void SendLoginRequest(string username, string password)
        {
            LoginRequest account = new LoginRequest(username, password, true);

            Send(PacketName.RequestLogin.ToString(), account);
        }

        public static void SendRegisterRequest(string username, string password, string firstname, string lastname)
        {
            RegisterRequest account = new RegisterRequest(username, password, firstname, lastname);

            Send(PacketName.RequestRegisterAccount.ToString(), account);
        }

        public static void SendRequestGroupUserList(long accountId)
        {
            Send(PacketName.RequestUserGroupList.ToString(), accountId);
        }

        public static void SendRequestAllAppList()
        {
            Send(PacketName.RequestAllAppList.ToString(), 0);
        }

        public static void SendRequestAccountList()
        {
            Send(PacketName.RequestAccountList.ToString(), 1);
        }

        public static void SendServiceCommand(long serviceId, string command)
        {
            ServiceCommand serviceCommand = new ServiceCommand(serviceId, command);

            Send(PacketName.RequestServiceCommand.ToString(), serviceCommand);
        }

        public static void SendDisableService(long serviceId)
        {
            Send(PacketName.PostDisableService.ToString(), serviceId);
        }

        public static void SendDeleteService(long serviceId)
        {
            Send(PacketName.PostDeleteService.ToString(), serviceId);
        }

        public static void SendEnableService(long serviceId)
        {
            Send(PacketName.PostEnableService.ToString(), serviceId);
        }

        public static void SendModuleListRequest()
        {
            Send(PacketName.RequestModuleList.ToString(), 1);
        }

        public static void SendUpdateAccountPassword(long accountId, string password)
        {
            PasswordChangeRequest passwordChangeRequest = new PasswordChangeRequest(accountId, password);

            Send(PacketName.RequestUpdatePassword.ToString(), passwordChangeRequest);
        }

        public static void SendAddGroupToUser(long accountId, long groupId)
        {
            GroupRequest groupRequest = new GroupRequest(groupId, accountId);

            Send(PacketName.RequestAddGroupToUser.ToString(), groupRequest);
        }

        public static void SendRequestDeleteAccount(long accountId)
        {
            Send(PacketName.RequestDeleteAccount.ToString(), accountId);
        }

        public static void SendUpdateAccountInformation(long accountId, string username, string firstname, string lastname)
        {
            Account account = new Account(accountId, username, firstname, lastname);

            Send(PacketName.RequestUpdateAccountInformation.ToString(), account);
        }

        public static void SendRequestGroupList()
        {
            Send(PacketName.RequestGroupList.ToString(), 1);
        }

        public static void SendRequestAppGroupList(long appId)
        {
            Send(PacketName.RequestAppGroupList.ToString(), appId);
        }

        public static void SendDisableApp(long appId)
        {
            Send(PacketName.PostDisableApp.ToString(), appId);
        }

        public static void SendEnableApp(long appId)
        {
            Send(PacketName.PostEnableApp.ToString(), appId);
        }

        public static void SendRemoveGroupFromApp(long appId, long groupId)
        {
            Send(PacketName.PostRemoveGroupFromApp.ToString(), new GroupRequest(groupId, appId));
        }

        public static void SendAddGroupToApp(long appId, long groupId)
        {
            Send(PacketName.PostAddGroupToApp.ToString(), new GroupRequest(groupId, appId));
        }

        public static void SendDeleteGroup(long groupId)
        {
            Send(PacketName.RequestDeleteGroup.ToString(), groupId);
        }

        public static void SendAddGroup(string groupName, string groupDescription)
        {
            Group group = new Group(0, groupName, groupDescription);

            Send(PacketName.RequestAddGroup.ToString(), group);
        }

        private static void Send(string packetName, object data)
        {
            try
            {
                TCPConnection.GetConnection(ConnectionInfo).
                    SendObject(packetName, data);
            }
            catch
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show("No connection to server. Exiting.", "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }));
            }
        }
        #endregion
    }
}
