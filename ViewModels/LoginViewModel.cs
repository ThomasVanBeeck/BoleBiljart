using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;

namespace BoleBiljart.Viewmodels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly FirebaseAuthClient _authClient;

        [ObservableProperty]
        private string _email = null!;

        [ObservableProperty]
        private string _password = null!;

        [ObservableProperty]
        private string _loginStatus = null!;

        public LoginViewModel(FirebaseAuthClient authClient)
        {
            _authClient = authClient;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || (string.IsNullOrWhiteSpace(Password)))
            {
                LoginStatus = "Gegevens ontbreken. Vul alle logingegevens in.";
                return;
            }
            try
            {
                await _authClient.SignInWithEmailAndPasswordAsync(Email, Password);
                await Shell.Current.GoToAsync("//GameHistory");
            }
        catch (FirebaseAuthHttpException ex)
            {
                LoginStatus = ex.Reason switch
                {
                    AuthErrorReason.InvalidEmailAddress => 
                    "Het e-mailadres is niet geldig.",
                    AuthErrorReason.WrongPassword => 
                    "Onjuist wachtwoord.",
                    AuthErrorReason.UserNotFound => 
                    "Gebruiker niet gevonden.",
                    AuthErrorReason.TooManyAttemptsTryLater => 
                    "Te veel pogingen. Probeer later opnieuw.",
                    _ => $"Er is een fout opgetreden: {ex.Reason}"
                };
            }
            catch (Exception ex)
            {
                LoginStatus = $"Er is een fout opgetreden: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task GotoRegister()
        {
            await Shell.Current.GoToAsync("//Register");
        }
    }
}
