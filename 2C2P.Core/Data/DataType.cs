using System;

namespace _2C2P.Core.Data
{
    public class DataType
    {
        #region ColumnAttribute
        public string ColumnName { get; set; }
        #endregion ColumnAttribute

        #region StringLengthAttribute
        public int MinLen { get; set; }
        public int MaxLen { get; set; }
        public string LenErrMsg { get; set; }
        #endregion StringLengthAttribute

        #region RequiredAttribute
        public bool Required { get; set; }
        public bool EmptyString { get; set; }
        public string RequiredErrMsg { get; set; }
        #endregion RequiredAttribute

        #region RangeAttribute
        public int? MinInteger { get; set; }
        public int? MaxInteger { get; set; }
        public Int64? MinLong { get; set; }
        public Int64? MaxLong { get; set; }
        public double? MinDouble { get; set; }
        public double? MaxDouble { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public decimal? MinDecimal { get; set; }
        public decimal? MaxDecimal { get; set; }
        #endregion RangeAttribute

        #region DisplayAttribute
        public string Description { get; set; }
        #endregion DisplayAttribute

        public Type ColumnType { get; set; }
    }
}
