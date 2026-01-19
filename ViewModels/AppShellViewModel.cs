using BoleBiljart.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using Firebase.Auth;

namespace BoleBiljart.ViewModels
{
    public partial class AppShellViewModel : ObservableObject
    {
        private readonly FirebaseAuthClient _authClient;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsLoggedOut))]
        private bool _isAuthenticated;

        public bool IsLoggedOut => !IsAuthenticated;

        public AppShellViewModel(FirebaseAuthClient authClient)
        {
            _authClient = authClient;

            _authClient.AuthStateChanged += OnAuthStateChanged;

            IsAuthenticated = _authClient.User != null;
        }

        private void OnAuthStateChanged(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsAuthenticated = _authClient.User != null;
                if (IsAuthenticated) Shell.Current.GoToAsync("//UserStats");
            });
        }
    }
}