using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using NetworkCommsDotNet.Tools;
using Networking.Data;
using Networking.Data.Packets;
using Networking.Data.ResponseCodes;

using Repository.Interface;
using System;
using System.Collections.Generic;

namespace Server.PacketHandlers
{
    /// <summary>
    /// This class handles account functions.
    /// </summary>
    public class AccountManager
    {
        private readonly Dictionary<ShortGuid, long> authorizedAccounts;
        private readonly IAccountRepository accountRepository;
        private readonly IGroupRepository groupRepository;

        /// <summary>
        /// Create <see cref="AccountManager"/>.
        /// </summary>
        /// <param name="accountRepository"><see cref="IAccountRepository"/> implementation.</param>
        /// <param name="groupRepository"><see cref="IGroupRepository"/> implementation.</param>
        public AccountManager(IAccountRepository accountRepository, IGroupRepository groupRepository)
        {
            this.authorizedAccounts = new Dictionary<ShortGuid, long>();
            this.groupRepository = groupRepository;
            this.accountRepository = accountRepository;

            NetworkComms.AppendGlobalIncomingPacketHandler<LoginRequest>(PacketName.RequestLogin.ToString(), LoginRequest);
            NetworkComms.AppendGlobalIncomingPacketHandler<int>(PacketName.RequestAccountList.ToString(), RequestAccountList);
            NetworkComms.AppendGlobalIncomingPacketHandler<RegisterRequest>(PacketName.RequestRegisterAccount.ToString(), RegisterRequest);
            NetworkComms.AppendGlobalIncomingPacketHandler<long>(PacketName.RequestUserGroupList.ToString(), RequestUserGroupList);
            NetworkComms.AppendGlobalIncomingPacketHandler<PasswordChangeRequest>(PacketName.RequestUpdatePassword.ToString(), ReceivePasswordChangeRequest);
            NetworkComms.AppendGlobalIncomingPacketHandler<GroupRequest>(PacketName.RequestDeleteGroupFromUser.ToString(), ReceiveGroupRemoveFromUserRequest);
            NetworkComms.AppendGlobalIncomingPacketHandler<Account>(PacketName.RequestUpdateAccountInformation.ToString(), ReceiveUserInformationUpdateRequest);
            NetworkComms.AppendGlobalIncomingPacketHandler<int>(PacketName.RequestGroupList.ToString(), ReceiveRequestGroupList);
            NetworkComms.AppendGlobalIncomingPacketHandler<GroupRequest>(PacketName.RequestAddGroupToUser.ToString(), ReceiveRequestAddGroupToUser);
            NetworkComms.AppendGlobalIncomingPacketHandler<long>(PacketName.RequestDeleteAccount.ToString(), ReceiveRequestDeleteAccount);
            NetworkComms.AppendGlobalIncomingPacketHandler<long>(PacketName.RequestDeleteGroup.ToString(), ReceiveDeleteGroup);
            NetworkComms.AppendGlobalIncomingPacketHandler<Group>(PacketName.RequestAddGroup.ToString(), ReceiveAddGroup);
            NetworkComms.AppendGlobalIncomingPacketHandler<long>(PacketName.RequestAppGroupList.ToString(), ReceiveRequestAppGroupList);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>(PacketName.RequestSessionVerification.ToString(), ReceiveSessionVerification);
            NetworkComms.AppendGlobalIncomingPacketHandler<GroupRequest>(PacketName.PostAddGroupToApp.ToString(), ReceivePostAddGroupToApp);
            NetworkComms.AppendGlobalIncomingPacketHandler<GroupRequest>(PacketName.PostRemoveGroupFromApp.ToString(), ReceivePostRemoveGroupFromApp);

            // Check if administrator group exists (a requirement)
            if (!groupRepository.Exists(0))
            {
                // Administrator group should have id = 0
                groupRepository.AddGroup(0, "administrator", "Default administrator group");
            }
        }

        #region Packet handlers
        protected void ReceiveSessionVerification(PacketHeader header, Connection connection, string session)
        {
            long id = accountRepository.GetSessionAccountId(session);

            if(id == -1)
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(PacketName.ReSessionVerificationResult.ToString(), GenericResponse.Fail);
            }
            else
            {
                lock (authorizedAccounts)
                {
                    if (authorizedAccounts.ContainsKey(connection.ConnectionInfo.NetworkIdentifier))
                    {
                        authorizedAccounts[connection.ConnectionInfo.NetworkIdentifier] = id;
                    }
                    else
                    {
                        authorizedAccounts.Add(connection.ConnectionInfo.NetworkIdentifier, id);
                    }
                }

                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(PacketName.ReSessionVerificationResult.ToString(), GenericResponse.Success);
            }
        }

        protected void ReceivePostAddGroupToApp(PacketHeader header, Connection connection, GroupRequest request)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            if (Administrator(connection))
            {
                groupRepository.AddAppAccess(request.accountId, request.groupId);
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUpdateAccountInformation.ToString(), GenericResponse.Success);
            }
        }

        protected void ReceivePostRemoveGroupFromApp(PacketHeader header, Connection connection, GroupRequest request)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            if (Administrator(connection))
            {
                groupRepository.RemoveAppAccess(request.accountId, request.groupId);
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUpdateAccountInformation.ToString(), GenericResponse.Success);
            }
        }

        protected void ReceiveRequestAppGroupList(PacketHeader header, Connection connection, long appId)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            if (Administrator(connection))
            {
                List<Group> sendUserGroups = new List<Group>();

                List<Repository.Entities.Group> userGroupAccess = groupRepository.RetrieveAppAccess(appId);

                foreach (Repository.Entities.Group access in userGroupAccess)
                {
                    sendUserGroups.Add(new Group(access.id, access.name, access.description));
                }

                TCPConnection.GetConnection(connection.ConnectionInfo).
                    SendObject(PacketName.ReAppGroupList.ToString(), new GroupList(sendUserGroups.ToArray()));
            }
        }

        protected void ReceiveDeleteGroup(PacketHeader header, Connection connection, long groupId)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            if (Administrator(connection))
            {
                groupRepository.DeleteGroup(groupId);

                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReDeleteGroup.ToString(), GenericResponse.Success);
            }
        }

        protected void ReceiveAddGroup(PacketHeader header, Connection connection, Group group)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            if (Administrator(connection))
            {
                groupRepository.AddGroup(group.name, group.description);

                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReAddGroup.ToString(), GenericResponse.Success);
            }
        }

        protected void ReceiveRequestDeleteAccount(PacketHeader header, Connection connection, long accountId)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            if (Administrator(connection))
            {
                groupRepository.DeleteAllAccountAccess(accountId);
                accountRepository.DeleteUser(accountId);

                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReDeleteAccount.ToString(), GenericResponse.Success);
            }
        }

        protected void ReceiveRequestAddGroupToUser(PacketHeader header, Connection connection, GroupRequest request)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            if (Administrator(connection))
            {
                groupRepository.AddAccess(request.accountId, request.groupId);

                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReAddGroupToUser.ToString(), GenericResponse.Success);
            }
        }

        protected void ReceiveRequestGroupList(PacketHeader header, Connection connection, int code)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            if (Administrator(connection))
            {
                List<Group> sendUserGroups = new List<Group>();

                List<Repository.Entities.Group> userGroupAccess = groupRepository.RetrieveAll();

                foreach (Repository.Entities.Group access in userGroupAccess)
                {
                    sendUserGroups.Add(new Group(access.id, access.name, access.description));
                }

                TCPConnection.GetConnection(connection.ConnectionInfo).
                    SendObject(PacketName.ReUserGroupList.ToString(), new GroupList(sendUserGroups.ToArray()));
            }
        }
        
        protected void ReceivePasswordChangeRequest(PacketHeader header, Connection connection, PasswordChangeRequest request)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            if (Administrator(connection))
            {
                accountRepository.UpdatePassword(request.accountId, Security.PasswordHasher.HashPassword(request.password));

                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUpdatePassword.ToString(), GenericResponse.Success);
            }
        }

        protected void ReceiveGroupRemoveFromUserRequest(PacketHeader header, Connection connection, GroupRequest request)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            if (Administrator(connection))
            {
                groupRepository.RemoveAccess(request.accountId, request.groupId);

                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReDeleteGroupFromUser.ToString(), GenericResponse.Success);
            }
        }

        protected void ReceiveUserInformationUpdateRequest(PacketHeader header, Connection connection, Account request)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            if (Administrator(connection))
            {
                accountRepository.Update(request.id, request.username, request.firstname, request.lastname);

                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUpdateAccountInformation.ToString(), GenericResponse.Success);
            }
        }

        protected void RequestUserGroupList(PacketHeader header, Connection connection, long accountId)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            if (accountId != -1)
            {
                if (Administrator(connection))
                {
                    List<Group> sendUserGroups = new List<Group>();

                    List<Repository.Entities.Group> userGroupAccess = groupRepository.RetrieveAccountAccess(accountId);

                    foreach (Repository.Entities.Group access in userGroupAccess)
                    {
                        sendUserGroups.Add(new Group(access.id, access.name, access.description));
                    }

                    TCPConnection.GetConnection(connection.ConnectionInfo).
                        SendObject(PacketName.ReUserGroupList.ToString(), new GroupList(sendUserGroups.ToArray()));
                }
            }
            else
            {
                if (authorizedAccounts.ContainsKey(connection.ConnectionInfo.NetworkIdentifier))
                {
                    long userAccountId = authorizedAccounts[connection.ConnectionInfo.NetworkIdentifier];

                    List<Group> sendUserGroups = new List<Group>();

                    List<Repository.Entities.Group> userGroupAccess = groupRepository.RetrieveAccountAccess(userAccountId);

                    foreach (Repository.Entities.Group access in userGroupAccess)
                    {
                        sendUserGroups.Add(new Group(access.id, access.name, access.description));
                    }

                    TCPConnection.GetConnection(connection.ConnectionInfo).
                        SendObject(PacketName.ReUserGroupList.ToString(), new GroupList(sendUserGroups.ToArray()));
                }
            }
        }

        protected void RequestAccountList(PacketHeader header, Connection connection, int code)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            if (Administrator(connection))
            {
                List<Account> sendAccounts = new List<Account>();
                List<Repository.Entities.Account> accounts = accountRepository.GetAll();

                foreach (Repository.Entities.Account account in accounts)
                {
                    sendAccounts.Add(new Account(account.id, account.username, account.firstname, account.lastname));
                }

                TCPConnection.GetConnection(connection.ConnectionInfo).
                    SendObject(PacketName.ReAccountList.ToString(), sendAccounts.ToArray());
            }
        }

        protected void RegisterRequest(PacketHeader header, Connection connection, RegisterRequest account)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            if (Administrator(connection))
            {
                RegisterResponse response = RegisterResponse.Failure;

                if (account.username.Length > 3 && account.password.Length > 3)
                {
                    if (accountRepository.UsernameExist(account.username))
                    {
                        response = RegisterResponse.UsernameTaken;
                    }
                    else
                    {
                        accountRepository.Add(account.username, Security.PasswordHasher.HashPassword(account.password), account.firstname, account.lastname);

                        response = RegisterResponse.Success;
                    }
                }

                TCPConnection.GetConnection(connection.ConnectionInfo).
                    SendObject(PacketName.ReRegisterAccount.ToString(), (int)response);
            }
        }

        protected void LoginRequest(PacketHeader header, Connection connection, LoginRequest account)
        {
            string sessionId = Guid.NewGuid().ToString();
            long accountId;
            string passwordHash;
            if (accountRepository.AccountPasswordHash(account.username, out accountId, out passwordHash))
            {
                if (Security.PasswordHasher.VerifyHashedPassword(passwordHash, account.password))
                {
                    if (account.admin)
                    {
                        if (groupRepository.Contains(0, accountId))
                        {
                            // Authorized...

                            accountRepository.SetSession(accountId, sessionId);

                            lock (authorizedAccounts)
                            {
                                if (authorizedAccounts.ContainsKey(connection.ConnectionInfo.NetworkIdentifier))
                                {
                                    authorizedAccounts[connection.ConnectionInfo.NetworkIdentifier] = accountId;
                                }
                                else
                                {
                                    authorizedAccounts.Add(connection.ConnectionInfo.NetworkIdentifier, accountId);
                                }
                            }

                            TCPConnection.GetConnection(connection.ConnectionInfo).
                                SendObject(PacketName.ReLoginResult.ToString(),
                                (int)LoginResponse.Success);
                            TCPConnection.GetConnection(connection.ConnectionInfo).
                                SendObject(PacketName.ReSessionId.ToString(), sessionId);
                        }
                        else
                        {
                            TCPConnection.GetConnection(connection.ConnectionInfo).
                                SendObject(PacketName.ReLoginResult.ToString(),
                                (int)LoginResponse.NoAdminAccess);
                        }
                    }
                    else
                    {
                        // Authorized...
                        accountRepository.SetSession(accountId, sessionId);

                        lock (authorizedAccounts)
                        {
                            if (authorizedAccounts.ContainsKey(connection.ConnectionInfo.NetworkIdentifier))
                            {
                                authorizedAccounts[connection.ConnectionInfo.NetworkIdentifier] = accountId;
                            }
                            else
                            {
                                authorizedAccounts.Add(connection.ConnectionInfo.NetworkIdentifier, accountId);
                            }
                        }

                        TCPConnection.GetConnection(connection.ConnectionInfo).
                            SendObject(PacketName.ReLoginResult.ToString(), (int)LoginResponse.Success);
                        TCPConnection.GetConnection(connection.ConnectionInfo).
                            SendObject(PacketName.ReSessionId.ToString(), sessionId);
                    }
                }
                else
                {
                    // Wrong password
                    TCPConnection.GetConnection(connection.ConnectionInfo).
                        SendObject(PacketName.ReLoginResult.ToString(),
                        (int)LoginResponse.WrongUsernamePassword);
                }
            }
            else
            {
                // Account doesn't exist...
                TCPConnection.GetConnection(connection.ConnectionInfo).
                    SendObject(PacketName.ReLoginResult.ToString(),
                    (int)LoginResponse.WrongUsernamePassword);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves all of an accounts groups.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns><see cref="Group"/> array.</returns>
        public Group[] AccountGroups(long accountId)
        {
            List<Group> sendUserGroups = new List<Group>();

            List<Repository.Entities.Group> userGroupAccess = groupRepository.RetrieveAccountAccess(accountId);

            foreach (Repository.Entities.Group access in userGroupAccess)
            {
                sendUserGroups.Add(new Group(access.id, access.name, access.description));
            }

            return sendUserGroups.ToArray();
        }

        /// <summary>
        /// Remove user from authorizedAccounts when user disconnects.
        /// </summary>
        /// <param name="connection"></param>
        public void UserDisconnected(Connection connection)
        {
            lock (authorizedAccounts)
            {
                if (authorizedAccounts.ContainsKey(connection.ConnectionInfo.NetworkIdentifier))
                {
                    authorizedAccounts.Remove(connection.ConnectionInfo.NetworkIdentifier);
                }
            }
        }

        /// <summary>
        /// Returns the connections account ID.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public long AuthorizedAccountId(Connection connection)
        {
            long accountId = -1;

            if (authorizedAccounts.ContainsKey(connection.ConnectionInfo.NetworkIdentifier))
            {
                accountId = authorizedAccounts[connection.ConnectionInfo.NetworkIdentifier];
            }

            return accountId;
        }

        /// <summary>
        /// Create a group.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public void CreateGroup(string name, string description)
        {
            groupRepository.AddGroup(name, description);
        }

        /// <summary>
        /// Get the account ID from guid.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public long AuthorizedAccountId(ShortGuid guid)
        {
            long accountId = -1;

            if (authorizedAccounts.ContainsKey(guid))
            {
                accountId = authorizedAccounts[guid];
            }

            return accountId;
        }

        public Account AuthorizedAccount(long accountId)
        {
            Account account = null;

            Repository.Entities.Account entityAccount = accountRepository.Get(accountId);

            if (entityAccount != null)
            {
                account = new Account(entityAccount.id, entityAccount.username, entityAccount.firstname, entityAccount.lastname);
            }

            return account;
        }

        /// <summary>
        /// Check if connection is administator.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public bool Administrator(Connection connection)
        {
            bool administrator = false;

            lock (authorizedAccounts)
            {
                if (authorizedAccounts.ContainsKey(connection.ConnectionInfo.NetworkIdentifier))
                {
                    long accountId = authorizedAccounts[connection.ConnectionInfo.NetworkIdentifier];

                    // Admin id = 0
                    if (groupRepository.Contains(0, accountId))
                    {
                        administrator = true;
                    }
                }
            }

            return administrator;
        }

        /// <summary>
        /// Check if connection is authorized.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public bool Authorized(Connection connection)
        {
            bool authorized = false;

            lock (authorizedAccounts)
            {
                if (authorizedAccounts.ContainsKey(connection.ConnectionInfo.NetworkIdentifier))
                {
                    authorized = true;
                }
            }

            return authorized;
        }
    }
}
