using Repository.Entities;
using System;
using System.Collections.Generic;

namespace Repository.Interface
{
    /// <summary>
    /// This interface is used to read and write service log data.
    /// </summary>
    public interface IServiceLogRepository
    {
        /// <summary>
        /// Retrieve log entries by service ID
        /// </summary>
        /// <param name="serviceId">The service log identifier.</param>
        /// <returns>List of <see cref="ServiceLog"/>.</returns>
        List<ServiceLog> RetrieveLog(Int64 serviceId);

        /// <summary>
        /// Retrieve log entries by service ID and limit by date.
        /// </summary>
        /// <param name="serviceId">The service log identifier.</param>
        /// <returns>List of <see cref="ServiceLog"/>.</returns>
        List<ServiceLog> RetrieveLog(Int64 serviceId, long limitDate);

        /// <summary>
        /// Add log entry.
        /// </summary>
        /// <param name="serviceId">The service log identifier.</param>
        /// <param name="text">The log entry to add.</param>
        void AddLog(Int64 serviceId, string text);
    }
}
