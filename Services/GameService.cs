using BoleBiljart.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System.Reactive.Linq;

namespace BoleBiljart.Services
{
    public class GameService : AbstractBaseService<Game>
    {
        public GameService(FirebaseClient fbClient) : base(fbClient)
        { }

        public IObservable<Game> GetAllByUid(string uid)
        {

            var asPlayer1 = _fbClient
                .Child("Game")
                .OrderBy("Player1Id")
                .EqualTo(uid)
                .AsObservable<Game>();

            var asPlayer2 = _fbClient
                .Child("Game")
                .OrderBy("Player2Id")
                .EqualTo(uid)
                .AsObservable<Game>();

            return asPlayer1.Merge(asPlayer2)
                .Where(x => x.Object != null)
                .Select(x => {
                    x.Object!.Key = x.Key;
                    return x.Object;
                })
                // Voorkom dubbele Games, bijvoorbeeld bij tegen jezelf spelen
                // Waarbij je zowel player1 als player2 bent geweest
                .GroupBy(m => m.Key)
                .SelectMany(g => g.Take(1));
        }
    }
}
