using Dapper;
using _2C2P.Core.Data;
using System.Threading.Tasks;
using System.Data;
using _2C2P.DataAccess.Models;
using System.Collections.Generic;
using System;

namespace _2C2P.DataAccess.Interfaces
{
    public interface ITransactionsLogic : IBaseLogic
    {
   
        Task<Int32> ImportData(List<TransactionsModel> list);

        Task<List<TransactionsModel>> GetList(string currencyCode, DateTime? transactionDateFrom, DateTime? transactionDateTo, string status);
    }
}
