using BoleBiljart.Models;
using BoleBiljart.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BoleBiljart.Viewmodels
{
    public partial class GameHistoryViewModel : ObservableObject
    {
        private readonly GameService _gameService;
        private IObservable<Game> _gamesObservable;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [ObservableProperty]
        private List<Game> _games = new();

        //[ObservableProperty]
        //private ObservableCollection<Game> games = null!;

        [RelayCommand]
        async Task EditGame(Game game)
        {
            await Shell.Current.GoToAsync("//GameEditorTab/GameEditor", false,
                new Dictionary<string, object> { { "SelectedGame", game }, { "IsNewGame", false } });
        }

        /*
        [RelayCommand]
        async Task AddGame()
        {
            await Shell.Current.GoToAsync("//GameEditorTab/GameEditor", false,
                new Dictionary<string, object> { { "SelectedGame", new Game() }, { "IsNewGame", true } });
        }
        */

        public GameHistoryViewModel(GameService gameService, UserService userService)
        {
            _gameService = gameService;
            _gamesObservable = gameService.GetAllAsync();

            var subscription = _gamesObservable
                .Subscribe(game => Games.Add(game));
            _disposables.Add(subscription);

            // Games = new ObservableCollection<Game>();



            //Game game1 = new Game()
            //{
            //    Player1Username = "jerste"
            //};
            //Games.Add(game1);
            //Game game2 = new Game()
            //{
            //    Player2Username = "twedde",
            //    HasWhiteBall = "Player 1",
            //    HasOpeningShot = "Player 1"
            //};
            //Games.Add(game2);
            //Game game3 = new Game()
            //{
            //    Player1Username = "dikke",
            //    Player2Username = "dunne",
            //    HasOpeningShot = "Player 2",
            //    HasWhiteBall = "Player 2"
            //};
            //Games.Add(game3);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
