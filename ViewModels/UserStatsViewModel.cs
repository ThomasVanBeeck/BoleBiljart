

using BoleBiljart.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Firebase.Auth;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BoleBiljart.Viewmodels
{
    public partial class UserStatsViewModel : ObservableObject, IDisposable
    {
        private readonly FirebaseAuthClient _authClient;
        private readonly UserService _userService;
        private IObservable<Models.User> _userObservable;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [ObservableProperty]
        private Models.User _mainUser = null!;

        [ObservableProperty]
        private string _authId;

        public UserStatsViewModel(UserService userService, FirebaseAuthClient authClient)
        {
            _authClient = authClient;
            AuthId = _authClient.User.Uid;
            _userService = userService;
            _userObservable = userService.GetByUidAsync(_authClient.User.Uid);

            var subscription = _userObservable
                .Where(user => user.Uid == _authClient.User?.Uid)
                .Subscribe(user => MainUser = user);
            _disposables.Add(subscription);       
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
