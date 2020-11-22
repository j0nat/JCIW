using System;
using System.Configuration;
using Repository.Interface;
using Repository.SQLite;

namespace Server
{
    /// <summary>
    /// This class is used to create repository interface implementation based on AppSettings.
    /// </summary>
    public static class RepositoryFactory
    {
        /// <summary>
        /// Retrieve <see cref="ICustomRepository"/> based on AppSettings.
        /// </summary>
        /// <returns>The <see cref="ICustomRepository"/> implementation.</returns>
        public static ICustomRepository CustomRepository()
        {
            ICustomRepository customRepository = null;

            string repositoryType = ConfigurationManager.AppSettings["Repository"].ToString();

            switch (repositoryType)
            {
                case "SQLite":
                    customRepository = new CustomRepository();
                    break;
                default:
                    throw new ArgumentException("Invalid repository type");
            }

            return customRepository;
        }

        /// <summary>
        /// Retrieve <see cref="IGroupRepository"/> based on AppSettings.
        /// </summary>
        /// <returns>The <see cref="IGroupRepository"/> implementation.</returns>
        public static IGroupRepository GroupRepository()
        {
            IGroupRepository groupRepository = null;

            string repositoryType = ConfigurationManager.AppSettings["Repository"].ToString();

            switch (repositoryType)
            {
                case "SQLite":
                    groupRepository = new GroupRepository();
                    break;
                default:
                    throw new ArgumentException("Invalid repository type");
            }

            return groupRepository;
        }

        /// <summary>
        /// Retrieve <see cref="IModuleRepository"/> based on AppSettings.
        /// </summary>
        /// <returns>The <see cref="IModuleRepository"/> implementation.</returns>
        public static IModuleRepository ModuleRepository()
        {
            IModuleRepository customRepository = null;

            string repositoryType = ConfigurationManager.AppSettings["Repository"].ToString();

            switch (repositoryType)
            {
                case "SQLite":
                    customRepository = new ModuleRepository();
                    break;
                default:
                    throw new ArgumentException("Invalid repository type");
            }

            return customRepository;
        }

        /// <summary>
        /// Retrieve <see cref="IServiceLogRepository"/> based on AppSettings.
        /// </summary>
        /// <returns>The <see cref="IServiceLogRepository"/> implementation.</returns>
        public static IServiceLogRepository ServiceLogRepository()
        {
            IServiceLogRepository serviceLogRepository = null;

            string repositoryType = ConfigurationManager.AppSettings["Repository"].ToString();

            switch (repositoryType)
            {
                case "SQLite":
                    serviceLogRepository = new ServiceLogRepository();
                    break;
                default:
                    throw new ArgumentException("Invalid repository type");
            }

            return serviceLogRepository;
        }

        /// <summary>
        /// Retrieve <see cref="IAccountRepository"/> based on AppSettings.
        /// </summary>
        /// <returns>The <see cref="IAccountRepository"/> implementation.</returns>
        public static IAccountRepository AccountRepository()
        {
            IAccountRepository accountRepository = null;

            string repositoryType = ConfigurationManager.AppSettings["Repository"].ToString();

            switch (repositoryType)
            {
                case "SQLite":
                    accountRepository = new AccountRepository();
                break;
                default:
                    throw new ArgumentException("Invalid repository type");
            }

            return accountRepository;
        }
    }
}
