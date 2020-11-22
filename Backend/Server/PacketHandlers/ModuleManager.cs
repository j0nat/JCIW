using System;
using System.Collections.Generic;
using System.IO;
using JCIW.Module;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using Networking.Data.Packets;
using Networking.Data.ResponseCodes;
using Repository.Entities;
using Repository.Interface;
using Networking.Data;

namespace Server.PacketHandlers
{
    /// <summary>
    /// This class handles the module actions from admin client. Install / Delete / Modify.
    /// </summary>
    public class ModuleManager
    {
        private readonly IModuleRepository moduleRepository;
        private readonly IGroupRepository groupRepository;
        private readonly ServiceManager serviceManager;
        private readonly AccountManager accountManager;

        /// <summary>
        /// Creates <see cref="ModuleManager"/>.
        /// </summary>
        /// <param name="accountManager"><see cref="AccountManager"/></param>
        /// <param name="moduleRepository"><see cref="IModuleRepository"/></param>
        /// <param name="serviceManager"><see cref="ServiceManager"/></param>
        /// <param name="groupRepository"><see cref="IGroupRepository"/></param>
        public ModuleManager(AccountManager accountManager, IModuleRepository moduleRepository, ServiceManager serviceManager, IGroupRepository groupRepository)
        {
            this.groupRepository = groupRepository;
            this.accountManager = accountManager;
            this.moduleRepository = moduleRepository;
            this.serviceManager = serviceManager;

            NetworkComms.AppendGlobalIncomingPacketHandler<int>(PacketName.RequestModuleList.ToString(), ReceiveModuleListRequest);
            NetworkComms.AppendGlobalIncomingPacketHandler<long>(PacketName.PostEnableService.ToString(), ReceiveEnableService);
            NetworkComms.AppendGlobalIncomingPacketHandler<long>(PacketName.PostDisableService.ToString(), ReceiveDisableService);
            NetworkComms.AppendGlobalIncomingPacketHandler<long>(PacketName.PostDeleteService.ToString(), ReceiveDeleteService);
            NetworkComms.AppendGlobalIncomingPacketHandler<ServiceCommand>(PacketName.RequestServiceCommand.ToString(), ReceiveServiceCommand);
            NetworkComms.AppendGlobalIncomingPacketHandler<ServiceLogRequest>(PacketName.RequestServiceLog.ToString(), ReceiveRequestServiceLog);
            NetworkComms.AppendGlobalIncomingPacketHandler<int>(PacketName.RequestUserAppList.ToString(), ReceiveRequestUserAppList);
            NetworkComms.AppendGlobalIncomingPacketHandler<long>(PacketName.RequestAppFile.ToString(), ReceiveRequestAppFile);
            NetworkComms.AppendGlobalIncomingPacketHandler<int>(PacketName.RequestAllAppList.ToString(), ReceiveRequestAllAppList);
            NetworkComms.AppendGlobalIncomingPacketHandler<long>(PacketName.PostEnableApp.ToString(), ReceivePostEnableApp);
            NetworkComms.AppendGlobalIncomingPacketHandler<long>(PacketName.PostDisableApp.ToString(), ReceivePostDisableApp);
        }

        #region Packet Handlers
        protected void ReceivePostEnableApp(PacketHeader header, Connection connection, long appId)
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

            if (accountManager.Administrator(connection))
            {
                moduleRepository.EnableService(appId);

                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUpdateAccountInformation.ToString(), GenericResponse.Success);
            }
        }

        protected void ReceivePostDisableApp(PacketHeader header, Connection connection, long appId)
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

            if (accountManager.Administrator(connection))
            {
                moduleRepository.DisableService(appId);

                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUpdateAccountInformation.ToString(), GenericResponse.Success);
            }
        }

        protected void ReceiveRequestAppFile(PacketHeader header, Connection connection, long moduleId)
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

            Module module = moduleRepository.Get(moduleId);

            string filePath = module.path;

            byte[] fileData = File.ReadAllBytes(filePath);

            AppFileData appData = new AppFileData(module.id, filePath, fileData);

            TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                PacketName.ReAppFile.ToString(), appData);
        }

        protected void ReceiveRequestServiceLog(PacketHeader header, Connection connection, ServiceLogRequest request)
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

            List<ServiceLog> logs = serviceManager.RetrieveLogs(request.id, request.dateLimit);

            if (logs.Count > 0)
            {
                List<ServiceLogItem> serviceLogItems = new List<ServiceLogItem>();

                foreach (ServiceLog log in logs)
                {
                    serviceLogItems.Add(new ServiceLogItem(log.id, log.serviceId, log.text, log.date));
                }

                ServiceLogList serviceLogList = new ServiceLogList(serviceLogItems.ToArray());

                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    Networking.Data.PacketName.ReServiceLog.ToString(), serviceLogList);
            }
        }

        protected void ReceiveRequestUserAppList(PacketHeader header, Connection connection, int code)
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

            long accountId = accountManager.AuthorizedAccountId(connection);

            if (accountId != -1)
            {
                List<ModuleInfo> moduleInfoList = new List<ModuleInfo>();

                List<Repository.Entities.Group> userGroupAccess = groupRepository.RetrieveAccountAccess(accountId);

                foreach (Module service in moduleRepository.RetrieveEnabledApps())
                {
                    List<Repository.Entities.Group> appGroupAccess = groupRepository.RetrieveAppAccess(service.id);

                    bool foundGroupInCommon = false;
                    foreach (Repository.Entities.Group appGroup in appGroupAccess)
                    {
                        // One of the groups in appGroupAccess needs to exist in userGroupAccess.

                        foreach (Repository.Entities.Group userGroup in userGroupAccess)
                        {
                            if (appGroup.id == userGroup.id)
                            {
                                foundGroupInCommon = true;
                                break;
                            }
                        }
                    }

                    if (foundGroupInCommon)
                    {
                        moduleInfoList.Add(new ModuleInfo(service.id, service.type, service.name, service.version, service.path, service.enabled));
                    }
                }

                // groupRepository

                ModuleList moduleList = new ModuleList(moduleInfoList.ToArray());

                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    Networking.Data.PacketName.ReModuleList.ToString(), moduleList);
            }
        }

        protected void ReceiveRequestAllAppList(PacketHeader header, Connection connection, int code)
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

            long accountId = accountManager.AuthorizedAccountId(connection);

            if (accountId != -1)
            {
                List<ModuleInfo> moduleInfoList = new List<ModuleInfo>();

                foreach (Module service in moduleRepository.RetrieveApps())
                {
                    moduleInfoList.Add(new ModuleInfo(service.id, service.type, service.name, service.version, service.path, service.enabled));
                }

                ModuleList moduleList = new ModuleList(moduleInfoList.ToArray());

                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    Networking.Data.PacketName.ReAllAppList.ToString(), moduleList);
            }
        }

        protected void ReceiveModuleListRequest(PacketHeader header, Connection connection, int code)
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

            SendModuleList(connection);
        }

        protected void ReceiveServiceCommand(PacketHeader header, Connection connection, ServiceCommand command)
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

            string result = serviceManager.ExecuteServiceCommand(command.serviceId, command.value);

            ServiceCommand commandResult = new ServiceCommand(command.serviceId, result);

            TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                Networking.Data.PacketName.ReServiceResponse.ToString(), commandResult);
        }

        protected void ReceiveDeleteService(PacketHeader header, Connection connection, long serviceId)
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

            serviceManager.DeleteService(serviceId);

            SendModuleList(connection);
        }

        protected void ReceiveEnableService(PacketHeader header, Connection connection, long serviceId)
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

            serviceManager.EnableService(serviceId);

            SendModuleList(connection);
        }

        protected void ReceiveDisableService(PacketHeader header, Connection connection, long serviceId)
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

            serviceManager.DisableService(serviceId);

            SendModuleList(connection);
        }
        #endregion

        /// <summary>
        /// Retrieve all services and send to connection.
        /// </summary>
        /// <param name="connection"></param>
        public void SendModuleList(Connection connection)
        {
            List<ModuleInfo> moduleInfoList = new List<ModuleInfo>();

            foreach (Module service in moduleRepository.RetrieveServices())
            {
                moduleInfoList.Add(new ModuleInfo(service.id, service.type, service.name, service.version, service.path, service.enabled));
            }

            ModuleList moduleList = new ModuleList(moduleInfoList.ToArray());

            TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                PacketName.ReModuleList.ToString(), moduleList);
        }

        /// <summary>
        /// Load service from disk.
        /// </summary>
        /// <param name="filePath"></param>
        public void RegisterModule(string filePath)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (baseDirectory.Length == 0)
            {
                baseDirectory = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            }

            string fullPath = Path.Combine(baseDirectory, filePath);

            if (File.Exists(filePath))
            {
                ModuleHeader pluginHeader = ModuleHeaderReader.AppHeader(
                    baseDirectory, fullPath);

                ModuleHeader serviceHeader = ModuleHeaderReader.ServiceHeader(
                    baseDirectory, fullPath);

                if (pluginHeader != null)
                {
                    Module module = new Module();
                    module.enabled = 0;
                    module.type = (int)ModuleType.App;
                    module.path = filePath;
                    module.name = pluginHeader.Name;
                    module.version = pluginHeader.Version;

                    moduleRepository.Add(module);
                }

                if (serviceHeader != null)
                {
                    Module module = new Module();
                    module.enabled = 0;
                    module.type = (int)ModuleType.Service;
                    module.path = filePath;
                    module.name = serviceHeader.Name;
                    module.version = serviceHeader.Version;

                    moduleRepository.Add(module);

                    serviceManager.LoadServices();
                } 
            }
            else
            {
                // No such file
            }
        }
    }
}
