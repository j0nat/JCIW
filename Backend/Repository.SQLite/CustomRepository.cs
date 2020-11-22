using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Interface;

namespace Repository.SQLite
{
    [Serializable]
    public class CustomRepository : ICustomRepository
    {
        public void CreateTable(string name, Dictionary<string, Type> columns)
        {
            if (TableExist(name))
            {
                // Ensure that the columns are alike
                Dictionary<string, Type> tableInfo = TableInfo(name);

                if (!Equals(columns, tableInfo))
                {
                    // The columns in the new table differ from the old

                    // hmm... Drop and re-create? Modify table?
                }
            }
            else
            {
                // Create the table
                SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
                connection.Open();

                SQLiteCommand command = connection.CreateCommand();

                string query = "CREATE TABLE " + name + " (";

           //     command.Parameters.Add(new SQLiteParameter("@0", name));

                int count = 0;
                foreach (KeyValuePair<string, Type> column in columns)
                {
                    if (column.Value == typeof(string))
                    {
                        // TEXT
                        query += column.Key + " TEXT";
                      //  command.Parameters.Add(new SQLiteParameter((count + 1).ToString(), column.Key));
                    }
                    else
                    if (column.Value == typeof(double))
                    {
                        // INTEGER
                        query += column.Key + " INTEGER";
                      //  command.Parameters.Add(new SQLiteParameter((count + 1).ToString(), column.Key));
                    }

                    if ((count + 1) < (columns.Count))
                    {
                        query += ", ";
                    }

                    count++;
                }

                query += ")";

                command.CommandType = CommandType.Text;
                command.CommandText = query;

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        /// <summary>
        /// Compares two tables to each other and returns a bool value that says if they are equal or not
        /// </summary>
        /// <param name="table1"></param>
        /// <param name="table2"></param>
        /// <returns>equals</returns>
        private bool Equals(Dictionary<string, Type> table1, Dictionary<string, Type> table2)
        {
            bool equals = true;

            if (table1.Count == table2.Count)
            {
                foreach (KeyValuePair<string, Type> column in table1)
                {
                    if (table2.ContainsKey(column.Key))
                    {
                        Type type = table2[column.Key];

                        if (column.Value != type)
                        {
                            equals = false;
                            break;
                        }
                    }
                    else
                    {
                        equals = false;
                    }
                }
            }
            else
            {
                equals = false;
            }

            return equals;
        }

        /// <summary>
        /// Returns info contained in a table by it's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>columns</returns>
        private Dictionary<string, Type> TableInfo(string name)
        {
            Dictionary<string, Type> columns = new Dictionary<string, Type>();

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "PRAGMA table_info(" + name + ")";

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string columnName = reader["name"].ToString();
                string tableType = reader["type"].ToString();

                if (tableType.ToLower() == "text")
                {
                    columns.Add(columnName, typeof(string));
                }
                else
                if (tableType.ToLower() == "integer")
                {
                    columns.Add(columnName, typeof(double));
                }
            }

            reader.Close();
            connection.Close();

            return columns;
        }

        /// <summary>
        /// Checks to see if a table exists by it's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>exists</returns>
        private bool TableExist(string name)
        {
            bool exists = false;

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name=@0";
            command.Parameters.Add(new SQLiteParameter("@0", name));

            SQLiteDataReader reader = command.ExecuteReader();

            bool foundTable = false;
            while (reader.Read())
            {
                foundTable = true;

                break;
            }

            reader.Close();
            connection.Close();

            if (foundTable == true)
            {
                exists = true;
            }

            return exists;

        }

        public void ExecuteQuery(string query, Dictionary<string, object> commands)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = query;

            foreach (KeyValuePair<string, object> commandPair in commands)
            {
                command.Parameters.Add(new SQLiteParameter(commandPair.Key, commandPair.Value));
            }
            
            command.ExecuteNonQuery();
            connection.Close();
        }

        public Dictionary<string, string>[] Read(string query)
        {
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = query;

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int columns = reader.FieldCount;
                Dictionary<string, string> row = new Dictionary<string, string>();
                for (int i = 0; i < columns; i++)
                {
                    string columnName = reader.GetName(i);
                    string columnValue = reader[i].ToString();

                    row.Add(columnName, columnValue);
                }

                rows.Add(row);
            }

            reader.Close();
            connection.Close();

            return rows.ToArray();
        }
    }
}
