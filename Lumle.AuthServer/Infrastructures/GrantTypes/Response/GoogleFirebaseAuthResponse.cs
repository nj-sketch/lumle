namespace Lumle.AuthServer.Infrastructures.GrantTypes.Response
{

    public class GoogleFirebaseAuthResponse
    {
        public string Iss { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string Aud { get; set; }
        public int Auth_time { get; set; }
        public string User_id { get; set; }
        public string Sub { get; set; }
        public int Iat { get; set; }
        public int Exp { get; set; }
        public string Email { get; set; }
        public bool Email_verified { get; set; }
        public Firebase Firebase { get; set; }
    }
}
