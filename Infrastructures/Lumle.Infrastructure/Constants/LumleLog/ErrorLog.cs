using Lumle.Infrastructure.Constants.Log;

namespace Lumle.Infrastructure.Constants.LumleLog
{
    public static class ErrorLog
    {
        public const string DataFetchError = CustomLogIdentifier.CustomLog + "Error in fetching data from DB";
        public const string SaveError = CustomLogIdentifier.CustomLog + "Error in saving data in DB";
        public const string UpdateError = CustomLogIdentifier.CustomLog + "Error in saving data in DB";
        public const string DeleteError = CustomLogIdentifier.CustomLog + "Error in saving data in DB";
        public const string AuditLogError = CustomLogIdentifier.CustomLog + "Error in saving the audit log in DB";
        public const string AuditActionError = CustomLogIdentifier.CustomLog + "Undefined audit action performed";
        public const string AddUpdateError = CustomLogIdentifier.CustomLog + "Error in add or update action performed in DB";
        public const string ForgetPasswordMailTemplate = CustomLogIdentifier.CustomLog + "Error in forgot password mail template";
        public const string LoginCredentialMailTemplate = CustomLogIdentifier.CustomLog + "Error in login credential mail template";
        public const string MailError = CustomLogIdentifier.CustomLog + "Error while sending mail";
        public const string MailCrendetialError = CustomLogIdentifier.CustomLog + "Error while sending credentials to user mail";
        public const string StringComparisonError = CustomLogIdentifier.CustomLog + "Error while comparing strings";
        public const string UserCreationError = CustomLogIdentifier.CustomLog + "Error while creating new user";
        public const string TimeZoneConversion = CustomLogIdentifier.CustomLog + "Error in converting user timezone";
        public const string TwilioSms = CustomLogIdentifier.CustomLog + "Error while sending SMS";
        public const string Undefinederror = CustomLogIdentifier.CustomLog + "Undefined error";
        public const string ExcelError = CustomLogIdentifier.CustomLog + "Error in transforming into excel file";
        public const string ImageUploadError = CustomLogIdentifier.CustomLog + "Error in uploading image into server";
    }
}
