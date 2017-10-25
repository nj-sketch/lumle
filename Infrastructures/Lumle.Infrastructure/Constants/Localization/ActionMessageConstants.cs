namespace Lumle.Infrastructure.Constants.Localization
{
    public class ActionMessageConstants
    {
        public const string AddedSuccessfully = "Added successfully";
        public const string UpdatedSuccessfully = "Updated successfully";
        public const string DeletedSuccessfully = "Deleted successfully";
        public const string EmailSentSuccessfully = "Email sent successfully";
        public const string Success = "Success";
        public const string ErrorOccured = "Error occured";
        public const string SelectValidItemErrorMessage = "Please select valid item";
        public const string ResourceNotFoundErrorMessage = "Resource you are looking no longer exits";
        public const string InternalServerErrorMessage = "Something went wrong. Please try again later";
        public const string PleaseFillAllTheRequiredFieldErrorMessage = "Please fill all the required field";
        public const string DataInsertedSuccessfully = "Data inserted successfully";
        public const string UnableToInsertDataError = "Unable to insert data. Please try again";
        public const string InvalidExcelFormatError = "Unable to insert data due to Invalid excel format";
        public const string Of = "of";

        public const string UnableToSendActivationEmailErrorMessage = "Unable to send activation email. Please try again";
        public const string RoleNotFoundErrorMessage = "Role not found";
        public const string RoleAlreadyExistErrorMessage = "Unable to create role. Role already exist";
        public const string UnableToAssignRoleToUserErrorMessage = "Unable to assign role to user";
        public const string UnableToManageUserRoleErrorMessage = "Unable to manage user role";
        public const string UnableToDeleteErrorMessage = "Unable to delete. Please try again";
        public const string UnableToAddErrorMessage = "Unable to add. Please try again";
        public const string UnableToUpdateErrorMessage = "Unable to update. Please try again";

        #region Constants used in core module
        //account controller constants AccountCreatedSuccessfully
        public const string AccountCreatedSuccessfully = "Success!!! Your account has been created please check your email and confirm to login";
        public const string EmailConfirmedSuccessfully = "Your email has been confirmed. Please reset password to login";
        public const string PasswordResetSuccessfully = "Your password has been reset. Please enter credential to login";
        public const string SecurityCode = "Your security code is:";
        public const string SignUpEmailconfirmedSuccessfully = "Your email has been confirmed";
        public const string InvalidLoginAttemptErrorMessage = "Invalid login attempt. Please contact your adminstration";
        public const string UnableToCreateUserErrorMessage = "Unable to create User. Please try again";
        public const string FromExternalProviderErrorMessage = "Error from external provider:";
        public const string UnableToSignInErrorMessage = "Unable to sign in. Please contact your admin";
        public const string EmailAlreadyConfirmedErrorMessage = "Email already confirmed";
        public const string InvalidLinkErrorMessage = "This link is invalid";
        public const string LinkExpiredErrorMessage = "This link has been expired";
        public const string InValidUserErrorMessage = "Somthing went wrong, Please contact your admin";
        public const string PasswordCannotResetErrorMessage = "Cannot reset your password due to internal server error. Please try again";
        public const string InvalidCodeErrorMessage = "Invalid code";
        //manage controller constants
        public const string PasswordChangedSucessfully = "Your password has been changed";
        public const string PhoneNumberAddedSucessfully = "Your phone number was added";
        public const string PhoneNumberRemovedSucessfully = "Your phone number was removed";
        public const string PasswordSetSucessfully = "Your password has been set";
        public const string ExternalLoginAddedSuccessfully = "The external login was added";
        public const string TwoFactorAuthenticationProviderSetSucessfully = "Your two-factor authentication provider has been set";
        public const string UnableToChangePasswordErrorMessage = "Unable to change password";
        public const string ExternalLoginRemovedErrorMessage = "The external login was removed";
        public const string FailedToVerifyPhoneNumberErrorMessage = "Failed to verify phone number";
        #endregion

        #region Constants used in Calendar module
        public const string EventNotFoundErrorMessage = "Event not found. Please try again";
        public const string HolidayNotFoundErrorMessage = "Holiday not found. Please try again";
        #endregion
    }
}
