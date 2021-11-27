using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using _2C2P.Core.Data;

namespace _2C2P.Core
{
    public static class Common
    {
        public static readonly int SystemUserId = 0;
        public static int DefaultPageSize = 30;

        #region "String"
        public static string Format(this string value, object arg0)
        {
            return string.Format(value, arg0);
        }

        public static string Format(this string value, params object[] args)
        {
            return string.Format(value, args);
        }

        public static bool In(this string value, params string[] stringValues)
        {
            foreach (string otherValue in stringValues)
                if (string.Compare(value, otherValue) == 0)
                    return true;

            return false;
        }

        public static string Left(this string value, int length)
        {
            return value.Substring(0, length);
        }

        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static bool IsUnicode(this string input)
        {
            var asciiBytesCount = Encoding.ASCII.GetByteCount(input);
            var unicodBytesCount = Encoding.UTF8.GetByteCount(input);
            return asciiBytesCount != unicodBytesCount;
        }

        public static string RemoveLastDelimiter(this string targetString, string lastDelimiter)
        {
            if (string.IsNullOrWhiteSpace(targetString))
                return string.Empty;
            if (string.IsNullOrWhiteSpace(lastDelimiter))
                return targetString;

            if (targetString.Length == 1)
            {
                if (targetString == lastDelimiter)
                {
                    return string.Empty;
                }
                return targetString;
            }
            if (targetString.Length >= lastDelimiter.Length)
            {
                if (targetString.Substring(targetString.Length - lastDelimiter.Length, lastDelimiter.Length) == lastDelimiter)
                {
                    return targetString.Substring(0, targetString.Length - lastDelimiter.Length);
                }
                return targetString;
            }
            return targetString;
        }
        #endregion

        #region "Numeric"
        public static bool IsNumeric(this string value)
        {
            var regex = new Regex(@"[0-9]");
            return regex.IsMatch(value);
        }

        public static int ToInt(this object value)
        {
            try { return Convert.ToInt32(value); }
            catch { return 0; }
        }

        public static Int16 ToShort(this object value)
        {
            try { return Convert.ToInt16(value); }
            catch { return 0; }
        }

        public static decimal ToDecimal(this object value)
        {
            try { return Convert.ToDecimal(value); }
            catch { return 0; }
        }

        public static double ToDouble(this object value)
        {
            try { return Convert.ToDouble(value); }
            catch { return 0; }
        }

        #endregion

        #region "DateTime"
        public static bool IsDate(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) { DateTime dt; return (DateTime.TryParse(value, out dt)); }
            return false;
        }

        public static DateTime ToDateTime(this object value)
        {
            try { return Convert.ToDateTime(value); }
            catch { return DateTime.MinValue; }
        }

        #endregion

        #region "Boolean"
        public static bool ToBool(this object value)
        {

            if (value == null) { return false; }

            if (value is bool)
            {
                return (bool)value;
            }
            if (value is string)
            {
                if (value.ToString().Trim().ToUpper() == "TRUE") return true;
            }
            if (value is int || value is short)
            {
                if (Convert.ToInt32(value) == 1) return true;
            }
            return false;
        }
        #endregion

        #region "Enum"
        public static T ToEnum<T>(this object value)
        {
            return (T)Enum.Parse(typeof(T), value.ToString());
        }

        public static T ToEnum<T>(this string value) where T : struct
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static bool IsEnumDefined<T>(this T value)
        {
            return Enum.IsDefined(typeof(T), value);
        }
        #endregion

        #region "Sql"
        public static string ParseValue(this object value)
        {
            try
            {
                string tmpValue;
                if (value is string)
                {
                    tmpValue = Convert.ToString(value);
                }
                else
                {
                    if (value is DateTime)
                    {
                        DateTime? tmpDate = Convert.ToDateTime(value);
                        tmpValue = tmpDate.Value.ToString("yyyy\\/MM\\/dd HH\\:mm\\:ss\\.fff");
                    }
                    else
                    {
                        tmpValue = Convert.ToString(value).Trim();
                    }
                }

                if (value is string)
                {
                    tmpValue = ParseCleanValue(tmpValue);
                    if (tmpValue.IsUnicode() == false) { tmpValue = "'" + tmpValue + "'"; } else { tmpValue = "N'" + tmpValue + "'"; }
                }
                else if (value is int || value is long || value is short)
                {
                    if (!IsNumeric(tmpValue)) { tmpValue = "0"; }
                }
                else if (value is bool)
                {
                    tmpValue = tmpValue.Trim().ToUpper() == "TRUE" ? "1" : "0";
                }
                else if (value is DateTime)
                {
                    tmpValue = FormatDateTime(tmpValue, false);
                }
                else if (value is Enum)
                {
                    tmpValue = Convert.ToInt32(value).ToString();
                }
                return tmpValue;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string ParseCleanValue(string value)
        {
            if (value?.Length > 0)
            {
                return value.Replace("'", "''");
            }
            return string.Empty;
        }

        private static string FormatDateTime(string dateTimeValue, bool dateOnly)
        {
            try
            {
                if (IsDate(dateTimeValue))
                {
                    var inDateTime = Convert.ToDateTime(dateTimeValue);
                    dateTimeValue = inDateTime.ToString(dateOnly ? "yyyy\\/MM\\/dd" : "yyyy\\/MM\\/dd HH\\:mm\\:ss\\.fff");

                    var dateTimeSep = dateTimeValue.Split(' ');
                    if (dateTimeSep[0].Contains("."))
                    {
                        dateTimeValue = dateTimeValue.Replace(".", "/");
                    }
                    if (dateTimeSep[0].Contains("-"))
                    {
                        dateTimeValue = dateTimeValue.Replace("-", "/");
                    }

                    if (dateTimeValue == "0001/01/01" || dateTimeValue == "0001/01/01 00:00:00" || dateTimeValue == "1900/01/01" || dateTimeValue == "1900/01/01 00:00:00")
                    {
                        dateTimeValue = "NULL";
                    }
                    else
                    {
                        dateTimeValue = "'" + dateTimeValue + "'";
                    }
                    return dateTimeValue;

                }
                dateTimeValue = "NULL";
            }
            catch (Exception)
            {
                return "NULL";
            }
            return dateTimeValue;
        }

       
        public static IList<string> GetSqlsInBatches(string insertStatement, IList<string> insertValues)
        { 
            if (string.IsNullOrWhiteSpace(insertStatement) == false && insertValues != null)
            {
                insertValues = insertValues.Where(c => c != null).ToList();

                if (insertValues.Count> 0)
                {
                    var batchSize = 1000;
                    var sqlBatches = new List<string>();
                    var numberOfBatches = (Int32)Math.Ceiling((decimal)insertValues.Count / batchSize);

                    for (int i = 0; i < numberOfBatches; i++)
                    {
                        var valueToInsert = insertValues.Skip(i * batchSize).Take(batchSize);
                        sqlBatches.Add(insertStatement + string.Join(',', valueToInsert));
                    }

                    return sqlBatches;
                }
            }

            return null;
        }

        #endregion

        #region "Misc"
        public static string[] SplitTo(this string source, string separator, bool removeWhiteSpaceEntries = true, bool trimEntries = true)
        {
            if (source == null || separator == null) { return new string[] { }; }

            var s = source.Split(separator, removeWhiteSpaceEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
            if (removeWhiteSpaceEntries) { s = s.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(); }
            if (trimEntries) { s = s.Select(x => x.Trim()).ToArray(); }

            return s;
        }

        public static IEnumerable<T> SplitTo<T>(this string str, params string[] separator) where T : IConvertible
        {
            foreach (var s in str.Split(separator, StringSplitOptions.None))
                yield return (T)Convert.ChangeType(s, typeof(T));
        }

        public static long GetCurrentPage(long pageNo, long pageSize, long totalCount)
        {
            if (pageNo <= 0) { pageNo = 1; }
            if (pageSize <= 0) { pageSize = DefaultPageSize; }
            if (pageSize <= 0) { pageSize = 20; }
            if (totalCount <= 0)
            {
                pageNo = 1;
            }
            else
            {
                var n = totalCount / Convert.ToDecimal(pageSize);

                int totalPage;
                if (totalCount <= pageSize)
                {
                    totalPage = 1;
                }
                else
                {
                    if (Math.Truncate(n) == n && n.ToInt() != 0)
                    {
                        totalPage = Convert.ToInt32(Math.Truncate(n) + 0);
                    }
                    else
                    {
                        totalPage = Convert.ToInt32(Math.Truncate(n) + 1);
                    }
                }
                if (totalPage < pageNo) { pageNo = totalPage; }
            }
            return pageNo;
        }

        #endregion
    }
}
