using BoleBiljart.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BoleBiljart.Viewmodels
{
    public partial class UserStatsViewModel : ObservableObject, IDisposable
    {
        private readonly AvatarService _avatarService;
        private readonly FirebaseAuthClient _authClient;
        private readonly UserService _userService;

        private IObservable<Models.User> _userObservable;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AvatarImageSource))]
        private Models.User _mainUser = null!;

        public string AvatarImageSource => MainUser != null
            ? _avatarService.GetAvatarFilenameByNumber(MainUser.AvatarNumber)
            : "default_avatar.png";

        public UserStatsViewModel(AvatarService avatarService,
            FirebaseAuthClient authClient,
            UserService userService)
        {
            _avatarService = avatarService;
            _authClient = authClient;
            _userService = userService;
            _userObservable = userService.GetByUidAsync(_authClient.User.Uid);

            var subscription = _userObservable
                .Where(user => user.Uid == _authClient.User?.Uid)
                .Subscribe(user => MainUser = user);
            _disposables.Add(subscription);
        }

        [RelayCommand]
        private async Task Logout()
        {
            _authClient.SignOut();
            await Shell.Current.GoToAsync("//Login");
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
