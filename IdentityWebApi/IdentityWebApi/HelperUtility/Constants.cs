namespace IdentityWebApi.HelperUtility
{
    public class Constants
    {
        //routes
        public const string LOGINIDENTITYROUTE = "login-identity";
        public const string REGISTERIDENTITYROUTE = "register-identity"; 
        public const string SEARCHIDENTITYROUTE = "search-identity";
        public const string VERIFYIDENTITYROUTE = "verify-identity";
        public const string VALIDITYPASSCODEIDENTITYROUTE = "validate-passcode";
        public const string CHANGECREDENTIALSIDENTITYROUTE = "change-credentials-identity";
        public const string GENERATEACCESSTOKENIDENTITYROUTE = "generate-accessToken";
        public const string VALIDATEACCESSTOKENIDENTITYROUTE = "validate-accessToken";

        //configuration
        public const string SUBJECT = "PostOShare OTP";

        //Validation
        public const string USERVALIDATIONERROR = "Invalid username and/or password";
        public const string USEREXISTSERROR = "Please choose a different username and/or password";
    }
}
