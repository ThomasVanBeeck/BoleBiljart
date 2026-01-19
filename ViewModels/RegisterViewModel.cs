using BoleBiljart.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BoleBiljart.Viewmodels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly FirebaseAuthClient _authClient;
        private readonly AvatarService _avatarService;
        private readonly GlobalLookupService _globalLookupService;
        private readonly UserService _userService;

        private IObservable<Models.GlobalLookup> _globalLookupObservable;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [ObservableProperty]
        private Models.GlobalLookup _gLookup = null!;

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

        public RegisterViewModel(FirebaseAuthClient authClient,
            AvatarService avatarService,
            GlobalLookupService globalLookupService,
            UserService userService)
        {
            _authClient = authClient;
            _avatarService = avatarService;
            _globalLookupService = globalLookupService;
            _userService = userService;
            _globalLookupObservable = _globalLookupService.GetByUidAsync("singleton");

            var subscription = _globalLookupObservable
                .Where(gl => gl.Uid == "singleton")
                .Subscribe(gl => GLookup = gl);
            _disposables.Add(subscription);

            // handig voor aanmaken vd singleton in firebase realtime db, code 1x uitvoeren in development
            //Models.GlobalLookup glookup = new();
            //_globalLookupService.PostAsync(glookup);
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

            if (GLookup.Usernames.ContainsKey(Username))
            {
                RegisterStatus = "Gebruikersnaam al in gebruik, kies een andere.";
                return;
            }

            try
            {
                await _authClient.CreateUserWithEmailAndPasswordAsync(Email, Password);
                await _authClient.User.GetIdTokenAsync();
                if (_authClient.User.Uid != null)
                {
                    var avatarNumber = _avatarService.GetRandomAvatarNumber();
                    Models.User newUser = new Models.User()
                    {
                        Uid = _authClient.User.Uid,
                        AvatarNumber = avatarNumber,
                        Username = Username,
                        Email = Email
                    };
                    await _userService.PostAsync(newUser);
                    GLookup.Usernames.Add( Username, avatarNumber);
                    await _globalLookupService.UpdateAsync(GLookup);
                    await Shell.Current.GoToAsync("//GameHistory");
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
            await Shell.Current.GoToAsync("//Login");
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
