namespace UnityWebBrowser.Engine.Shared
{
    /// <summary>
    ///     Settings for the proxy
    /// </summary>
    public readonly struct ProxySettings
    {
        public ProxySettings(string username, string password)
        {
            Username = username;
            Password = password;
        }
        
        /// <summary>
        ///     The username to use for auth with the proxy
        /// </summary>
        public string Username { get; }
        
        /// <summary>
        ///     The password to use for auth with the proxy
        /// </summary>
        public string Password { get; }
    }
}