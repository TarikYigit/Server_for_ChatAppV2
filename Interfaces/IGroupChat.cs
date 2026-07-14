using Server_for_ChatApp.Managers.GroupChatManager;
using Server_for_ChatApp.Managers.UserManagers;

namespace Server_for_ChatApp.Interfaces
{
    public interface IGroupChat
    {
        GroupChatInfo CreateGroupChat(string groupName, List<int> userIds);

        GroupChatInfo? GetGroupById(int groupId);

        int GenerateRandomGroupID();

        List<GroupChatInfo> GetGroupsForUser(int userId);

    }
}