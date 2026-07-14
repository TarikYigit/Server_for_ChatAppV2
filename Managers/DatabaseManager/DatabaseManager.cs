using Microsoft.Data.Sqlite;

namespace Server_for_ChatApp.Managers.DatabaseManager
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

                string createUsersTable = @"CREATE TABLE IF NOT EXISTS Users (
                                            ID INTEGER PRIMARY KEY, 
                                            Username TEXT UNIQUE NOT NULL, 
                                            PasswordHash TEXT NOT NULL);";

                string createGroupsTable = @"   CREATE TABLE IF NOT EXISTS Groups (
                                                GroupID INTEGER PRIMARY KEY, 
                                                GroupName TEXT NOT NULL);";


                string createGroupMembersTable = @" CREATE TABLE IF NOT EXISTS GroupMembers (
                                                    GroupID INTEGER, 
                                                    UserID INTEGER,
                                                    PRIMARY KEY (GroupID, UserID),
                                                    FOREIGN KEY(GroupID) REFERENCES Groups(GroupID),
                                                    FOREIGN KEY(UserID) REFERENCES Users(ID));";

                command.CommandText = createUsersTable + createGroupsTable + createGroupMembersTable;

                command.ExecuteNonQuery();

            }
            Console.WriteLine("[Database] SQLite Initialized Successfully.");

        }

        public string GetConnectionString() => _connectionString;

    }
}