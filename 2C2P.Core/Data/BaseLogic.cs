using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace _2C2P.Core.Data
{
    public abstract class BaseLogic : IDisposable, IBaseLogic
    {
        private bool IsConnected;
        private bool TransactionBegan;
        private bool Disposed;

        protected IDatabase Db { get; private set; }

        protected bool IsUTC { get; private set; }

        #region "Constructor"
        protected BaseLogic(IDatabase database)
        {
            SetConnection(database);
        }
        #endregion

        #region "Connection management"     
        public bool SetConnection(IDatabase database)
        {
            try
            {
                if (database == null || database.Connection == null)
                {
                    IsConnected = false;
                }
                else
                {
                    IsConnected = true;
                    Db = database;
                }
                return true;
            }
            catch { throw; }
        }

        protected bool OpenConnection(bool beginTransaction = false, bool reset = false)
        {
            try
            {
                if (IsConnected == false) { return false; }
                if (Db != null)
                {
                    if (Db.Connection != null && Db.TransactionStarted && Db.Connection.State == ConnectionState.Open) { return true; }

                    if (reset) { Db.CloseConnection(); }

                    if (beginTransaction)
                    {
                        Db.OpenConnection(beginTransaction);
                        TransactionBegan = true;
                    }
                    else { Db.OpenConnection(); }
                    if (Db.Connection.State == ConnectionState.Open) { return true; }
                    return false;
                }
            }
            catch { throw; }
            return false;
        }

        protected bool CloseConnection(SqlTransType transType = SqlTransType.None)
        {
            try
            {
                if (IsConnected == false)
                    return false;
                if (TransactionBegan == false && Db.TransactionStarted) { return false; }

                if (Db != null)
                {
                    if (transType == SqlTransType.None && Db.TransactionStarted) { return false; }

                    if (Db.Connection != null && Db.Connection.State == ConnectionState.Open)
                    {
                        switch (transType)
                        {
                            case SqlTransType.Commit:
                                Db.CloseConnection(transType);
                                TransactionBegan = false;
                                break;

                            case SqlTransType.Rollback:
                                Db.CloseConnection(transType);
                                TransactionBegan = false;
                                break;

                            default:
                                Db.CloseConnection();
                                break;
                        }
                    }
                }
                return true;
            }
            catch { throw; }
        }

        #endregion

        #region "Data retrieval functions"
        protected async Task<DateTime> GetSystemDateTime()
        {
            try
            {
                OpenConnection();
                return await Db.Connection.QuerySingleAsync<DateTime>(SqlDateTime, null, Db.Transaction, Db.CommandTimeout).ConfigureAwait(false);
            }
            catch { throw; }
            finally { CloseConnection(); }
        }

        protected string SqlDateTime
        {
            get
            {
                string result;
                Type type = Db.GetType();
                if (type == typeof(MySql.Data.MySqlClient.MySqlConnection))
                {
                    //if (IsUTC) { }   No using yet
                    result = "(SELECT NOW())";
                }
                else
                {
                    //if (IsUTC) { }   No using yet
                    result = "(SELECT GETDATE())";
                }
                return result;
            }
        }
        #endregion

        #region "Dispose Function"
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                    CloseConnection();
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
            }
            Disposed = true;
        }

        ~BaseLogic() { Dispose(false); }
        #endregion

    }
}