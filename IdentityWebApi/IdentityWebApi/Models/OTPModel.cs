namespace IdentityWebApi.Models
{
    public class OTPModel
    {
        public string Body(int otp)
        {
            return $"Dear User,\r\n\r\nPlease use {otp} as the OTP to create your password." +
                   $"\r\n\r\nRegards,\r\n\r\nPostOShare";
        }

        public int CreateOTP()
        {
            return new Random().Next(1000000);
        }
    }
}
