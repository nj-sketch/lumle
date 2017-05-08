namespace Lumle.Infrastructure.Constants.LumleLog
{
    public static class CustomLogIdentifier
    {
        public const string CustomLog = "CustomLog: ";
        public const string LoginSuccessMessage = " logged in successfully from IP: ";
        public const string LoginFailureMessage = " attempted unauthorized login from IP: ";
        public const string InactiveUserLoginMessage = " attempted login for inactive user from IP: ";
        public const string LockedAccountLoginMessage = " attempted login for locked account from IP: ";
        public const string LogoutMessage = " logged off successfully from IP: ";
        public const string ForgotPasswordAttemptAndFailure = " attempted forgot password but error occurred from IP:";
        public const string ForgotPasswordAttemptAndEmailSent = " attempted forgot password and email has been sent from IP:";
        public const string InvalidForgotPasswordLink = " attempted invalid link for forgot password from IP:";
        public const string ExpiredForgotPasswordLink = " attempted expired link for forgot password from IP:";
        public const string PasswordResetSuccessfully = " reset password successfully from IP:";
    }
}
