using BoleBiljart.Models;
using BoleBiljart.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BoleBiljart.Viewmodels
{
    public partial class GameHistoryViewModel : ObservableObject
    {
        private readonly FirebaseAuthClient _authClient;
        private readonly AvatarService _avatarService;
        private readonly GameService _gameService;
        private readonly GlobalLookupService _globalLookupService;

        private IObservable<Game> _gamesObservable;
        private IObservable<Models.GlobalLookup> _globalLookupObservable;
        private Models.GlobalLookup GLookup { get; set; } = null!;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [ObservableProperty]
        private ObservableCollection<GameWrapper> _games = new();

        //[ObservableProperty]
        // oude manier zonder wrapper voor avatars
        //private ObservableCollection<Game> games = null!;

        [RelayCommand]
        async Task EditGame(GameWrapper gameWrapper)
        {
            await Shell.Current.GoToAsync("//GameEditor", false,
                new Dictionary<string, object> { { "SelectedGame", gameWrapper.Game }, { "IsNewGame", false } });
        }

        public GameHistoryViewModel(FirebaseAuthClient authClient,
            AvatarService avatarService,
            GameService gameService,
            GlobalLookupService gLookup,
            UserService userService)
        {
            _authClient = authClient;
            _avatarService = avatarService;
            _gameService = gameService;
            _globalLookupService = gLookup;
            _globalLookupObservable = _globalLookupService.GetByUidAsync("singleton");
            _gamesObservable = gameService.GetAllByUid(_authClient.User.Uid);

            var subscriptionGLookup = _globalLookupObservable
                .Where(gl => gl.Uid == "singleton")
                .Subscribe(gl => GLookup = gl);
            
            _disposables.Add(subscriptionGLookup);


            var subscriptionGames = _gamesObservable
                .Subscribe(game =>
                {
                    if (!Games.Any(g => g.Game.Key == game.Key))
                    {
                        if (!GLookup.Usernames.TryGetValue(game.Player1Username, out int p1AvatarNumber))
                        {
                            p1AvatarNumber = 1; // Default, just in case
                        }
                        if (!GLookup.Usernames.TryGetValue(game.Player2Username, out int p2AvatarNumber))
                        {
                            p2AvatarNumber = 1; // Default, just in case
                        }
                        string p1ImgSource = _avatarService.GetAvatarFilenameByNumber(p1AvatarNumber);
                        string p2ImgSource = _avatarService.GetAvatarFilenameByNumber(p2AvatarNumber);
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Games.Add(new GameWrapper(game, p1ImgSource, p2ImgSource));
                        });
                    }
                });

            _disposables.Add(subscriptionGames);

        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
