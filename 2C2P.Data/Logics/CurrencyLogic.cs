using System;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using _2C2P.Core.Data;
using _2C2P.DataAccess.Interfaces;
using _2C2P.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace _2C2P.DataAccess.Logics
{
    public class CurrencyLogic : BaseLogic, ICurrencyLogic
    {
        public CurrencyLogic(IDatabase database) : base(database) { }

        public async Task<CurrencyModel> GetRow(string currencyCode)
        {
            try
            {
                var builder = new SqlBuilder();
                var sql = builder.AddTemplate($@"SELECT * FROM {CurrencyModel.TableName()} 
                                    WHERE {CurrencyModel.ColumnName(x => x.CurrencyCode)} = @currCode", new { currCode = currencyCode });
                OpenConnection();
                return await Db.Connection.QuerySingleOrDefaultAsync<CurrencyModel>(sql.RawSql, sql.Parameters, Db.Transaction, Db.CommandTimeout).ConfigureAwait(false);
            }
            catch { throw; }
            finally { CloseConnection(); }
        }

        public async Task<List<CurrencyModel>> GetAll()
        {
            try
            {
                var builder = new SqlBuilder();
                var sql = builder.AddTemplate($@"SELECT * FROM {CurrencyModel.TableName()}");

                OpenConnection();
                var results = await Db.Connection.QueryAsync<CurrencyModel>(sql.RawSql, sql.Parameters, Db.Transaction, Db.CommandTimeout).ConfigureAwait(false);
                return results.ToList();
            }
            catch { throw; }
            finally { CloseConnection(); }
        }
    }
}