using System.Collections.ObjectModel;

namespace BoleBiljart.Models
{
    internal class MainModel
    {
        public User? MainUser;
        public ObservableCollection<User> Opponents = new();
        public ObservableCollection<Game> GamesHistory = new();

        public User? GetUserByKey(string key)
            => Opponents.FirstOrDefault(u => u.Key == key);

        public IEnumerable<Game> GetMatchesByYear(int year)
            => GamesHistory.Where(g => g.Datetime.Year == year).OrderByDescending(m => m.Datetime);
    }
}
