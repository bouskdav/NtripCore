namespace Ghostware.GPS.NET.Models.ConnectionInfo.Credentials
{
    public class ProxyCredentials : BaseProxyCredentials
    {
        public string ProxyPassword { get; set; }

        public ProxyCredentials(string username, string password) : base(username)
        {
            ProxyPassword = password;
        }
    }
}
