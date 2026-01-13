using BoleBiljart.Pages;
using BoleBiljart.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;

namespace BoleBiljart.Viewmodels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly FirebaseAuthClient _authClient;
        private readonly UserService _userService;

        [ObservableProperty]
        private string _email = null!;

        [ObservableProperty]
        private string _password = null!;

        [ObservableProperty]
        private string _passwordRepeat = null!;

        [ObservableProperty]
        private string _username = null!;

        [ObservableProperty]
        private string _registerStatus = null!;

        public RegisterViewModel(FirebaseAuthClient authClient, UserService userService)
        {
            _authClient = authClient;
            _userService = userService;
        }

        [RelayCommand]
        private async Task Register()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(PasswordRepeat))
            {
                RegisterStatus = "Gegevens ontbreken. Vul alle logingegevens in.";
                return;
            }
            if (Password != PasswordRepeat)
            {
                RegisterStatus = "Wachtwoorden komen niet overeen.";
                return;
            }
            if (Username.Any(char.IsWhiteSpace))
            {
                RegisterStatus = "Geen spaties toegelaten in gebruikersnaam.";
                return;
            }
            if (Username.Length < 5)
            {
                RegisterStatus = "Gebruikersnaam moet minstens 5 karakters lang zijn.";
                return;
            }

            try
            {
                await _authClient.CreateUserWithEmailAndPasswordAsync(Email, Password);
                await _authClient.User.GetIdTokenAsync();
                if (_authClient.User.Uid != null)
                {
                    Models.User newUser = new Models.User()
                    {
                        Uid = _authClient.User.Uid,
                        Username = Username,
                        Email = Email
                    };
                    await _userService.PostAsync(newUser);
                    await Shell.Current.GoToAsync(nameof(UserStatsPage));
                }
                else throw new Exception("Authorisatie van nieuwe gebruiker mislukt.");
            }
            catch (FirebaseAuthHttpException ex)
            {
                RegisterStatus = ex.Reason switch
                {
                    AuthErrorReason.InvalidEmailAddress =>
                        "Het e-mailadres is niet geldig.",
                    AuthErrorReason.EmailExists =>
                        "Dit e-mailadres is al in gebruik.",
                    AuthErrorReason.WeakPassword =>
                        "Wachtwoord is te zwak. Gebruik minstens 6 karakters.",
                    AuthErrorReason.TooManyAttemptsTryLater =>
                        "Te veel pogingen. Probeer later opnieuw.",
                    _ => $"Er is een fout opgetreden: {ex.Reason}"
                };
            }
            catch (Exception ex)
            {
                RegisterStatus = $"Er is een fout opgetreden: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task GotoLogin()
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
}
