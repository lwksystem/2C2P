using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace _2C2P.Core.Data
{
    public interface IDatabase
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        bool TransactionStarted { get; }
        int CommandTimeout { get; }
        bool OpenConnection(bool beginTransaction = false, bool reset = false);
        bool CloseConnection(SqlTransType transType = SqlTransType.None);
    }
}
