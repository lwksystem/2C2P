using System;
using System.Globalization;
using System.Text;
//using Microsoft.Extensions.Localization;

namespace _2C2P.Core
{
    public class AppException : ApplicationException
    {
        private const string ArgumentKey = "args";
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        public string ErrorCode { get; set; }
       //public IStringLocalizer Localizer { get; set; }

        public object[] Args
        {
            get { return Data.Contains(ArgumentKey) ? (object[])Data[ArgumentKey] : new object[] { }; }
            set { Data[ArgumentKey] = value; }
        }

        public AppException(string message, params object[] args)
            : base(message)
        {
            Args = args;
        }

        public AppException(string message, Exception innerException, params object[] args)
           : base(message, innerException)
        {
            Args = args;
        }

        public AppException(string errorCode, string message, params object[] args)
           : base(message)
        {
            ErrorCode = errorCode;
            Args = args;
        }

        public AppException(string errorCode, string message, Exception innerException, params object[] args)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            Args = args;
        }

    }
}
