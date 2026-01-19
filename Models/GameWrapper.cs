namespace BoleBiljart.Models
{
    public class GameWrapper
    {
        public Models.Game Game { get; set; } = null!;
        public string Player1AvatarImageSource { get; set; } = null!;
        public string Player2AvatarImageSource { get; set; } = null!;

        public GameWrapper(Game game, string p1ImgSource, string p2ImgSource)
        {
            Game = game;
            Player1AvatarImageSource = p1ImgSource;
            Player2AvatarImageSource = p2ImgSource;
        }
    }
}
