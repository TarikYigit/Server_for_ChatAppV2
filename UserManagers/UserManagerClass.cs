using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Server_for_ChatApp.UserManagers
{
    internal class UserManager
    {
        public Dictionary<int, UserInfo> UserManagerObject;

        private Random _random;


        public UserManager()
        {

            UserManagerObject = new Dictionary<int, UserInfo>();

            _random = new Random();

        }

        public UserInfo CreateAndAddUser(string username)
        {

            int newId = GenerateRandomUserID();

            UserInfo newUser = new UserInfo
            {

                ID = newId,

                username = username

            };

            UserManagerObject.Add(newId, newUser);

            return newUser;

        }

        public void RemoveUser(int userId)
        {

            if (UserManagerObject.ContainsKey(userId))
            {

                UserManagerObject.Remove(userId);

            }
        }


        public UserInfo GetUserById(int userId)
        {

            if (UserManagerObject.TryGetValue(userId, out UserInfo user))
            {

                return user;

            }
            return null;

        }

        public List<UserInfo> GetAllUsers()
        {

            return UserManagerObject.Values.ToList();

        }

        public List<UserInfo> GetAllUsersExcept(int excludedUserId)
        {

            return UserManagerObject.Values.Where(u => u.ID != excludedUserId).ToList();

        }

        public UserInfo GetUserByName(string targetUsername)
        {

            return UserManagerObject.Values.FirstOrDefault(u => u.username.Equals(targetUsername, StringComparison.OrdinalIgnoreCase));

        }

        //Random process to assign ID's to new users -- 00 is assumed to be server and is thus not avaliable
        public int GenerateRandomUserID()
        {

            while (true)
            {

                int randomNumber = _random.Next(1, 255);

                if (!UserManagerObject.ContainsKey(randomNumber))
                {

                    return randomNumber; 

                }
            }
        }
    }
}