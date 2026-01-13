using Firebase.Auth;

namespace BoleBiljart.Services
{
    public class AuthStateService
    {
        private readonly FirebaseAuthClient _authClient;

        public AuthStateService(FirebaseAuthClient authClient)
        {
            _authClient = authClient;
        }

        public async Task<string?> GetIdTokenAsync()
        {
            if (_authClient.User == null)
                return null;

            return await _authClient.User.GetIdTokenAsync();
        }
    }
}
