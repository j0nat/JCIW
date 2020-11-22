using Repository.Entities;
using System.Collections.Generic;

namespace Repository.Interface
{
    /// <summary>
    /// This interface is used to read and write group data.
    /// </summary>
    public interface IGroupRepository
    {
        /// <summary>
        /// Give an app access to a specific group.
        /// </summary>
        /// <param name="appId">The app module identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        void AddAppAccess(long appId, long groupId);

        /// <summary>
        /// Remove an app's access to a specific group.
        /// </summary>
        /// <param name="appId">The app module identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        void RemoveAppAccess(long appId, long groupId);

        /// <summary>
        /// Check to see if an app has access to a specific group.
        /// </summary>
        /// <param name="appId">The app module identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>True if app has access to group and false if not.</returns>
        bool AppAccessExists(long appId, long groupId);

        /// <summary>
        /// Create a new group.
        /// </summary>
        /// <param name="name">Name of the group.</param>
        /// <param name="description">Group descrition.</param>
        void AddGroup(string name, string description);

        /// <summary>
        /// Create a new group.
        /// </summary>
        /// <param name="id">The group identifier.</param>
        /// <param name="name">Name of the group.</param>
        /// <param name="description">Group descrition.</param>
        void AddGroup(int id, string name, string description);

        /// <summary>
        /// Remove a group.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        void DeleteGroup(long groupId);

        /// <summary>
        /// Delete account access.
        /// </summary>
        /// <param name="groupId">The account identifier.</param>
        void DeleteAllAccountAccess(long accountId);

        /// <summary>
        /// Retrieve all groups.
        /// </summary>
        /// <returns><see cref="Group"/> list.</returns>
        List<Group> RetrieveAll();

        /// <summary>
        /// Retrieve all account groups.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns><see cref="Group"/> list.</returns>
        List<Group> RetrieveAccountAccess(long accountId);

        /// <summary>
        /// Retrieve all app groups.
        /// </summary>
        /// <param name="appId">The app module identifier.</param>
        /// <returns><see cref="Group"/> list.</returns>
        List<Group> RetrieveAppAccess(long appId);

        /// <summary>
        /// Checks to see if a group exists by group id.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>True if group exists False if not.</returns>
        bool Exists(long groupId);

        /// <summary>
        /// Checks to see if a account is a member of a specific group.
        /// </summary>
        /// <param name="id">The group identifier.</param>
        /// <param name="accountId">The account identifier.</param>
        /// <returns>True if account has membership. False if not.</returns>
        bool Contains(long id, long accountId);

        /// <summary>
        /// Give account group access.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        void AddAccess(long accountId, long groupId);

        /// <summary>
        /// Remove account group access.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        void RemoveAccess(long accountId, long groupId);
    }
}
