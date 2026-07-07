using Server_for_ChatApp.Interfaces;
using Server_for_ChatApp.UserManagers;
using ServerForChatApp;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server_for_ChatApp.Messages.ServerToClient
{
    internal class GetUserListResponse : INetworkMessage
    {

        private byte[] _userListPayload;

        public GetUserListResponse(List<UserInfo> userList)
        {

            using (MemoryStream ms = new MemoryStream())

            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write((byte)userList.Count);

                foreach (UserInfo user in userList)
                {

                    writer.Write((byte)user.ID); 

                    byte[] nameBytes = Encoding.UTF8.GetBytes(user.username);

                    writer.Write((byte)nameBytes.Length);

                    writer.Write(nameBytes);

                }

                _userListPayload = ms.ToArray();

            }
        }

        public byte GetId()
        {

            return (byte)MessageId.GET_USERS;

        }

        public byte[] ToBytes()
        {

            return _userListPayload;

        }
    }
}


// State 1: Unauthorized  --> Login  State 2: Authorized
// State 2: Authorized --> Logout State 1: Unauthorized


/* StateMachine(Users)
 * 
 * IResponse ExecuteMessage(INetworRequest message)
 * 
 * Enum state = unauthorized;
 * 
 * 
 * switch(state)
 *     case state1:
 *      switch(message):
 *          case message_1:
 *              LoginRequest request = (LoginRequest)(message); 
 *              X işlemini yap
 *              changeToState2
 *           default:
 *              return EmptyResponse()
 *     case state2:
 *          case message_1:
 *          case message2:
 *              
 *          default:
 *              return EmptyResponse()
 * 
 * 
 * 
 * 
 * 
 * */