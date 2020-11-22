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
    CREATE TABLE "group" (
	    "id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	    "name"	TEXT UNIQUE,
	    "description"	TEXT
    );

    CREATE TABLE "groupuseraccess" (
	    "id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	    "groupid"	INTEGER,
	    "accountId"	INTEGER
    );

    CREATE TABLE "groupappaccess" (
	    "id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	    "groupid"	INTEGER,
	    "appid"	INTEGER
    );
    */

    public class GroupRepository : IGroupRepository
    {
        public void AddAccess(long accountId, long groupId)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO groupuseraccess VALUES(@0, @1, @2)";

            command.Parameters.Add(new SQLiteParameter("@0", null));        // Null = autoincrement
            command.Parameters.Add(new SQLiteParameter("@1", groupId));
            command.Parameters.Add(new SQLiteParameter("@2", accountId));

            command.ExecuteNonQuery();
            connection.Close();
        }

        public void AddGroup(string name, string description)
        {
            if (!Exists(name))
            {
                SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
                connection.Open();

                SQLiteCommand command = connection.CreateCommand();

                command.CommandType = CommandType.Text;
                command.CommandText = "INSERT INTO groups VALUES(@0, @1, @2)";

                command.Parameters.Add(new SQLiteParameter("@0", null));        // Null = autoincrement
                command.Parameters.Add(new SQLiteParameter("@1", name));
                command.Parameters.Add(new SQLiteParameter("@2", description));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        /// <summary>
        /// Checks to see if a goup exists by it's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool Exists(string name)
        {
            bool exists = false;

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT id FROM groups WHERE name=@0";

            command.Parameters.Add(new SQLiteParameter("@0", name));

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                exists = true;

                break;
            }

            reader.Close();
            connection.Close();

            return exists;
        }

        public void AddGroup(int id, string name, string description)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO groups VALUES(@0, @1, @2)";

            command.Parameters.Add(new SQLiteParameter("@0", id));        // Null = autoincrement
            command.Parameters.Add(new SQLiteParameter("@1", name));
            command.Parameters.Add(new SQLiteParameter("@2", description));

            command.ExecuteNonQuery();
            connection.Close();
        }

        public bool Contains(long groupId, long accountId)
        {
            bool contains = false;

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT id FROM groupuseraccess WHERE groupid=@0 AND accountId=@1";

            command.Parameters.Add(new SQLiteParameter("@0", groupId));
            command.Parameters.Add(new SQLiteParameter("@1", accountId));

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                contains = true;

                break;
            }

            reader.Close();
            connection.Close();

            return contains;
        }

        public void DeleteGroup(long groupId)
        {
            DeleteFromGroup(groupId);
            DeleteFromGroupAccess(groupId);
        }

        /// <summary>
        /// Removes a group from the database by group ID
        /// </summary>
        /// <param name="groupId"></param>
        private void DeleteFromGroup(long groupId)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "DELETE FROM groups WHERE id=@0";

            command.Parameters.Add(new SQLiteParameter("@0", groupId));        // Null = autoincrement

            command.ExecuteNonQuery();
            connection.Close();
        }

        /// <summary>
        /// Removes users' access to a deleted group
        /// </summary>
        /// <param name="groupId"></param>
        private void DeleteFromGroupAccess(long groupId)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "DELETE FROM groupuseraccess WHERE groupid=@0";

            command.Parameters.Add(new SQLiteParameter("@0", groupId));        // Null = autoincrement

            command.ExecuteNonQuery();
            connection.Close();
        }

        public void DeleteAllAccountAccess(long accountId)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "DELETE FROM groupuseraccess WHERE accountId=@0";

            command.Parameters.Add(new SQLiteParameter("@0", accountId));        // Null = autoincrement

            command.ExecuteNonQuery();
            connection.Close();
        }

        public bool Exists(long groupId)
        {
            bool exists = false;

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT id FROM groups WHERE id=@0";

            command.Parameters.Add(new SQLiteParameter("@0", groupId));

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                exists = true;

                break;
            }

            reader.Close();
            connection.Close();

            return exists;
        }

        public void RemoveAccess(long accountId, long groupId)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "DELETE FROM groupuseraccess WHERE accountId=@0 AND groupid=@1";

            command.Parameters.Add(new SQLiteParameter("@0", accountId));        // Null = autoincrement
            command.Parameters.Add(new SQLiteParameter("@1", groupId));

            command.ExecuteNonQuery();
            connection.Close();
        }

        public List<Group> RetrieveAll()
        {
            List<Group> userGroups = new List<Group>();

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM groups";

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Group group = new Group();

                group.description = reader["description"].ToString();
                group.id = Convert.ToInt64(reader["id"]);
                group.name = reader["name"].ToString();

                userGroups.Add(group);
            }

            reader.Close();
            connection.Close();

            return userGroups;
        }

        public List<Group> RetrieveAccountAccess(long accountId)
        {
            List<Group> userGroups = new List<Group>();

            List<long> groupIds = RetrieveGroupAccessIdList(accountId);

            foreach (long groupId in groupIds)
            {
                SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
                connection.Open();

                SQLiteCommand command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT * FROM groups WHERE id=@0";

                command.Parameters.Add(new SQLiteParameter("@0", groupId));

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Group group = new Group();

                    group.description = reader["description"].ToString();
                    group.id = Convert.ToInt64(reader["id"]);
                    group.name = reader["name"].ToString();

                    userGroups.Add(group);
                }

                reader.Close();
                connection.Close();
            }

            return userGroups;
        }

        /// <summary>
        /// Returns a list of group IDs of groups a specific user is a member of
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>userGroupAccessIds</returns>
        private List<long> RetrieveGroupAccessIdList(long accountId)
        {
            List<long> userGroupAccessIds = new List<long>();


            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT groupid FROM groupuseraccess WHERE accountId=@0";

            command.Parameters.Add(new SQLiteParameter("@0", accountId));

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                userGroupAccessIds.Add(Convert.ToInt64(reader["groupid"]));
            }

            reader.Close();
            connection.Close();

            return userGroupAccessIds;
        }

        /// <summary>
        /// Lists group IDs of groups where a specific app has access
        /// </summary>
        /// <param name="appId"></param>
        /// <returns>userGroupAccessIds</returns>
        private List<long> RetrieveGroupAppAccessIdList(long appId)
        {
            List<long> userGroupAccessIds = new List<long>();

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT groupid FROM groupappaccess WHERE appid=@0";

            command.Parameters.Add(new SQLiteParameter("@0", appId));

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                userGroupAccessIds.Add(Convert.ToInt64(reader["groupid"]));
            }

            reader.Close();
            connection.Close();

            return userGroupAccessIds;
        }

        public void AddAppAccess(long appId, long groupId)
        {
            if (!AppAccessExists(appId, groupId))
            {
                SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
                connection.Open();

                SQLiteCommand command = connection.CreateCommand();

                command.CommandType = CommandType.Text;
                command.CommandText = "INSERT INTO groupappaccess VALUES(@0, @1, @2)";

                command.Parameters.Add(new SQLiteParameter("@0", null));        // Null = autoincrement
                command.Parameters.Add(new SQLiteParameter("@1", groupId));
                command.Parameters.Add(new SQLiteParameter("@2", appId));

                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void RemoveAppAccess(long appId, long groupId)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "DELETE FROM groupappaccess WHERE appid=@0 AND groupid=@1";

            command.Parameters.Add(new SQLiteParameter("@0", appId));        // Null = autoincrement
            command.Parameters.Add(new SQLiteParameter("@1", groupId));

            command.ExecuteNonQuery();
            connection.Close();
        }

        public bool AppAccessExists(long appId, long groupId)
        {
            bool exists = false;

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT id FROM groupappaccess WHERE appid=@0 AND groupid=@1";

            command.Parameters.Add(new SQLiteParameter("@0", appId));
            command.Parameters.Add(new SQLiteParameter("@1", groupId));

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                exists = true;

                break;
            }

            reader.Close();
            connection.Close();

            return exists;
        }

        public List<Group> RetrieveAppAccess(long appId)
        {
            List<Group> userGroups = new List<Group>();

            List<long> groupIds = RetrieveGroupAppAccessIdList(appId);

            foreach (long groupId in groupIds)
            {
                SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
                connection.Open();

                SQLiteCommand command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT * FROM groups WHERE id=@0";

                command.Parameters.Add(new SQLiteParameter("@0", groupId));

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Group group = new Group();

                    group.description = reader["description"].ToString();
                    group.id = Convert.ToInt64(reader["id"]);
                    group.name = reader["name"].ToString();

                    userGroups.Add(group);
                }

                reader.Close();
                connection.Close();
            }

            return userGroups;
        }
    }
}
