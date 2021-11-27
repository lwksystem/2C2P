using _2C2P.Core.Data;
using _2C2P.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2C2P.Web.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly IConfiguration _config;
        private readonly IDatabase _db;
        private readonly ICurrencyLogic _currency;

        public string Message { get; private set; } = "PageModel in C#";

        public PrivacyModel(ILogger<PrivacyModel> logger,IConfiguration config, IDatabase db, ICurrencyLogic currency)
        {
            _logger = logger;
            _config = config;
            _db = db;
            _currency = currency;
        }

        public void OnGet()
        {

            var list = _currency.GetAll().Result;

            Message = list.Count.ToString();

        }
    }
}
