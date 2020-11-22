using Networking.Data;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using JCIW.Module;
using Repository.Entities;
using Server.PacketHandlers;

namespace Server
{
    /// <summary>
    /// This class is used to load and unload services.
    /// </summary>
    public class ServiceManager
    {
        private readonly Dictionary<long, ModuleInstance> services;
        private readonly PluginNetworking pluginNetworking;
        private readonly IModuleRepository moduleRepository;
        private readonly CustomPacketDatabase customPacketDatabase;
        private readonly PacketManager packetManager;
        private readonly LogManager logManager;
        private readonly ServerFunctions serverFunctions;

        /// <summary>
        /// Creates a <see cref="ServiceManager"/>.
        /// </summary>
        /// <param name="moduleRepository">A <see cref="IModuleRepository"/> implementation.</param>
        /// <param name="accountManager">The <see cref="AccountManager"/>.</param>
        public ServiceManager(IModuleRepository moduleRepository, AccountManager accountManager)
        {
            this.logManager = new LogManager();
            this.services = new Dictionary<long, ModuleInstance>();
            this.packetManager = new PacketManager();
            this.pluginNetworking = new PluginNetworking(packetManager, accountManager);
            this.moduleRepository = moduleRepository;
            this.customPacketDatabase = new CustomPacketDatabase(RepositoryFactory.CustomRepository());
            this.serverFunctions = new ServerFunctions(accountManager);

            // Start services from database
            LoadServices();
        }

        /// <summary>
        /// Retrieve service logs.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="limitDate">Date to limit. Unix timestamp.</param>
        /// <returns>List of <see cref="ServiceLog"/>.</returns>
        public List<ServiceLog> RetrieveLogs(long serviceId, long limitDate)
        {
            if (limitDate == -1)
            {
                return logManager.Get(serviceId);
            }
            else
            {
                return logManager.Get(serviceId, limitDate);
            }
        }

        /// <summary>
        /// Enable service.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        public void EnableService(long serviceId)
        {
            if (services.ContainsKey(serviceId))
            {
                ModuleInstance service = services[serviceId];

                Module module = moduleRepository.Get(serviceId);

                if (module.type == (int)ModuleType.Service && module.enabled == 0)
                {
                    try
                    {
                        if (service.Load())
                        {
                            moduleRepository.EnableService(serviceId);
                        }
                    }
                    catch (Exception err)
                    {
                        logManager.Add(module.id, err.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Delete service.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        public void DeleteService(long serviceId)
        {
            if (services.ContainsKey(serviceId))
            {
                ModuleInstance service = services[serviceId];

                Module module = moduleRepository.Get(serviceId);

                if (module.type == (int)ModuleType.Service && module.enabled == 1)
                {
                    moduleRepository.DisableService(serviceId);
                    service.Unload();
                }

                moduleRepository.DeleteService(serviceId);
            }
            else
            {
                moduleRepository.DeleteService(serviceId);
            }
        }

        /// <summary>
        /// Disable service.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        public void DisableService(long serviceId)
        {
            if (services.ContainsKey(serviceId))
            {
                ModuleInstance service = services[serviceId];

                Module module = moduleRepository.Get(serviceId);

                if (module.type == (int)ModuleType.Service && module.enabled == 1)
                {
                    try
                    {
                        moduleRepository.DisableService(serviceId);
                        service.Unload();
                    }
                    catch (Exception err)
                    {
                        logManager.Add(module.id, err.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Send service command to active service.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <param name="command">The command to send.</param>
        /// <returns>The result from the service.</returns>
        public string ExecuteServiceCommand(long serviceId, string command)
        {
            string commandResult = "";

            if (services.ContainsKey(serviceId))
            {
                ModuleInstance service = services[serviceId];

                Module module = moduleRepository.Get(serviceId);

                if (module.type == (int)ModuleType.Service && module.enabled == 1)
                {
                    commandResult = service.ExecuteServiceCommand(command);
                }
                else
                {
                    commandResult = "Service is not enabled.";
                }
            }
            else
            {
                commandResult = "Service does not exist.";
            }

            return commandResult;
        }

        /// <summary>
        /// Load services from repository.
        /// </summary>
        public void LoadServices()
        {
            // Enabled is the only thing that should be different
            foreach (Module service in moduleRepository.RetrieveServices())
            {
                if (service.type == (int)ModuleType.Service)
                {
                    ModuleInstance moduleInstance;

                    if (!services.ContainsKey(service.id))
                    {
                        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                        if (baseDirectory.Length == 0)
                        {
                            baseDirectory = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                        }

                        string modulePath = Path.Combine(baseDirectory, service.path);

                        moduleInstance =
                            new ModuleInstance(modulePath,
                            "Module.Service",
                            new object[] { service.id, pluginNetworking, customPacketDatabase, packetManager, logManager, serverFunctions });

                        lock (services)
                        {
                            services.Add(service.id, moduleInstance);
                        }
                    }
                    else
                    {
                        moduleInstance = services[service.id];
                    }

                    if (service.enabled == 1)
                    {
                        if (moduleInstance.Load())
                        {
                            Console.WriteLine("Service " + service.name + " loaded.");
                        }
                        else
                        {
                            Console.WriteLine("Service " + service.name + " was unable to load.");
                        }
                    }
                }
            }
        }
    }
}
