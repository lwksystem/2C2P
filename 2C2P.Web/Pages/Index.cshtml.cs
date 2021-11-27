using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2C2P.DataAccess.Interfaces;
using _2C2P.Core.Data;
using _2C2P.DataAccess.Models;
using _2C2P.Web.Utilities;
using System.Data;
using System;
using System.Text;
using System.Xml.Serialization;

namespace _2C2P.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _config;
        private readonly IDatabase _db;
        private readonly ICurrencyLogic _currency;
        private readonly ITransactionsLogic _transaction;


        public IndexModel(ILogger<IndexModel> logger, IConfiguration config, IDatabase db, ITransactionsLogic transaction, ICurrencyLogic currency)
        {
            _logger = logger;
            _config = config;
            _db = db;
            _currency = currency;
            _transaction = transaction;

            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");

            // To save physical files to a path provided by configuration:
            _targetFilePath = config.GetValue<string>("StoredFilesPath");

            // To save physical files to the temporary files folder, use:
            //_targetFilePath = Path.GetTempPath();
        }

        private readonly long _fileSizeLimit;
        private readonly string[] _permittedExtensions = { ".csv", ".xml" };
        private readonly string _targetFilePath;

        [BindProperty]
        public BufferedSingleFileUploadPhysical FileUpload { get; set; }

        public string Result { get; private set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            string rowMessage = "";
            try
            {
                if (!ModelState.IsValid)
                {
                    Result = "Please correct the form.";

                    return Page();
                }

                var formFileContent =
                    await FileHelpers.ProcessFormFile<BufferedSingleFileUploadPhysical>(
                        FileUpload.FormFile, ModelState, _permittedExtensions,
                        _fileSizeLimit);

                if (!ModelState.IsValid)
                {
                    Result = "Unknown format.";

                    return Page();
                }

                var trustedFileNameForFileStorage = Path.GetRandomFileName();
                var filePath = Path.Combine(
                    _targetFilePath, trustedFileNameForFileStorage);


                using (var fileStream = System.IO.File.Create(filePath))
                {
                    await fileStream.WriteAsync(formFileContent);
                }


                string uploadData = System.IO.File.ReadAllText(filePath);      
                var errMsg = "";

                if (FileUpload.FormFile.FileName.ToLower().Contains(".csv"))
                {                
                    int rowCount = 0;
                    var transList = new List<TransactionsModel>();

                    foreach (string csvRow in uploadData.Split('\n'))
                    {
                        rowCount++;
                        rowMessage = "Row " + rowCount.ToString() + ": ";
                        if (!string.IsNullOrWhiteSpace(csvRow))
                        {
                            var transModel = new TransactionsModel();
                            var rowValue = csvRow.Replace('\r'.ToString(), "").Replace('\"'.ToString(), "");
                            int colCount = 0;

                            foreach (string FileRec in rowValue.Split(','))
                            {
                                if (colCount == 0) { transModel.TransactionId = FileRec.Trim(); }
                                else if (colCount == 1) { transModel.TransactionAmount = Convert.ToDecimal(FileRec.Trim()); }
                                else if (colCount == 2) { transModel.CurrencyCode = FileRec.Trim(); }
                                else if (colCount == 3)
                                {
                                    transModel.TransactionDate = Convert.ToDateTime(FileRec.Trim());
                                }
                                else if (colCount == 4) { transModel.InputStatus = FileRec.Trim(); }
                                colCount++;
                            }

                            var outStatus = OutputStatus(transModel.InputStatus);
                            if (string.IsNullOrEmpty(outStatus))
                            {
                                errMsg = rowMessage + "Invalid Status: " + transModel.InputStatus;
                                _logger.LogError(errMsg);
                                return BadRequest(errMsg);
                            }
                            else
                            {
                                transModel.FileType = "csv";
                                transModel.OutputStatus = outStatus;
                            }

                            transModel.Validate();
                            transList.Add(transModel);
                        }
                        else
                        {
                            errMsg = rowMessage + "Row Empty!";
                            _logger.LogError(errMsg);
                            return BadRequest(errMsg);
                        }
                    }
                }
                else //xml
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<_2C2P.Web.Models.Transaction>), new XmlRootAttribute("Transactions"));
                    StringReader stringReader = new StringReader(uploadData);

                    List<_2C2P.Web.Models.Transaction> transList = (List<_2C2P.Web.Models.Transaction>)serializer.Deserialize(stringReader);

                    if (transList != null && transList.Count > 0)
                    {

                    }
                    else
                    {
                        Result = "Unknown format.";
                        return Page();
                    }
                    

                  
                    
                }

              


            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(rowMessage + e.Message);
            }

            Result = "Transaction data successfully imported.";
            return Page();
        }

        private string OutputStatus(string inputStatus)
        {
            if (!string.IsNullOrWhiteSpace(inputStatus))
            {
                switch (inputStatus.ToLower())
                {
                    case "approved":
                        return "A";

                    case "failed":
                    case "rejected":
                        return "R";

                    case "done":
                    case "finished":
                        return "D";
                }
            }

            return null;
        }
    }

    public class BufferedSingleFileUploadPhysical
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }

        [Display(Name = "Note")]
        [StringLength(50, MinimumLength = 0)]
        public string Note { get; set; }
    }
}
