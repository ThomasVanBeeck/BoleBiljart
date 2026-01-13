using BoleBiljart.Pages;
using BoleBiljart.ViewModels;
using Firebase.Auth;

namespace BoleBiljart
{

public partial class App : Application
    {
        private readonly FirebaseAuthClient _authClient;

        public App(AppShell shell, FirebaseAuthClient authClient)
        {
            _authClient = authClient;
            InitializeComponent();

            UserAppTheme = AppTheme.Dark;

            MainPage = shell;
        }
    }
}