using BoleBiljart.Pages;
using BoleBiljart.ViewModels;
using Firebase.Auth;

namespace BoleBiljart
{
    public partial class AppShell : Shell
    {
        private readonly FirebaseAuthClient _authClient;
        public AppShell(AppShellViewModel vm, FirebaseAuthClient authClient)
        {
            _authClient = authClient;
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(GameHistoryPage), typeof(GameHistoryPage));
            Routing.RegisterRoute(nameof(GameEditorPage), typeof(GameEditorPage));
            Routing.RegisterRoute(nameof(UserStatsPage), typeof(UserStatsPage));
            BindingContext = vm;

            if (authClient.User != null)
            {
                // De gebruiker is al bekend, stuur ze direct door
                // Gebruik een kleine delay of Dispatcher om te zorgen dat de UI klaar is
                Dispatcher.Dispatch(async () =>
                {
                    await Shell.Current.GoToAsync("//UserStatsTab/UserStats");
                });
            }
        }
    }
}