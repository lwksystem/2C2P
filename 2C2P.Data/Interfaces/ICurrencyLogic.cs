using System.Collections.Generic;
using System.Threading.Tasks;
using _2C2P.Core.Data;
using _2C2P.DataAccess.Models;

namespace _2C2P.DataAccess.Interfaces
{
    public interface ICurrencyLogic : IBaseLogic
    {
        Task<CurrencyModel> GetRow(string currencyCode);

        Task<List<CurrencyModel>> GetAll();

    }
}
