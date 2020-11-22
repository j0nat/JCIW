using Networking.Data.Packets;
using Networking.Data.ResponseCodes;

namespace Client_Admin
{
    /// <summary>
    /// This class contains all the events used to send incoming network packets.
    /// </summary>
    public static class NetworkingEvents
    {
        public delegate void LoginResult(LoginResponse response);
        public static event LoginResult LoginResultEvent;

        public delegate void DeleteAccountResult(GenericResponse response);
        public static event DeleteAccountResult DeleteAccountResultEvent;

        public delegate void PasswordChangeResult(GenericResponse response);
        public static event PasswordChangeResult PasswordChangeResultEvent;

        public delegate void AccountUpdatedResult(GenericResponse response);
        public static event AccountUpdatedResult AccountUpdatedResultEvent;

        public delegate void GroupAddedToUserResult(GenericResponse response);
        public static event GroupAddedToUserResult GroupAddedToUserResultEvent;

        public delegate void UserGroupDeletedResult(GenericResponse response);
        public static event UserGroupDeletedResult UserGroupDeletedResultEvent;

        public delegate void AccountListResult(Account[] accounts);
        public static event AccountListResult AccountListResultEvent;

        public delegate void GroupListResult(Group[] groups);
        public static event GroupListResult GroupListResultEvent;

        public delegate void AppGroupListResult(Group[] groups);
        public static event AppGroupListResult AppGroupListResultEvent;

        public delegate void RegisterResult(RegisterResponse response);
        public static event RegisterResult RegisterResultEvent;

        public delegate void ServiceCommandResult(ServiceCommand result);
        public static event ServiceCommandResult ServiceCommandResultEvent;

        public delegate void ModuleListReceived(ModuleList moduleList);
        public static event ModuleListReceived ModuleListReceivedEvent;

        public delegate void AppListReceived(ModuleList moduleList);
        public static event AppListReceived AppListReceivedEvent;

        public delegate void ServiceLogListReceived(ServiceLogList serviceLogList);
        public static event ServiceLogListReceived ServiceLogListReceivedEvent;

        public delegate void AddRemoveGroupReceived(GenericResponse response);
        public static event AddRemoveGroupReceived AddRemoveGroupReceivedEvent;

        /// <summary>
        /// Raises event AppGroupListResultEvent.
        /// </summary>
        /// <param name="groups"></param>
        public static void RaiseAppGroupListResultEvent(Group[] groups)
        {
            if (AppGroupListResultEvent != null)
            {
                AppGroupListResultEvent.Invoke(groups);
            }
        }

        /// <summary>
        /// Raises event DeleteAccountResultEvent.
        /// </summary>
        /// <param name="groups"></param>
        public static void RaiseDeleteAccountResultEvent(GenericResponse response)
        {
            if (DeleteAccountResultEvent != null)
            {
                DeleteAccountResultEvent.Invoke(response);
            }
        }

        /// <summary>
        /// Raises event AppListReceivedEvent.
        /// </summary>
        /// <param name="groups"></param>
        public static void RaiseAppListReceivedEvent(ModuleList moduleList)
        {
            if (AppListReceivedEvent != null)
            {
                AppListReceivedEvent.Invoke(moduleList);
            }
        }

        /// <summary>
        /// Raises event AddRemoveGroupReceivedEvent.
        /// </summary>
        /// <param name="groups"></param>
        public static void RaiseAddRemoveGroupReceivedEvent(GenericResponse response)
        {
            if (AddRemoveGroupReceivedEvent != null)
            {
                AddRemoveGroupReceivedEvent.Invoke(response);
            }
        }

        /// <summary>
        /// Raises event LoginResultEvent.
        /// </summary>
        /// <param name="groups"></param>
        public static void RaiseLoginResultEvent(LoginResponse response)
        {
            if (LoginResultEvent != null)
            {
                LoginResultEvent.Invoke(response);
            }
        }

        /// <summary>
        /// Raises event GroupAddedToUserResultEvent.
        /// </summary>
        /// <param name="groups"></param>
        public static void RaiseGroupAddedToUserResultEvent(GenericResponse response)
        {
            if (GroupAddedToUserResultEvent != null)
            {
                GroupAddedToUserResultEvent.Invoke(response);
            }
        }

        /// <summary>
        /// Raises event PasswordChangeResultEvent.
        /// </summary>
        /// <param name="groups"></param>
        public static void RaisePasswordChangeResultEvent(GenericResponse response)
        {
            if (PasswordChangeResultEvent != null)
            {
                PasswordChangeResultEvent.Invoke(response);
            }
        }

        /// <summary>
        /// Raises event UserGroupDeletedResultEvent.
        /// </summary>
        /// <param name="groups"></param>
        public static void RaiseUserGroupDeletedResultEvent(GenericResponse response)
        {
            if (UserGroupDeletedResultEvent != null)
            {
                UserGroupDeletedResultEvent.Invoke(response);
            }
        }

        /// <summary>
        /// Raises event AccountUpdatedResultEvent.
        /// </summary>
        /// <param name="groups"></param>
        public static void RaiseAccountUpdatedResultEvent(GenericResponse response)
        {
            if (AccountUpdatedResultEvent != null)
            {
                AccountUpdatedResultEvent.Invoke(response);
            }
        }

        /// <summary>
        /// Raises event AccountListResultEvent.
        /// </summary>
        /// <param name="groups"></param>
        public static void RaiseAccountListResultEvent(Account[] accounts)
        {
            if (AccountListResultEvent != null)
            {
                AccountListResultEvent.Invoke(accounts);
            }
        }

        /// <summary>
        /// Raises event GroupListResultEvent.
        /// </summary>
        /// <param name="groups"></param>
        public static void RaiseGroupListResultEvent(Group[] groups)
        {
            if (GroupListResultEvent != null)
            {
                GroupListResultEvent.Invoke(groups);
            }
        }

        /// <summary>
        /// Raises event RegisterResultEvent.
        /// </summary>
        /// <param name="groups"></param>
        public static void RaiseRegisterResultEvent(RegisterResponse response)
        {
            if (RegisterResultEvent != null)
            {
                RegisterResultEvent.Invoke(response);
            }
        }

        /// <summary>
        /// Raises event ServiceLogListReceivedEvent.
        /// </summary>
        /// <param name="groups"></param>
        public static void RaiseServiceLogListReceivedEvent(ServiceLogList result)
        {
            if (ServiceLogListReceivedEvent != null)
            {
                ServiceLogListReceivedEvent.Invoke(result);
            }
        }

        /// <summary>
        /// Raises event ServiceCommandResultEvent.
        /// </summary>
        /// <param name="groups"></param>
        public static void RaiseServiceCommandResultEvent(ServiceCommand result)
        {
            if (ServiceCommandResultEvent != null)
            {
                ServiceCommandResultEvent.Invoke(result);
            }
        }

        /// <summary>
        /// Raises event ModuleListReceivedEvent.
        /// </summary>
        /// <param name="groups"></param>
        public static void RaiseModuleListReceivedEvent(ModuleList moduleList)
        {
            if (ModuleListReceivedEvent != null)
            {
                ModuleListReceivedEvent.Invoke(moduleList);
            }
        }
    }
}
