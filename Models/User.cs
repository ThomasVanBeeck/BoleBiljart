
using BoleBiljart.Interfaces;

namespace BoleBiljart.Models
{
    public class User: IModelHasKey
    {
        public string Key { get; set; } = null!;
        public string Uid { get; set; } = null!;

        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string AvatarUrl { get; set; } = null!;

        public int GamesLost { get; set; } = 0;
        public int GamesWon {  get; set; } = 0;
        public int GamesPlayed { get; set; } = 0;

        public float ScoreAvg { get; set; } = 0;
        public int ScoreRecord {  get; set; } = 0;

        public float HighrunAvg { get; set; } = 0;
        public int HighrunRecord { get; set; } = 0;
    }
}
