namespace Server_for_ChatApp.Managers.GroupChatManager
{
    public class GroupChatInfo
    {
        public int GroupChatID { get; set; }
        public string GroupName { get; set; }
        public List<int> GroupChatUsers { get; set; }

        public GroupChatInfo()
        {

            GroupChatUsers = new List<int>();

        }
    }
}