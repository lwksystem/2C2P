using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using ColumnAttribute = System.ComponentModel.DataAnnotations.Schema.ColumnAttribute;


namespace _2C2P.Core.Data
{
    public enum ColumnAction
    {
        Include,
        Exclude
    }

    public abstract class BaseModel<TModel> : BaseModel
        where TModel : BaseModel<TModel>
    {
        #region Fields
        // ReSharper disable once StaticMemberInGenericType
        protected static readonly IDictionary<string, DataType> DataTypes = new Dictionary<string, DataType>();
        #endregion

        #region Constructors
        static BaseModel()
        {
            LoadDataTypes();
        }
        #endregion

        private static void LoadDataTypes()
        {
            var properties = typeof(TModel).GetProperties(BindingFlags.Instance | BindingFlags.Public /*| BindingFlags.DeclaredOnly*/);
            foreach (var property in properties)
            {
                var column = property.GetCustomAttributes().OfType<ColumnAttribute>().FirstOrDefault();
                if (column != null)
                {
                    DataTypes.Add(property.Name, GetDataType(property.Name));
                }
            }
        }

        public static string TableName()
        {
            return ((TableAttribute)typeof(TModel).GetCustomAttribute(typeof(TableAttribute), false))?.Name;
        }

        public static string TableName(bool withNoLock)
        {
            string noLock = withNoLock ? " WITH (NOLOCK)" : "";
            return $"{TableName()}{noLock}";
        }

        public static string TableName(string alias)
        {
            return $"{TableName()} AS {alias}";
        }

        public static string TableName(string alias, bool withNoLock)
        {
            string noLock = withNoLock ? " WITH (NOLOCK)" : "";
            return $"{TableName()} AS {alias}{noLock}";
        }

        public static string TableColumnName<TKey>(Expression<Func<TModel, TKey>> column)
        {
            return TableColumnName(TableName(), column);
        }

        public static string TableColumnName<TKey>(string tableAlias, Expression<Func<TModel, TKey>> column)
        {
            return $"{tableAlias}.{Field(column).ColumnName}";
        }

        public static string ColumnName<TKey>(Expression<Func<TModel, TKey>> keySelector)
        {
            return Field(keySelector).ColumnName;
        }

        public static DataType Field<TKey>(Expression<Func<TModel, TKey>> keySelector)
        {
            var member = keySelector.Body as MemberExpression;
            var unary = keySelector.Body as UnaryExpression;
            var memberExpression = member ?? unary?.Operand as MemberExpression;

            if (memberExpression != null)
            {
                var propertyName = memberExpression.Member.Name;

                DataType dataType;
                DataTypes.TryGetValue(propertyName, out dataType);

                return dataType;
            }

            return null;
        }

        private static DataType GetDataType(string propertyName)
        {
            var dataType = new DataType();
            var property = typeof(TModel).GetProperties().FirstOrDefault(x => x.Name == propertyName);

            if (property != null)
            {
                var requiredAttribute = property.GetCustomAttributes().OfType<RequiredAttribute>().FirstOrDefault();
                if (requiredAttribute != null)
                {
                    dataType.Required = true;
                    dataType.EmptyString = requiredAttribute.AllowEmptyStrings;
                    dataType.RequiredErrMsg = requiredAttribute.ErrorMessage;
                }

                var columnAttribute = property.GetCustomAttributes().OfType<ColumnAttribute>().FirstOrDefault();
                if (columnAttribute != null)
                {
                    dataType.ColumnName = columnAttribute.Name;
                }

                var stringLengthAttribute = property.GetCustomAttributes().OfType<StringLengthAttribute>().FirstOrDefault();
                if (stringLengthAttribute != null)
                {
                    dataType.MinLen = stringLengthAttribute.MinimumLength;
                    dataType.MaxLen = stringLengthAttribute.MaximumLength;
                    dataType.LenErrMsg = stringLengthAttribute.ErrorMessage;
                }

                var displayAttribute = property.GetCustomAttributes().OfType<DisplayAttribute>().FirstOrDefault();
                if (displayAttribute != null)
                {
                    dataType.Description = displayAttribute.Name;
                }

                var rangeAttribute = property.GetCustomAttributes().OfType<RangeAttribute>().FirstOrDefault();
                if (rangeAttribute != null)
                {
                    if (property.PropertyType == typeof(DateTime))
                    {
                        dataType.MinDate = Convert.ToDateTime(rangeAttribute.Minimum);
                        dataType.MaxDate = Convert.ToDateTime(rangeAttribute.Maximum);
                    }
                    else if (property.PropertyType == typeof(decimal))
                    {
                        dataType.MinDecimal = Convert.ToDecimal(rangeAttribute.Minimum);
                        dataType.MaxDecimal = Convert.ToDecimal(rangeAttribute.Maximum);
                    }
                    else if (property.PropertyType == typeof(double))
                    {
                        dataType.MinDouble = Convert.ToDouble(rangeAttribute.Maximum);
                        dataType.MaxDouble = Convert.ToDouble(rangeAttribute.Maximum);
                    }
                    else if (property.PropertyType == typeof(long))
                    {
                        dataType.MinLong = Convert.ToInt64(rangeAttribute.Minimum);
                        dataType.MaxLong = Convert.ToInt64(rangeAttribute.Maximum);
                    }
                    else
                    {
                        dataType.MinInteger = Convert.ToInt32(rangeAttribute.Minimum);
                        dataType.MaxInteger = Convert.ToInt32(rangeAttribute.Maximum);
                    }
                }

                dataType.ColumnType = property.PropertyType;
            }

            return dataType;
        }

        public override void Validate(ColumnAction action, params DataType[] columns)
        {
            var columnDictionary = DataTypes.ToDictionary(x => GetType().GetProperties().FirstOrDefault(y => y.Name == x.Key), x => x.Value);

            if (columns == null)
            {
                columns = new DataType[] { };
            }

            columnDictionary = columnDictionary.Where(x =>
            {
                return action == ColumnAction.Include ? columns.Select(y => y.ColumnName).Contains(x.Value.ColumnName) : !columns.Select(y => y.ColumnName).Contains(x.Value.ColumnName);
            }).ToDictionary(x => x.Key, x => x.Value);

            foreach (var column in columnDictionary)
            {
                Validate(column.Value, column.Key.GetValue(this));
            }
        }
    }

    public abstract class BaseModel //: IEquatable<ModelBase>
    {
        [Editable(false)]
        public bool IsNew { get; set; }

        [Column("*")]
        [Editable(false)]
        public string All { get; set; }

        public static void Validate(DataType dataType, object value)
        {
            if (dataType.ColumnType == typeof(string))
            {
                string errorMessage;
                if (dataType.Required)
                {
                    if (value == null || value.ToString().Trim() == string.Empty)
                    {
                        errorMessage = string.IsNullOrWhiteSpace(dataType.RequiredErrMsg)
                            ? "The " + dataType.Description + " field is required."
                            : dataType.RequiredErrMsg;
                        throw new AppException(errorMessage);
                    }
                }
                if (dataType.MinLen > 0)
                {
                    if (value != null && value.ToString().Length < dataType.MinLen)
                    {
                        errorMessage = string.IsNullOrWhiteSpace(dataType.LenErrMsg)
                            ? "The field " + dataType.Description + " must be a string with minimum length of." +
                              dataType.MinLen
                            : dataType.LenErrMsg;
                        throw new AppException(errorMessage);
                    }
                }
                if (dataType.MaxLen > 0)
                {
                    if (value != null && value.ToString().Length > dataType.MaxLen)
                    {
                        errorMessage = string.IsNullOrWhiteSpace(dataType.LenErrMsg)
                            ? "The field " + dataType.Description + " must be a string with maximum length of." +
                              dataType.MaxLen
                            : dataType.LenErrMsg;
                        throw new AppException(errorMessage);
                    }
                }
            }
            else if (dataType.ColumnType == typeof(int))
            {
                if (!value.ToString().IsNumeric()) { throw new AppException("The value of " + dataType.Description + " is invalid."); }
                if (dataType.Required)
                {
                    if (value.ToInt() <= 0)
                    {
                        throw new AppException("The field " + dataType.Description + " must be greater than or equal zero.");
                    }
                }

                if (int.Parse(value.ToString(), System.Globalization.NumberStyles.Number) < dataType.MinInteger)
                {
                    throw new AppException("Minimum value of + " + dataType.Description + " is " + dataType.MinInteger + ".");
                }

                if (int.Parse(value.ToString(), System.Globalization.NumberStyles.Number) > dataType.MaxInteger)
                {
                    throw new AppException("Maximum value of + " + dataType.Description + " is " + dataType.MaxInteger + ".");
                }
            }
            else if (dataType.ColumnType == typeof(short))
            {
                if (value.ToString().IsNumeric() == false) { throw new AppException("The value of " + dataType.Description + " is invalid."); }
                if (dataType.Required)
                {
                    if (value.ToShort() <= 0)
                    {
                        throw new AppException("The field " + dataType.Description + " must be greater than or equal zero.");
                    }
                }

                if (int.Parse(value.ToString(), System.Globalization.NumberStyles.Number) < dataType.MinInteger)
                {
                    throw new AppException("Minimum value of " + dataType.Description + " is " + dataType.MinInteger + ".");
                }

                if (int.Parse(value.ToString(), System.Globalization.NumberStyles.Number) > dataType.MaxInteger)
                {
                    throw new AppException("Maximum value of " + dataType.Description + " is " + dataType.MaxInteger + ".");
                }
            }
            else if (dataType.ColumnType == typeof(double))
            {
                if (!value.ToString().IsNumeric())
                {
                    throw new AppException("The value of " + dataType.Description + " is invalid.");
                }

                if (dataType.Required)
                {
                    if (Convert.ToDouble(value) <= 0)
                    {
                        throw new AppException("The field " + dataType.Description + " must be greater than or equal zero.");
                    }
                }
                if (Convert.ToDouble(value) < dataType.MinDouble)
                {
                    throw new AppException("Minimum value of " + dataType.Description + " is " + dataType.MinDouble + ".");
                }

                if (Convert.ToDouble(value) > dataType.MaxDouble)
                {
                    throw new AppException("Maximum value of " + dataType.Description + " is " + dataType.MaxDouble + ".");
                }
            }
            else if (dataType.ColumnType == typeof(decimal))
            {
                if (!value.ToString().IsNumeric())
                {
                    throw new AppException("The value of " + dataType.Description + " is invalid.");
                }

                if (dataType.Required)
                {
                    if (Convert.ToDecimal(value) <= 0)
                    {
                        throw new AppException("The field " + dataType.Description + " must be greater than or equal zero.");
                    }
                }
                if (Convert.ToDecimal(value) < dataType.MinDecimal)
                {
                    throw new AppException("Minimum value of " + dataType.Description + " is " + dataType.MinDecimal + ".");
                }

                if (Convert.ToDecimal(value) > dataType.MaxDecimal)
                {
                    throw new AppException("Maximum value of " + dataType.Description + " is " + dataType.MaxDecimal + ".");
                }
            }
            else if (dataType.ColumnType == typeof(DateTime))
            {
                if (dataType.Required)
                {
                    var dateTime = Convert.ToDateTime(value);
                    if (dateTime == default(DateTime))
                    {
                        throw new AppException("The " + dataType.Description + " field is required.");
                    }
                }
            }
            else if (value is Enum)
            {
                if (dataType.Required)
                {
                    if (Enum.IsDefined(dataType.ColumnType, value) == false)
                    {
                        throw new AppException("The " + dataType.Description + " field is invalid.");
                    }
                }
            }
        }

        public void Validate()
        {
            Validate(ColumnAction.Exclude);
        }

        public abstract void Validate(ColumnAction action, params DataType[] columns);


    }
}
