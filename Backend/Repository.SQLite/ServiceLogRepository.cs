using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Entities;
using Repository.Interface;


namespace Repository.SQLite
{
    /*
    CREATE TABLE "servicelog" (
	    "id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	    "serviceid"	INTEGER,
	    "text"	TEXT,
	    "date"	INTEGER
    );
    */
    public class ServiceLogRepository : IServiceLogRepository
    {
        public void AddLog(long serviceId, string text)
        {
            long unixTimestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO servicelog VALUES(@0, @1, @2, @3)";

            command.Parameters.Add(new SQLiteParameter("@0", null));        // Null = autoincrement
            command.Parameters.Add(new SQLiteParameter("@1", serviceId));
            command.Parameters.Add(new SQLiteParameter("@2", text));
            command.Parameters.Add(new SQLiteParameter("@3", unixTimestamp));

            command.ExecuteNonQuery();
            connection.Close();
        }

        public List<ServiceLog> RetrieveLog(long serviceId)
        {
            List<ServiceLog> logs = new List<ServiceLog>();

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM servicelog WHERE serviceid=@0 ORDER BY date";

            command.Parameters.Add(new SQLiteParameter("@0", serviceId));

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ServiceLog log = new ServiceLog();

                log.id = Convert.ToInt64(reader["id"]);
                log.serviceId = Convert.ToInt64(reader["serviceid"]);
                log.text = Convert.ToString(reader["text"]);
                log.date = Convert.ToInt64(reader["date"]);

                logs.Add(log);
            }

            reader.Close();
            connection.Close();

            return logs;
        }

        public List<ServiceLog> RetrieveLog(long serviceId, long limitDate)
        {
            List<ServiceLog> logs = new List<ServiceLog>();

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM servicelog WHERE serviceid=@0 AND date > @1 ORDER BY date";

            command.Parameters.Add(new SQLiteParameter("@0", serviceId));
            command.Parameters.Add(new SQLiteParameter("@1", limitDate));

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ServiceLog log = new ServiceLog();

                log.id = Convert.ToInt64(reader["id"]);
                log.serviceId = Convert.ToInt64(reader["serviceid"]);
                log.text = Convert.ToString(reader["text"]);
                log.date = Convert.ToInt64(reader["date"]);

                logs.Add(log);
            }

            reader.Close();
            connection.Close();

            return logs;
        }
    }
}
