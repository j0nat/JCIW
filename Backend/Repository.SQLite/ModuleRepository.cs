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
    CREATE TABLE "module" (
        "id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
        "type"	INTEGER,
        "name"	TEXT,
        "version"	TEXT,
    	"path"	TEXT,
        "enabled"	INTEGER
    );
    */

    public class ModuleRepository : IModuleRepository
    {
        public void Add(Module module)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO module VALUES(@0, @1, @2, @3, @4, @5)";

            command.Parameters.Add(new SQLiteParameter("@0", null));        // Null = autoincrement
            command.Parameters.Add(new SQLiteParameter("@1", module.type));
            command.Parameters.Add(new SQLiteParameter("@2", module.name));
            command.Parameters.Add(new SQLiteParameter("@3", module.version));
            command.Parameters.Add(new SQLiteParameter("@4", module.path));
            command.Parameters.Add(new SQLiteParameter("@5", module.enabled));

            command.ExecuteNonQuery();
            connection.Close();
        }

        public void DeleteService(long id)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "DELETE FROM module WHERE id=@0";

            command.Parameters.Add(new SQLiteParameter("@0", id));

            command.ExecuteNonQuery();
            connection.Close();
        }

        public void DisableService(long id)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "UPDATE module SET enabled=0 WHERE id=@0";

            command.Parameters.Add(new SQLiteParameter("@0", id));

            command.ExecuteNonQuery();
            connection.Close();
        }

        public void EnableService(long id)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "UPDATE module SET enabled=1 WHERE id=@0";

            command.Parameters.Add(new SQLiteParameter("@0", id));

            command.ExecuteNonQuery();
            connection.Close();
        }

        public Module Get(long id)
        {
            Module module = null;

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM module WHERE id=@0";

            command.Parameters.Add(new SQLiteParameter("@0", id));

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                module = new Module();
                module.id = Convert.ToInt64(reader["id"]);
                module.name = Convert.ToString(reader["name"]);
                module.enabled = Convert.ToInt32(reader["enabled"]);
                module.path = Convert.ToString(reader["path"]);
                module.type = Convert.ToInt32(reader["type"]);
                module.version = Convert.ToString(reader["version"]);

                break;
            }

            reader.Close();
            connection.Close();

            return module;
        }

        public List<Module> RetrieveEnabledApps()
        {
            List<Module> modules = new List<Module>();

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM module WHERE type=1 AND enabled=1";

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Module module = new Module();
                module.id = Convert.ToInt64(reader["id"]);
                module.name = Convert.ToString(reader["name"]);
                module.enabled = Convert.ToInt32(reader["enabled"]);
                module.path = Convert.ToString(reader["path"]);
                module.type = Convert.ToInt32(reader["type"]);
                module.version = Convert.ToString(reader["version"]);

                modules.Add(module);
            }

            reader.Close();
            connection.Close();

            return modules;
        }

        public List<Module> RetrieveServices()
        {
            List<Module> modules = new List<Module>();

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM module WHERE type=" + 0;

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Module module = new Module();
                module.id = Convert.ToInt64(reader["id"]);
                module.name = Convert.ToString(reader["name"]);
                module.enabled = Convert.ToInt32(reader["enabled"]);
                module.path = Convert.ToString(reader["path"]);
                module.type = Convert.ToInt32(reader["type"]);
                module.version = Convert.ToString(reader["version"]);

                modules.Add(module);
            }

            reader.Close();
            connection.Close();

            return modules;
        }

        public List<Module> RetrieveApps()
        {
            List<Module> modules = new List<Module>();

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM module WHERE type=1";

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Module module = new Module();
                module.id = Convert.ToInt64(reader["id"]);
                module.name = Convert.ToString(reader["name"]);
                module.enabled = Convert.ToInt32(reader["enabled"]);
                module.path = Convert.ToString(reader["path"]);
                module.type = Convert.ToInt32(reader["type"]);
                module.version = Convert.ToString(reader["version"]);

                modules.Add(module);
            }

            reader.Close();
            connection.Close();

            return modules;
        }
    }
}
