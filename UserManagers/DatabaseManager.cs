using Microsoft.Data.Sqlite;
using System;

namespace Server_for_ChatApp.Database
{
    internal class DatabaseManager
    {
        private readonly string _connectionString = "Data Source=ChatAppDB.sqlite";

        public DatabaseManager()
        {

            InitializeDatabase();

        }

        private void InitializeDatabase()
        {

            using (var connection = new SqliteConnection(_connectionString))
            {

                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText = @"CREATE TABLE IF NOT EXISTS Users (ID INTEGER PRIMARY KEY, Username TEXT UNIQUE NOT NULL, PasswordHash TEXT NOT NULL); ";

                command.ExecuteNonQuery();

            }
            Console.WriteLine("[Database] SQLite Initialized Successfully.");

        }

        public string GetConnectionString() => _connectionString;

    }
}