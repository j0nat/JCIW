using Networking.Data.Packets;
using System;

namespace JCIW.Data.Interfaces
{
    /// <summary>
    /// This interface is used for calling server methods externally (for example in a service).
    /// </summary>
    public interface IServerFunctions
    {
        /// <summary>
        /// Retrieve user ID from connection <see cref="Guid"/>.
        /// </summary>
        /// <param name="connectionGuid">The connection identifier <see cref="Guid"/>.</param>
        /// <returns>The user ID</returns>
        long AccountId(Guid connectionGuid);

        /// <summary>
        /// Retrieve <see cref="Account"/>.
        /// </summary>
        /// <param name="accountId">The user ID</param>
        /// <returns>The users <see cref="Account"/>.</returns>
        Account Account(long accountId);

        /// <summary>
        /// Create a new group.
        /// </summary>
        /// <param name="name">The name of the new group.</param>
        /// <param name="description">The description of the new group.</param>
        void CreateGroup(string name, string description);

        /// <summary>
        /// Retrieve user groups from connection <see cref="Guid"/>.
        /// </summary>
        /// <param name="connectionGuid">The connection identifier <see cref="Guid"/>.</param>
        /// <returns>All the users groups.</returns>
        Group[] UserGroups(Guid connectionGuid);
    }
}
