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
        public const string ListUserDataRoute = "list-userdata";
        public const string UpsertUserDataRoute = "upsert-userdata";

        //configuration
        public const string Subject = "PostOShare OTP";
    }
}