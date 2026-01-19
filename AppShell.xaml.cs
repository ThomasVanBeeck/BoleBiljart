using BoleBiljart.Pages;

using Firebase.Auth;

namespace BoleBiljart
{
    public partial class AppShell : Shell
    {
        private readonly FirebaseAuthClient _authClient;
        public AppShell(FirebaseAuthClient authClient)
        {
            _authClient = authClient;

            InitializeComponent();

            if (authClient.User != null)
            {
                // De gebruiker is al bekend, stuur ze direct door
                // Gebruik een kleine delay of Dispatcher om te zorgen dat de UI klaar is
                Dispatcher.Dispatch(async () =>
                {
                    await Shell.Current.GoToAsync("//UserStats");
                });
            }
        }
    }
}