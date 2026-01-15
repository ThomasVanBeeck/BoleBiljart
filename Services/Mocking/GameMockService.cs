using BoleBiljart.Models;

namespace BoleBiljart.Services.Mocking
{
    internal class GameMockService
    {
        public List<Game> mockMatches = new();

        public GameMockService()
        {
            for (int i = 0; i < 20; i++)
            {
                Game mockMatch = CreateMockedMatch();
                mockMatches.Add(mockMatch);
            }
        }

        public Game CreateMockedMatch()
        {
            var yearString = DateTime.Now.Year.ToString();
            var monthString = DateTime.Now.Month.ToString();
            var dayString = DateTime.Now.Day.ToString();

            Game mockMatch = new Game()
            {
                Key = "random" + Random.Shared.Next(1, 99999),

                Datetime = DateTime.Now,

                HasWhiteBall = Random.Shared.Next(0,2) == 1 ? "Player 1" : "Player 2",
                HasOpeningShot = Random.Shared.Next(0,2) == 1 ? "Player 1" : "Player 2",

                Player1HighRun = Random.Shared.Next(2, 15),
                Player1Id = "p001",
                Player1Score = Random.Shared.Next(10, 100),

                Player2HighRun = Random.Shared.Next(2, 15),
                Player2Id = "p002",
                Player2Score = Random.Shared.Next(10, 100),

                TargetScore = 25,
                YearMonth = yearString + "-" + monthString,
                YearMonthDay = yearString + "-" + monthString + "-" + dayString,
            };
            return mockMatch;
        }

        async Task<List<Game>> GetGamesByUserIdAsync(string userId)
        {
            await Task.Delay(400);
            return mockMatches.Where(m => m.Player1Id == userId || m.Player2Id == userId).ToList();
        }

        async Task SaveNewMatchAsync(Game matchModel)
        {
            await Task.Delay(400);
            mockMatches.Add(matchModel);

        }

        async Task UpdateMatchAsync(Game matchModel)
        {
            await Task.Delay(400);

            Game existingMatch = mockMatches.FirstOrDefault(m => m.Key == matchModel.Key);
            if (existingMatch != null)
            {
                mockMatches.Remove(matchModel);
            }
            mockMatches.Add(matchModel);
        }
    }

}
