using Repository.Entities;
using System;
using System.Collections.Generic;

namespace Repository.Interface
{
    /// <summary>
    /// This interface is used to handle account specific storage and reading.
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="username">The login username.</param>
        /// <param name="password">The login password.</param>
        /// <param name="firstname">Personal first name.</param>
        /// <param name="lastname">Personal last name.</param>
        void Add(string username, string password, string firstname, string lastname);

        /// <summary>
        /// Retrieve <see cref="Account"/> based on id.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns>If found <see cref="Account"/> if not found returns null.</returns>
        Account Get(long accountId);

        /// <summary>
        /// Change account information.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="username">The login username.</param>
        /// <param name="firstname">Personal first name.</param>
        /// <param name="lastname">Personal last name.</param>
        void Update(long accountId, string username, string firstname, string lastname);

        /// <summary>
        /// Update account password.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="password">The login password.</param>
        void UpdatePassword(long accountId, string password);

        /// <summary>
        /// Delete account based on id.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        void DeleteUser(long accountId);

        /// <summary>
        /// Retrieve list of all registered <see cref="Account"/>'s.
        /// </summary>
        /// <returns>List of <see cref="Account"/>.</returns>
        List<Account> GetAll();

        /// <summary>
        /// Return account ID based on username.
        /// </summary>
        /// <param name="username">The login username.</param>
        /// <returns>The account identifier.</returns>
        long GetId(string username);

        /// <summary>
        /// Retrieve account id and password hash.
        /// </summary>
        /// <param name="username">The login username.</param>
        /// <param name="accountId">The found account identifier.</param>
        /// <param name="passwordHash">The found password hash.</param>
        /// <returns>True if account was found and false if not.</returns>
        bool AccountPasswordHash(string username, out long accountId, out string passwordHash);

        /// <summary>
        /// Check if account exists based on username.
        /// </summary>
        /// <param name="username">The login username.</param>
        /// <returns>Whether or not account with the username exists.</returns>
        bool UsernameExist(string username);

        /// <summary>
        /// Set session id on account.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="sessionId">The account login session id.</param>
        void SetSession(Int64 accountId, string sessionId);

        /// <summary>
        /// Returns the account ID of a specific session
        /// </summary>
        /// <param name="sessionId">The account login session id.</param>
        /// <returns>The id of the account found. -1 if not found.</returns>
        long GetSessionAccountId(string sessionId);
    }
}
