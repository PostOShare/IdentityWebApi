namespace IdentityWebApiCommon.HelperUtility
{
    public class ErrorCodes
    {
        public const string Success = "0000";
        public const string UserValidationError = "0001";
        public const string UserExistsError = "0002";
        public const string InvalidOTPError = "0003";
        public const string InvalidAccessTokenError = "0004";
        public const string TokenExpiredError = "0005";
        public const string UsernameTokenError = "0006";
        public const string OTPCountExceededError = "0007";
        public const string InternalServerError = "0008";
        public const string UserNotFound = "0009";
    }
}