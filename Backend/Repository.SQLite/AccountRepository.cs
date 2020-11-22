using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Repository.Entities;
using Repository.Interface;

namespace Repository.SQLite
{
    /*
    CREATE TABLE "account" (
	    "id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	    "username"	TEXT UNIQUE,
	    "password"	TEXT,
	    "firstname"	TEXT,
	    "lastname"	TEXT,
	    "session"	TEXT
    );
    */

    public class AccountRepository : IAccountRepository
    {
        public void Add(string username, string password, string firstname, string lastname)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO account VALUES(@0, @1, @2, @3, @4, @5)";

            command.Parameters.Add(new SQLiteParameter("@0", null));        // Null = autoincrement
            command.Parameters.Add(new SQLiteParameter("@1", username));
            command.Parameters.Add(new SQLiteParameter("@2", password));
            command.Parameters.Add(new SQLiteParameter("@3", firstname));
            command.Parameters.Add(new SQLiteParameter("@4", lastname));
            command.Parameters.Add(new SQLiteParameter("@5", "123"));

            command.ExecuteNonQuery();
            connection.Close();
        }

        public bool AccountPasswordHash(string username, out long accountId, out string passwordHash)
        {
            bool foundAccount = false;
            passwordHash = "";
            accountId = 0;

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM account WHERE username=@0";
            command.Parameters.Add(new SQLiteParameter("@0", username));

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                foundAccount = true;

                accountId = Convert.ToInt64(reader["id"]);
                passwordHash = reader["password"].ToString();

                break;
            }

            reader.Close();
            connection.Close();

            return foundAccount;
        }

        public void DeleteUser(long accountId)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "DELETE FROM account WHERE id=@0";

            command.Parameters.Add(new SQLiteParameter("@0", accountId));        // Null = autoincrement

            command.ExecuteNonQuery();
            connection.Close();
        }

        public Account Get(long accountId)
        {
            Account account = null;

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM account WHERE id=@0";

            command.Parameters.Add(new SQLiteParameter("@0", accountId));

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                account = new Account();

                account.lastname = reader["lastname"].ToString();
                account.firstname = reader["firstname"].ToString();
                account.username = reader["username"].ToString();
                account.id = Convert.ToInt64(reader["id"]);

                break;
            }

            reader.Close();
            connection.Close();

            return account;
        }

        public List<Account> GetAll()
        {
            List<Account> accounts = new List<Account>();

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT id, username, firstname, lastname FROM account";


            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Account addAccount = new Account();

                addAccount.lastname = reader["lastname"].ToString();
                addAccount.firstname = reader["firstname"].ToString();
                addAccount.username = reader["username"].ToString();
                addAccount.id = Convert.ToInt64(reader["id"]);

                accounts.Add(addAccount);
            }

            reader.Close();
            connection.Close();

            return accounts;
        }

        public long GetId(string username)
        {
            long id = 0;

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT id FROM account WHERE username=@0";

            command.Parameters.Add(new SQLiteParameter("@0", username));

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                id = Convert.ToInt64(reader["id"]);
                break;
            }

            reader.Close();
            connection.Close();

            return id;
        }

        public void SetSession(Int64 accountId, string sessionId)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "UPDATE account SET session=@0 WHERE id=@1";

            command.Parameters.Add(new SQLiteParameter("@0", sessionId));
            command.Parameters.Add(new SQLiteParameter("@1", accountId));

            command.ExecuteNonQuery();
            connection.Close();
        }

        public long GetSessionAccountId(string session)
        {
            long accountId = -1;

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM account WHERE session=@0";

            command.Parameters.Add(new SQLiteParameter("@0", session));

            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                
                accountId = Convert.ToInt64(reader["id"]);
                break;
            }

            reader.Close();
            connection.Close();

            return accountId;
        }

        public void Update(long accountId, string username, string firstname, string lastname)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "UPDATE account SET username=@1, firstname=@2, lastname=@3 WHERE id=@0";

            command.Parameters.Add(new SQLiteParameter("@0", accountId));
            command.Parameters.Add(new SQLiteParameter("@1", username));
            command.Parameters.Add(new SQLiteParameter("@2", firstname));
            command.Parameters.Add(new SQLiteParameter("@3", lastname));

            command.ExecuteNonQuery();
            connection.Close();
        }

        public void UpdatePassword(long accountId, string password)
        {
            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandType = CommandType.Text;
            command.CommandText = "UPDATE account set password=@0 WHERE id=@1";

            command.Parameters.Add(new SQLiteParameter("@0", password));        // Null = autoincrement
            command.Parameters.Add(new SQLiteParameter("@1", accountId));

            command.ExecuteNonQuery();
            connection.Close();
        }

        public bool UsernameExist(string username)
        {
            bool exists = false;

            SQLiteConnection connection = (SQLiteConnection)DatabaseContext.GetConnection();
            connection.Open();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM account WHERE username=@0";
            command.Parameters.Add(new SQLiteParameter("@0", username));

            SQLiteDataReader reader = command.ExecuteReader();

            bool foundAccount = false;
            while (reader.Read())
            {
                foundAccount = true;

                break;
            }

            reader.Close();
            connection.Close();

            if (foundAccount == true)
            {
                exists = true;
            }

            return exists;
        }
    }
}