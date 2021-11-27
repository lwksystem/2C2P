using System;

namespace _2C2P.Core.Data
{
    public class ConnectionSetting
    {
        public string ConnectionName { get; set; }

        public string ConnectionString { get; set; }

        public bool Encrypted { get; set; } = true;

        public int CommandTimeout { get; set; } = 30;

        public string ProviderName { get; set; } = "sqlserver";

        public ConnectionSetting() { }

        public ConnectionSetting(string connectionString, int commandTimeout = 30, string providerName = "sqlserver", bool encrypted = true)
        {
            ConnectionString = connectionString;
            CommandTimeout = commandTimeout;
            ProviderName = providerName;
            Encrypted = encrypted;
        }
    }
}
