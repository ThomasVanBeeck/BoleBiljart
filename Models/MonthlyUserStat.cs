
using BoleBiljart.Interfaces;

namespace BoleBiljart.Models
{
    public class MonthlyUserStat  : IModelHasKey
    {
        public string Key { get; set; } = null!;

        public int GamesLost { get; set; } = 0;
        public int GamesWon { get; set; } = 0;
        public int GamesPlayed { get; set; } = 0;
        public int GamesStartingPlayer { get; set; } = 0;

        public int ScoreAvg { get; set; } = 0;
        public int ScoreRecord { get; set; } = 0;

        public int HighrunAvg { get; set; } = 0;
        public int HighrunRecord { get; set; } = 0;
    }
}