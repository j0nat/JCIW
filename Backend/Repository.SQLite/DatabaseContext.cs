using System.Data.Common;
using System.Data.SQLite;
using System.Configuration;

namespace Repository.SQLite
{
    /// <summary>
    /// This is the SQLite database context. Used to establish connection to database file.
    /// </summary>
    public static class DatabaseContext
    {
        /// <summary>
        /// Establish and return SQLite file database connection.
        /// </summary>
        /// <returns>The <see cref="DbConnection"/></returns>
        public static DbConnection GetConnection()
        {
            SQLiteConnection connection =
                 new SQLiteConnection(ConfigurationManager.AppSettings["SQLiteConnectionString"].ToString());

            return connection;
        }
    }
}
