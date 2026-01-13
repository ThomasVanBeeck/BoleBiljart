using BoleBiljart.Models;
using Firebase.Database;

namespace BoleBiljart.Services
{
    public class GameService : AbstractBaseService<Game>
    {
        public GameService(FirebaseClient fbClient) : base(fbClient)
        { }
    }
}
