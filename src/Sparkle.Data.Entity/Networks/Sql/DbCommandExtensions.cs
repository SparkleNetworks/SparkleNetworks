
namespace Sparkle.Data.Entity.Networks.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Data.Common;
    using System.Data;

    public static class DbCommandExtensions
    {
        public static IDbCommand SetText(this IDbCommand command, string commandText, bool isStoredProcedure = false)
        {
            command.CommandText = commandText;
            command.CommandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            return command;
        }

        public static IDbCommand AddParameter(this IDbCommand command, string name, object value)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.Value = value;
            command.Parameters.Add(param);
            return command;
        }

        public static DbConnection EnsureOpen(this DbConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            return connection;
        }
    }
}
