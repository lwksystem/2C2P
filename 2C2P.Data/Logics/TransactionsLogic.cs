using System;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using _2C2P.Core.Data;
using _2C2P.DataAccess.Interfaces;
using _2C2P.DataAccess.Models;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using _2C2P.Core;
using System.Linq;

namespace _2C2P.DataAccess.Logics
{
    public class TransactionsLogic : BaseLogic, ITransactionsLogic
    {
        public TransactionsLogic(IDatabase database) : base(database) { }


		public async Task<List<TransactionsModel>> GetList(string currencyCode, DateTime? transactionDateFrom, DateTime? transactionDateTo, string status)
		{
			try
			{
				var builder = new SqlBuilder();
				DynamicParameters parameter = new DynamicParameters();

				builder.Select("*");
				if (!string.IsNullOrWhiteSpace(currencyCode))
                {
					builder.Where($@"{TransactionsModel.ColumnName(x => x.CurrencyCode)} = @currCode", new { currCode = currencyCode });
				}

				if (!string.IsNullOrWhiteSpace(status))
				{
					builder.Where($@"{TransactionsModel.ColumnName(x => x.InputStatus)} = @inputStatus", new { inputStatus = status });
				}

				if (transactionDateFrom != null)
				{
					builder.Where($@"{TransactionsModel.ColumnName(x => x.TransactionDate)} >= @transDateFrom", new { transDateFrom = transactionDateFrom });
				}

				if (transactionDateTo != null)
				{
					builder.Where($@"{TransactionsModel.ColumnName(x => x.TransactionDate)} <= @transDateTo", new { transDateTo = transactionDateTo });
				}

				var sql = builder.AddTemplate($@"SELECT /**select**/ FROM {TransactionsModel.TableName()} /**where**/ ");

				OpenConnection();
				var results = await Db.Connection.QueryAsync<TransactionsModel>(sql.RawSql, sql.Parameters, Db.Transaction, Db.CommandTimeout).ConfigureAwait(false);
				return results.ToList();
			}
			catch { throw; }
			finally { CloseConnection(); }
		}

		public async Task<Int32> ImportData(List<TransactionsModel> list)
        {
            try
            {
				if (list.Count > 0)
				{
					var sqlInsert = "INSERT INTO " + TransactionsModel.TableName() + "(" +
						 TransactionsModel.ColumnName(x => x.TransactionId) + "," +
						 TransactionsModel.ColumnName(x => x.TransactionAmount) + "," +
						 TransactionsModel.ColumnName(x => x.CurrencyCode) + "," +
						 TransactionsModel.ColumnName(x => x.TransactionDate) + "," +
						 TransactionsModel.ColumnName(x => x.FileType) + "," +
						 TransactionsModel.ColumnName(x => x.InputStatus) + "," +
						 TransactionsModel.ColumnName(x => x.OutputStatus) + ") VALUES ";


					var sqlValues = new List<string>();
					foreach (TransactionsModel row in list)
                    {
						sqlValues.Add("(" + row.TransactionId.ParseValue() + "," +
							row.TransactionAmount.ParseValue() + "," +
							row.CurrencyCode.ParseValue() + "," +
							row.TransactionDate.ParseValue() + "," +
							row.FileType.ParseValue() + "," +
							row.InputStatus.ParseValue() + "," +
							row.OutputStatus.ParseValue() + ")");
					}



					var sqlBatches = Common.GetSqlsInBatches(sqlInsert, sqlValues);
					var results = 0;

					OpenConnection(true);

					foreach (var sql in sqlBatches)
					{
						results += await Db.Connection.ExecuteAsync(sql, null, Db.Transaction, Db.CommandTimeout).ConfigureAwait(false);
					}
					
					CloseConnection(SqlTransType.Commit);
					return results;
				}
            }
            catch (Exception e)
            {
                CloseConnection(SqlTransType.Rollback);
                throw e;
            }
			return 0;
        }

    }
}
