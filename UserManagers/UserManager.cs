using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Server_for_ChatApp.UserManagers
{
    internal class UserManager
    {
        public Dictionary<int, UserInfo> UserManagerObject;

        private Random _random;

        private readonly string _databaseFilePath = @"C:\Users\tarik.dalkiran\Desktop\Workspace\Playground\Database.txt";

        public UserManager()
        {

            UserManagerObject = new Dictionary<int, UserInfo>();

            _random = new Random();

            LoadUsersFromFile();

        }

        private void LoadUsersFromFile()
        {
            if (!File.Exists(_databaseFilePath)) return;

            string[] lines = File.ReadAllLines(_databaseFilePath);

            foreach (string line in lines)
            {

                string[] parts = line.Split('|');

                if (parts.Length == 3)
                {

                    UserInfo user = new UserInfo
                    {

                        ID = int.Parse(parts[0]),

                        username = parts[1],

                        password = parts[2] 

                    };

                    UserManagerObject.Add(user.ID, user);
                }
            }
        }

        private void SaveUserToFile(UserInfo user)
        {

            string line = $"{user.ID}|{user.username}|{user.password}\n";

            File.AppendAllText(_databaseFilePath, line);

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

        public UserInfo CreateAndAddUser(string username, string password)
        {

            int newId = GenerateRandomUserID();

            UserInfo newUser = new UserInfo
            {

                ID = newId,

                password = password,  

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

        public string GetUserPassword(string targetUsername)
        {
            UserInfo existingUser = GetUserByName(targetUsername);

            if (existingUser != null)
            {
                return existingUser.password;
            }

            return null;
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