namespace Lumle.AuthServer.Infrastructures.Helpers.Constants
{
    public class GoogleAuthConstants
    {
        public const string ClinetId = "706131245342-gnmnp1583e23b8042fm3sr2a5hpjtftl.apps.googleusercontent.com";
        public const string Issuer = "https://accounts.google.com";

        //FireBase Constants
        public const string GoogleMetaDataUrl = "https://www.googleapis.com/robot/v1/metadata/";
        public const string GoogleSecureTokenBaseUrl = "https://securetoken.google.com/";
        public const string FireBaseProjectId = "sipradi-166707";
        public const string X509CertificateEndpoint = "x509/securetoken@system.gserviceaccount.com";
        public const string FirebaseIssuer = GoogleSecureTokenBaseUrl + FireBaseProjectId;
    }
}
