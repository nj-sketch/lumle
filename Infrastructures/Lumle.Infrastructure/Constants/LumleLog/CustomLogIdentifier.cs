namespace Lumle.Infrastructure.Constants.LumleLog
{
    public static class CustomLogIdentifier
    {
        public const string CustomLog = "CustomLog: ";
        public const string LoginSuccessMessage = " logged in successfully";
        public const string LoginFailureMessage = " attempted unauthorized login";
        public const string InactiveUserLoginMessage = " attempted login for inactive user";
        public const string LockedAccountLoginMessage = " attempted login for locked account";
        public const string LogoutMessage = " logged off successfully";
        public const string ForgotPasswordAttemptAndFailure = " attempted forgot password but error occurred";
        public const string ForgotPasswordAttemptAndEmailSent = " attempted forgot password and email has been sent";
        public const string InvalidForgotPasswordLink = " attempted invalid link for forgot password";
        public const string ExpiredForgotPasswordLink = " attempted expired link for forgot password";
        public const string PasswordResetSuccessfully = " reset password successfully";
    }
}
