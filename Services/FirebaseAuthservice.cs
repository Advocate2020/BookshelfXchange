using Firebase.Auth;

namespace BookShelfXChange.Services
{
    public class FirebaseTokens
    {
        public required string IdToken { get; set; }
        public required string RefreshToken { get; set; }
    }
    public class FirebaseAuthService
    {
        private readonly FirebaseAuthClient _firebaseAuth;

        public FirebaseAuthService(FirebaseAuthClient firebaseAuth)
        {
            _firebaseAuth = firebaseAuth;
        }

        public async Task<User?> SignUp(string email, string password)
        {
            var userCredentials = await _firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password);

            return userCredentials is null ? null : userCredentials.User;
        }

        public async Task<FirebaseTokens> Login(string email, string password)
        {
            try
            {
                var authUser = await _firebaseAuth.SignInWithEmailAndPasswordAsync(email, password);

                if (authUser is null)
                {
                    throw new Exception("User not found");
                }

                string token = await authUser.User.GetIdTokenAsync();

                if (token is null)
                {
                    throw new Exception("Token not found");
                }

                var refresh = authUser.User.Credential.RefreshToken;

                return new FirebaseTokens { IdToken = token, RefreshToken = refresh };
            }
            catch (Exception)
            {
                throw new Exception("User not found");
            }
        }

        public void SignOut() => _firebaseAuth.SignOut();
    }
}
