using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2C2P.Web.Models;
using _2C2P.Web.Pages;
using _2C2P.Core.Data;
using _2C2P.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace _2C2P.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {

        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _config;
        private readonly IDatabase _db;
        private readonly ITransactionsLogic _transaction;


        public TransactionsController(ILogger<IndexModel> logger, IConfiguration config, IDatabase db, ITransactionsLogic transaction)
        {
            _logger = logger;
            _config = config;
            _db = db;
            _transaction = transaction;
        }


        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet("{id}")]
        [Route("getC/{id}")]
        public string GetA(int id)
        {
            return id.ToString();
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] TransactionSearchRequest req)
        {
          
            var transactionList = await _transaction.GetList(req.CurrencyCode, req.TransactionDateFrom, req.TransactionDateTo, req.Status);
            
            if (transactionList == null)
            {
                return NotFound();
            }

           var results = transactionList.Select(x => new TransactionSearchResult()
                        {
                            id = x.TransactionId,
                            payment = x.TransactionAmount.ToString("#,##0.00") + " " + x.CurrencyCode,
                            Status = x.OutputStatus
                        }).ToList();

            return Ok(results);
        }

        
    }
}
