namespace IdentityWebApiCommon.HelperUtility
{
    public class ErrorDescriptions
    {
        public const string Success = "Success";
        public const string UserValidationError = "Invalid username and/or password";
        public const string UserExistsError = "Please choose a different username and/or password";
        public const string InvalidOTPError = "Invalid OTP";
        public const string InvalidAccessTokenError = "Invalid access token";
        public const string TokenExpiredError = "Token is expired";
        public const string UsernameTokenError = "Invalid username and/or expired token";
        public const string OTPCountExceededError = "Cannot try more than maximum attempts";
        public const string InternalServerError = "Internal server error";
        public const string UserNotFound = "User not found";
    }
}