using Dapper;
using _2C2P.Core.Data;
using System.Threading.Tasks;
using System.Data;

namespace _2C2P.DataAccess.Interfaces
{
    public interface ITransactionsLogic : IBaseLogic
    {
        void InsertCSVRecords(DataTable transData);
    }
}
