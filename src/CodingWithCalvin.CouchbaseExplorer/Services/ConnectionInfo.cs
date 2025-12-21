using System;

namespace CodingWithCalvin.CouchbaseExplorer.Services
{
    /// <summary>
    /// Serializable connection information for VS settings storage.
    /// Password is NOT stored here - it goes in Windows Credential Manager.
    /// </summary>
    [Serializable]
    public class ConnectionInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string Username { get; set; }
        public bool UseSsl { get; set; }

        public ConnectionInfo()
        {
            Id = Guid.NewGuid().ToString();
        }

        public ConnectionInfo(string name, string connectionString, string username, bool useSsl)
            : this()
        {
            Name = name;
            ConnectionString = connectionString;
            Username = username;
            UseSsl = useSsl;
        }
    }
}
