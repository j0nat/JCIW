using System.Net;
using NetworkCommsDotNet;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Connections;
using Server.PacketHandlers;
using Repository.Interface;

namespace Server
{
    /// <summary>
    /// This is the main class for the JCIW server networking.
    /// </summary>
    public class ServerManager
    {
        private readonly int port = 3921;
        private readonly ServiceManager serviceManager;
        private readonly ModuleManager moduleManager;
        private readonly AccountManager accountManager;
        private readonly IncomingFileManager incomingFileManager;

        /// <summary>
        /// Creates <see cref="ServerManager"/>.
        /// </summary>
        public ServerManager()
        {
            IAccountRepository accountRepository = RepositoryFactory.AccountRepository();
            IModuleRepository moduleRepository = RepositoryFactory.ModuleRepository();
            IGroupRepository groupRepository = RepositoryFactory.GroupRepository();

            accountManager = new AccountManager(accountRepository, groupRepository);
            serviceManager = new ServiceManager(moduleRepository, accountManager);
            moduleManager = new ModuleManager(accountManager, moduleRepository, serviceManager, groupRepository);

            incomingFileManager = new IncomingFileManager(moduleManager, accountManager);

            NetworkComms.AppendGlobalConnectionCloseHandler(HandleConnectionClosed);
        }

        /// <summary>
        /// Start listening on port.
        /// </summary>
        public void Start()
        {
            NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions(DPSManager.GetDataSerializer<ProtobufSerializer>(),
                NetworkComms.DefaultSendReceiveOptions.DataProcessors, NetworkComms.DefaultSendReceiveOptions.Options);

            NetworkComms.Shutdown();

            Connection.StartListening(ConnectionType.TCP, new IPEndPoint(IPAddress.Any, port));
        }

        /// <summary>
        /// Called when a connection is disconnected.
        /// </summary>
        /// <param name="connection">The <see cref="Connection"/>.</param>
        private void HandleConnectionClosed(Connection connection)
        {
            accountManager.UserDisconnected(connection);
        }
    }
}
