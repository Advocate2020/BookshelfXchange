using FirebaseAdmin.Auth;

namespace BookshelfXchange.Services
{
    public static class FirebaseAuthExceptionHandler
    {
        public static AuthResultStatus HandleException(FirebaseAuthException e)
        {
            AuthResultStatus status;
            switch (e.Message)
            {
                case "invalid-email":
                case "ERROR_INVALID_EMAIL":
                    status = AuthResultStatus.InvalidEmail;
                    break;
                case "INVALID_LOGIN_CREDENTIALS":
                    status = AuthResultStatus.InvalidLoginCredentials;
                    break;
                case "ERROR_WRONG_PASSWORD":
                    status = AuthResultStatus.WrongPassword;
                    break;
                case "ERROR_USER_NOT_FOUND":
                    status = AuthResultStatus.UserNotFound;
                    break;
                case "ERROR_USER_DISABLED":
                    status = AuthResultStatus.UserDisabled;
                    break;
                case "ERROR_TOO_MANY_REQUESTS":
                    status = AuthResultStatus.TooManyRequests;
                    break;
                case "operation-not-allowed":
                case "ERROR_OPERATION_NOT_ALLOWED":
                    status = AuthResultStatus.OperationNotAllowed;
                    break;
                case "email-already-in-use":
                case "ERROR_EMAIL_ALREADY_IN_USE":
                    status = AuthResultStatus.EmailAlreadyExists;
                    break;
                case "weak-password":
                    status = AuthResultStatus.WeakPassword;
                    break;
                default:
                    status = AuthResultStatus.Undefined;
                    break;
            }
            return status;
        }

        public static string GenerateExceptionMessage(AuthResultStatus exceptionCode)
        {
            string errorMessage;

            switch (exceptionCode)
            {
                case AuthResultStatus.InvalidEmail:
                case AuthResultStatus.WrongPassword:
                case AuthResultStatus.InvalidLoginCredentials:
                    errorMessage = "Invalid email or password.";
                    break;
                case AuthResultStatus.UserNotFound:
                    errorMessage = "Account does not exist.";
                    break;
                case AuthResultStatus.UserDisabled:
                    errorMessage = "Account has been disabled.";
                    break;
                case AuthResultStatus.TooManyRequests:
                    errorMessage = "Too many requests. Please try again later.";
                    break;
                case AuthResultStatus.EmailAlreadyExists:
                    errorMessage = "The email has already been registered. Please login or reset your password.";
                    break;
                case AuthResultStatus.WeakPassword:
                    errorMessage = "Password is too weak.";
                    break;
                case AuthResultStatus.OperationNotAllowed:
                default:
                    errorMessage = "An error occurred.";
                    break;
            }

            return errorMessage;
        }
    }

    public enum AuthResultStatus
    {
        InvalidLoginCredentials,
        Successful,
        EmailAlreadyExists,
        WrongPassword,
        InvalidEmail,
        UserNotFound,
        UserDisabled,
        OperationNotAllowed,
        TooManyRequests,
        WeakPassword,
        Undefined
    }

}
