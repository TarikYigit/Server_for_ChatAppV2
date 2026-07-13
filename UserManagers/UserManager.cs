using Microsoft.Data.Sqlite;
using Server_for_ChatApp.Database;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Server_for_ChatApp.Interfaces;


namespace Server_for_ChatApp.UserManagers
{
    internal class UserManager : IUsers
    {

        private readonly string _connectionString;

        private Random _random;

        public UserManager(DatabaseManager dbManager)
        {

            _connectionString = dbManager.GetConnectionString();

            _random = new Random();

        }

        public UserInfo CreateAndAddUser(string username, string password)
        {

            int newId = GenerateRandomUserID();

            string hashedPassword = HashPassword(password);

            using (var connection = new SqliteConnection(_connectionString))
            {

                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText = @"INSERT INTO Users (ID, Username, PasswordHash) VALUES ($id, $user, $pass);";

                command.Parameters.AddWithValue("$id", newId);

                command.Parameters.AddWithValue("$user", username);

                command.Parameters.AddWithValue("$pass", hashedPassword);

                command.ExecuteNonQuery();

            }

            return new UserInfo { ID = newId, username = username, password = hashedPassword };

        }

        public List<UserInfo> GetAllUsers()
        {
            List<UserInfo> userList = new List<UserInfo>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText = "SELECT ID, Username, PasswordHash FROM Users;";

                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read()) 
                    {

                        userList.Add(new UserInfo
                        {

                            ID = reader.GetInt32(0),

                            username = reader.GetString(1),

                            password = reader.GetString(2)

                        });
                    }
                }
            }
            return userList;
        }

        public UserInfo? GetUserByName(string targetUsername)
        {

            using (var connection = new SqliteConnection(_connectionString))
            {

                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText = "SELECT ID, Username, PasswordHash FROM Users WHERE Username = $user;";

                command.Parameters.AddWithValue("$user", targetUsername);

                using (var reader = command.ExecuteReader())
                {

                    if (reader.Read()) // Read the first row found
                    {

                        return new UserInfo
                        {

                            ID = reader.GetInt32(0),

                            username = reader.GetString(1),

                            password = reader.GetString(2)

                        };
                    }
                }
            }
            return null;
        }

        public UserInfo? GetUserById(int userId)
        {

            using (var connection = new SqliteConnection(_connectionString))
            {

                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText = "SELECT ID, Username, PasswordHash FROM Users WHERE ID = $id;";

                command.Parameters.AddWithValue("$id", userId);

                using (var reader = command.ExecuteReader())
                {

                    if (reader.Read())
                    {

                        return new UserInfo
                        {

                            ID = reader.GetInt32(0),

                            username = reader.GetString(1),

                            password = reader.GetString(2)

                        };
                    }
                }
            }
            return null;

        }

        public List<UserInfo> GetAllUsersExcept(int excludedUserId)
        {
            List<UserInfo> userList = new List<UserInfo>();

            using (var connection = new SqliteConnection(_connectionString))
            {

                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText = "SELECT ID, Username, PasswordHash FROM Users WHERE ID != $id;";

                command.Parameters.AddWithValue("$id", excludedUserId);

                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read()) // Loop through all rows found
                    {

                        userList.Add(new UserInfo
                        {

                            ID = reader.GetInt32(0),

                            username = reader.GetString(1),

                            password = reader.GetString(2)

                        });
                    }
                }
            }
            return userList;
        }

        public bool VerifyUserPassword(string targetUsername, string plainTextPassword)
        {

            UserInfo existingUser = GetUserByName(targetUsername);

            if (existingUser != null)
            {

                string hashedInput = HashPassword(plainTextPassword);

                return existingUser.password == hashedInput;

            }
            return false;

        }

        private string HashPassword(string plainTextPassword)
        {

            using (SHA256 sha256 = SHA256.Create())
            {

                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainTextPassword));

                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {

                    builder.Append(bytes[i].ToString("x2"));

                }
                return builder.ToString();
            }
        }

        public int GenerateRandomUserID()
        {

            while (true)
            {

                int randomNumber = _random.Next(1, 255);

                if (GetUserById(randomNumber) == null)
                {

                    return randomNumber;

                }
            }
        }
    }
}