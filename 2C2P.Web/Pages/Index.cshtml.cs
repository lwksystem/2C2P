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
using _2C2P.Web.Utilities;

namespace _2C2P.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _config;
        private readonly IDatabase _db;
        private readonly ICurrencyLogic _currency;

        
        public IndexModel(ILogger<IndexModel> logger, IConfiguration config, IDatabase db, ICurrencyLogic currency)
        {
            _logger = logger;
            _config = config;
            _db = db;
            _currency = currency;

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
                Result = "Please correct the form.";

                return Page();
            }

            var trustedFileNameForFileStorage = Path.GetRandomFileName();
            var filePath = Path.Combine(
                _targetFilePath, trustedFileNameForFileStorage);


            using (var fileStream = System.IO.File.Create(filePath))
            {
                await fileStream.WriteAsync(formFileContent);

                // To work directly with a FormFile, use the following
                // instead:
                //await FileUpload.FormFile.CopyToAsync(fileStream);
            }

            return RedirectToPage("./Index");
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
