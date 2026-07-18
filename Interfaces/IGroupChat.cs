using Server_for_ChatApp.Managers.GroupChatManager;

namespace Server_for_ChatApp.Interfaces
{
    public interface IGroupChat
    {
        GroupChatInfo CreateGroupChat(string groupName, List<int> userIds);

        GroupChatInfo? GetGroupById(int groupId);

        int GenerateRandomGroupID();

        List<GroupChatInfo> GetGroupsForUser(int userId);

        void RemoveUserFromGroup(int groupId, int userId);

        void AddUserToGroup(int groupId, int userId);

    }
}