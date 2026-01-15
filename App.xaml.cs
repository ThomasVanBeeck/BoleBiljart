using Firebase.Auth;

namespace BoleBiljart
{
    public partial class App : Application
    {
        private readonly FirebaseAuthClient _authClient;

        public App(FirebaseAuthClient authClient)
        {
            _authClient = authClient;
            InitializeComponent();

            UserAppTheme = AppTheme.Dark;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell(_authClient));
        }
    }
}