using JCIW.Data.Interfaces;
using Networking.Data.Packets;
using Server.PacketHandlers;
using System;

namespace Server
{
    /// <summary>
    /// This class implements <see cref="IServerFunctions"/>.
    /// </summary>
    public class ServerFunctions : IServerFunctions
    {
        private readonly AccountManager accountManager;

        public ServerFunctions(AccountManager accountManager)
        {
            this.accountManager = accountManager;
        }

        public Account Account(long accountId)
        {
            return accountManager.AuthorizedAccount(accountId);
        }

        public void CreateGroup(string name, string description)
        {
            accountManager.CreateGroup(name, description);
        }

        public Group[] UserGroups(Guid connectionGuid)
        {
            long authorizedAccountId = AccountId(connectionGuid);

            if (authorizedAccountId != -1)
            {
                return accountManager.AccountGroups(authorizedAccountId);
            }
            else
            {
                return null;
            }
        }

        public long AccountId(Guid connectionGuid)
        {
            return accountManager.AuthorizedAccountId(connectionGuid);
        }
    }
}