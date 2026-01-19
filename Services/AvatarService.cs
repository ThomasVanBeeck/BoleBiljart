namespace BoleBiljart.Services
{
    public class AvatarService
    {
        public List<string> avatarFilenames =
        [
            "avatar_anime.png", "avatar_default.png", "avatar_eekhoorn.png", "avatar_hoed.png","avatar_krijt.png", "avatar_winner.png",
        ];

        public int GetRandomAvatarNumber()
        {
            return Random.Shared.Next(avatarFilenames.Count);
        }

        public string GetAvatarFilenameByNumber(int number)
        {
            return avatarFilenames[number];
        }
    }
}