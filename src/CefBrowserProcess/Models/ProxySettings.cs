namespace CefBrowserProcess.Models
{
    public struct ProxySettings
    {
        public ProxySettings(string username, string password)
        {
            Username = username;
            Password = password;
        }
        
        public string Username { get; init; }
        public string Password { get; init; }
    }
}