namespace Ghostware.GPS.NET.Models.ConnectionInfo.Credentials
{
    public abstract class BaseProxyCredentials
    {
        public string ProxyUsername { get; set; }

        protected BaseProxyCredentials(string username)
        {
            ProxyUsername = username;
        }
    }
}
