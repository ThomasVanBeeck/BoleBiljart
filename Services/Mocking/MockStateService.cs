using BoleBiljart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoleBiljart.Services.Mocking
{
    public class MockStateService
    {
        public User mainUser;
        List<User> allUsers;
        private readonly List<string> _randomNames = new List<string> { 
        "Arno","Bert", "Danny", "Johnny", "Joseph", "Koen", "Thomas", "Paul", "Michael", "Ken",
        "Benny", "Fred", "Giovanni", "Herbert", "Gerard", "Willem", "Dave", "Jacque", "David"};
        int userCounter = 0;
        public MockStateService() {
            allUsers = new List<User>();
            mainUser = CreateMockUser();
            allUsers.Add(mainUser);
            User randomUser = CreateMockUser();
            allUsers.Add(randomUser);
        }

         User CreateMockUser()
        {
            int gamesPlayed = Random.Shared.Next(15);
            int gamesWon = Random.Shared.Next(gamesPlayed);
            int gamesLost = gamesPlayed - gamesWon;
            int gamesStartingPlayer = Random.Shared.Next(gamesPlayed);

            int scoreRecord = Random.Shared.Next(120);
            int scoreAvg = Random.Shared.Next(scoreRecord);

            int highrunRecord = Random.Shared.Next(30);
            int highrunAvg = Random.Shared.Next(highrunRecord);


            userCounter++;
            var user = new User();
            user.Key = "userid" + userCounter;
            user.Username = CreateUniqueUsername();
            user.AvatarUrl = $"mockedUrl{userCounter}";
            user.GamesLost = gamesLost;
            user.GamesWon = gamesWon;
            user.GamesPlayed = gamesPlayed;
            user.ScoreAvg = scoreAvg;
            user.ScoreRecord = scoreRecord;
            user.HighrunAvg = highrunAvg;
            user.HighrunRecord = highrunRecord;

            return user;
        }


        string CreateUniqueUsername()
        {
            string username = "";
            bool isUnique = false;
            while(!isUnique)
            {
                int nameIndex = Random.Shared.Next(_randomNames.Count);
                string name = _randomNames[nameIndex];
                int number = Random.Shared.Next(100);
                username = $"{name}_{number}";
                bool exists = allUsers.Any(u => u.Username == username);
                if (!exists) isUnique = true;
            }
            return username;
        }
    }


}
