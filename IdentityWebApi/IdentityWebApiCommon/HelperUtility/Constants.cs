namespace IdentityWebApiCommon.HelperUtility
{
    public class Constants
    {
        //routes
        public const string LoginIdentityRoute = "login-identity";
        public const string RegisterIdentityRoute = "register-identity"; 
        public const string SearchIdentityRoute = "search-identity";
        public const string VerifyIdentityRoute = "verify-identity";
        public const string ValidatePasscodeIdentityRoute = "validate-passcode";
        public const string ChangeCredentialsIdentityRoute = "change-credentials-identity";
        public const string GenerateAccessTokenIdentityRoute = "generate-accessToken";
        public const string ValidateAccessTokenIdentityRoute = "validate-accessToken";

        //configuration
        public const string Subject = "PostOShare OTP";

        //Validation
        public const string UserValidationError = "Invalid username and/or password";
        public const string UserExistsError = "Please choose a different username and/or password";
        public const string InvalidOTPError = "Invalid OTP";
    }
}
