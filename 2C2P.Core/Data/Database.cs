using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;

namespace _2C2P.Core.Data
{
    public class Database : IDatabase
    {
        private readonly byte[] DBConnectionCryptKey;
        private readonly byte[] DBConnectionAuthKey;

        private bool IsConnected;
        private bool TransactionBegan;

        public IDbConnection Connection { get; private set; }
        public IDbTransaction Transaction { get; private set; }
        public bool TransactionStarted { get; private set; }
        public int CommandTimeout { get; private set; } = 30;


        public Database(ConnectionSetting setting)
        {
            CreateConnection(setting);
        }

        private void CreateConnection(ConnectionSetting setting)
        {
            if (Connection == null && setting != null)
            {
                if (setting.Encrypted)
                {
                    setting.ConnectionString = Cryptography.AesHmac.SimpleDecrypt(setting.ConnectionString, DBConnectionCryptKey, DBConnectionAuthKey);
                    setting.Encrypted = false;
                }

                if (setting.CommandTimeout <= 0) { setting.CommandTimeout = 30; }
                if (string.IsNullOrWhiteSpace(setting.ProviderName)) { setting.ProviderName = ""; }

                if (setting.ProviderName.ToLower() == "mysql")
                {
                    Connection = new MySqlConnection(setting.ConnectionString);
                }
                else
                {
                    Connection = new SqlConnection(setting.ConnectionString);
                }
                CommandTimeout = setting.CommandTimeout;
                IsConnected = true;
            }
        }

        public bool OpenConnection(bool beginTransaction = false, bool reset = false)
        {
            if (IsConnected == false) { return false; }
            if (Connection != null)
            {
                if (TransactionStarted && Connection.State == ConnectionState.Open) { return true; }

                if (reset) { Connection.Close(); }

                if (beginTransaction)
                {
                    Connection.Open();
                    Transaction = Connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    TransactionStarted = true;
                    TransactionBegan = true;
                }
                else { Connection.Open(); }
                if (Connection.State == ConnectionState.Open) { return true; }
                return false;
            }
            return false;
        }

        public bool CloseConnection(SqlTransType transType = SqlTransType.None)
        {
            if (IsConnected == false)
                return false;
            if (TransactionBegan == false && TransactionStarted) { return false; }

            if (Connection != null)
            {
                if (transType == SqlTransType.None && TransactionStarted) { return false; }

                if (Connection != null && Connection.State == ConnectionState.Open)
                {
                    switch (transType)
                    {
                        case SqlTransType.Commit:
                            Transaction.Commit();
                            Connection.Close();
                            Transaction = null;
                            TransactionStarted = false;
                            TransactionBegan = false;
                            break;

                        case SqlTransType.Rollback:
                            Transaction.Rollback();
                            Connection.Close();
                            Transaction = null;
                            TransactionStarted = false;
                            TransactionBegan = false;
                            break;

                        default:
                            Connection.Close();
                            break;
                    }
                }
            }
            return true;
        }

    }
}