using System;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using _2C2P.Core.Data;
using _2C2P.DataAccess.Interfaces;
using _2C2P.DataAccess.Models;
using System.Data;
using System.Data.SqlClient;

namespace _2C2P.DataAccess.Logics
{
    public class TransactionsLogic : BaseLogic, ITransactionsLogic
    {
        public TransactionsLogic(IDatabase database) : base(database) { }


        public void InsertCSVRecords(DataTable transData)
        {
            try
            {
                //creating object of SqlBulkCopy    
                SqlBulkCopy objbulk = new SqlBulkCopy(Db.Connection.ConnectionString);
                //assigning Destination table name    
                objbulk.DestinationTableName = TransactionsModel.TableName();
                //Mapping Table column    
                foreach (DataColumn column in transData.Columns)
                {
                    objbulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
                }
                //inserting Datatable Records to DataBase    
                objbulk.WriteToServer(transData);
                objbulk.Close();
           
            }
            catch (Exception e)
            {

            }
            finally
            {

            }
           
        }

    }
}
