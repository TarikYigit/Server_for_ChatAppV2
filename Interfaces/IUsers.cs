using Server_for_ChatApp.Managers.UserManagers;

namespace Server_for_ChatApp.Interfaces
{
    public interface IUsers
    {
        UserInfo? GetUserByName(string targetUsername);

        UserInfo? GetUserById(int userId);

        UserInfo CreateAndAddUser(string username, string password);

        bool VerifyUserPassword(string targetUsername, string plainTextPassword);

        List<UserInfo> GetAllUsersExcept(int excludedUserId);

        List<UserInfo> GetAllUsers();

    }
}