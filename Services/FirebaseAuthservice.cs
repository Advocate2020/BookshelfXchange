using Firebase.Auth;

namespace BookshelfXchange.Repository
{
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

        public async Task<User?> Login(string email, string password)
        {
            var userCredentials = await _firebaseAuth.SignInWithEmailAndPasswordAsync(email, password);

            return userCredentials is null ? null : userCredentials.User;
        }

        public void SignOut() => _firebaseAuth.SignOut();
    }
}
