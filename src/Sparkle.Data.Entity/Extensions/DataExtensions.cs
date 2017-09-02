
namespace System.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class DataExtensions
    {
        public static IDbCommand AddParameter(this IDbCommand command, string name, object value, DbType type)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.Value = value;
            param.DbType = type;
            command.Parameters.Add(param);
            return command;
        }
    }
}
