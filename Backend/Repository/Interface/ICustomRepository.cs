using System;
using System.Collections.Generic;

namespace Repository.Interface
{
    /// <summary>
    /// This interface is used to allow service developers to write and read data.
    /// </summary>
    public interface ICustomRepository
    {
        /// <summary>
        /// Run custom database query.
        /// </summary>
        /// <param name="query">SQL Query.</param>
        /// <param name="commands">Dictionary with string identifying the label in query and object the actual value to insert.</param>
        void ExecuteQuery(string query, Dictionary<string, object> commands);

        /// <summary>
        /// Create a new database table.
        /// </summary>
        /// <param name="name">The table name.</param>
        /// <param name="columns">The columns to add. Name and data type.</param>
        void CreateTable(string name, Dictionary<string, Type> columns);

        /// <summary>
        /// Read from database and return result.
        /// </summary>
        /// <param name="query">SQL Query.</param>
        /// <returns>Dictionary containing the column name and sql query results.</returns>
        Dictionary<string, string>[] Read(string query);
    }
}
