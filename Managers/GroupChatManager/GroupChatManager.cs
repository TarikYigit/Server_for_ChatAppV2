using Microsoft.Data.Sqlite;
using Server_for_ChatApp.Interfaces;
using Server_for_ChatApp.Managers.DatabaseManager;
using Server_for_ChatApp.Managers.GroupChatManager;

namespace Server_for_ChatApp.GroupChatManager
{
    internal class GroupChatManager : IGroupChat
    {
        private Random _random;
        private string _connectionString;

        public GroupChatManager(DatabaseManager dbManager)
        {
            _random = new Random();

            _connectionString = dbManager.GetConnectionString();

        }

        public GroupChatInfo CreateGroupChat(string groupName, List<int> userIds)
        {
            int newId = GenerateRandomGroupID();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var groupCommand = connection.CreateCommand();

                groupCommand.CommandText = "INSERT INTO Groups (GroupID, GroupName) VALUES ($groupId, $groupName);";

                groupCommand.Parameters.AddWithValue("$groupId", newId);

                groupCommand.Parameters.AddWithValue("$groupName", groupName);

                groupCommand.ExecuteNonQuery();

                foreach (int userId in userIds)
                {
                    var memberCommand = connection.CreateCommand();

                    memberCommand.CommandText = "INSERT INTO GroupMembers (GroupID, UserID) VALUES ($groupId, $userId);";

                    memberCommand.Parameters.AddWithValue("$groupId", newId);

                    memberCommand.Parameters.AddWithValue("$userId", userId);

                    memberCommand.ExecuteNonQuery();

                }
            }

            return new GroupChatInfo
            {

                GroupChatID = newId,

                GroupName = groupName,

                GroupChatUsers = userIds

            };
        }

        public void RemoveUserFromGroup(int groupId, int userId)
        {
            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(_connectionString))
            {

                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText = "DELETE FROM GroupMembers WHERE GroupID = $groupId AND UserID = $userId;";

                command.Parameters.AddWithValue("$groupId", groupId);

                command.Parameters.AddWithValue("$userId", userId);

                command.ExecuteNonQuery();

            }
        }

        public GroupChatInfo? GetGroupById(int groupId)
        {
            GroupChatInfo group = null;

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var groupCommand = connection.CreateCommand();

                groupCommand.CommandText = "SELECT GroupName FROM Groups WHERE GroupID = $groupId;";

                groupCommand.Parameters.AddWithValue("$groupId", groupId);

                using (var reader = groupCommand.ExecuteReader())
                {

                    if (reader.Read())
                    {

                        group = new GroupChatInfo
                        {

                            GroupChatID = groupId,

                            GroupName = reader.GetString(0)

                        };
                    }
                }

                if (group == null) return null;

                var membersCommand = connection.CreateCommand();

                membersCommand.CommandText = "SELECT UserID FROM GroupMembers WHERE GroupID = $groupId;";

                membersCommand.Parameters.AddWithValue("$groupId", groupId);

                using (var reader = membersCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        group.GroupChatUsers.Add(reader.GetInt32(0));

                    }
                }
            }

            return group;
        }

        public int GenerateRandomGroupID()
        {

            while (true)
            {

                int randomNumber = _random.Next(1, 255);

                if (GetGroupById(randomNumber) == null)
                {

                    return randomNumber;

                }
            }
        }

        public List<GroupChatInfo> GetGroupsForUser(int userId)
        {
            List<GroupChatInfo> myGroups = new List<GroupChatInfo>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = @"SELECT g.GroupID, g.GroupName 
                                        FROM Groups g
                                        INNER JOIN GroupMembers gm ON g.GroupID = gm.GroupID
                                        WHERE gm.UserID = $userId;";

                command.Parameters.AddWithValue("$userId", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        myGroups.Add(new GroupChatInfo
                        {
                            GroupChatID = reader.GetInt32(0),
                            GroupName = reader.GetString(1)
                        });
                    }
                }
            }
            return myGroups;
        }
    }
}