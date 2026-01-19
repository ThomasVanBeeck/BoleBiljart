
using Android.Media;
using BoleBiljart.Interfaces;

namespace BoleBiljart.Models
{
    public class Game : IModelHasKey
    {
        public string Key { get; set; } = null!;

        public DateTime Datetime { get; set; } = DateTime.Today;

        public string HasWhiteBall { get; set; } = "Player 2";
        public string HasOpeningShot { get; set; } = "Player 1";

        public string Player1Id { get; set; } = null!;
        public int Player1HighRun { get; set; } = 0;
        public int Player1Score { get; set; } = 0;
        public string Player1Username { get; set; } = "usernameP1";

        public string Player2Id { get; set; } = null!;
        public int Player2HighRun { get; set; } = 0;
        public int Player2Score { get; set; } = 0;
        public string Player2Username { get; set; } = "usernameP2";

        public int TargetScore { get; set; } = 25;

        public string YearMonth { get; set; } = "1999-01";
        public string YearMonthDay { get; set; } = "01-01-1999";
    }
}
