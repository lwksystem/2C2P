using System;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using _2C2P.Core.Data;
using _2C2P.DataAccess.Interfaces;
using _2C2P.DataAccess.Models;


namespace _2C2P.DataAccess.Logics
{
    public class TransactionsLogic : BaseLogic, ITransactionsLogic
    {
        public TransactionsLogic(IDatabase database) : base(database) { }



    }
}
