
#if SSC
namespace SparkleSystems.Configuration.Data
#else
namespace Sparkle.Infrastructure.Data
#endif
{
    using System;
    using System.Configuration;
    using System.Data;

    /// <summary>
    /// SQL Server implementation of <see cref="ISysLogRepository"/>.
    /// </summary>
    public class SqlSysLogRepository : SqlRepository, ISysLogRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSysLogRepository"/> class using the SparkleSystems.Logging connection string in config.
        /// </summary>
        public SqlSysLogRepository()
            : base(ConfigurationManager.ConnectionStrings["SparkleSystems.Logging"].ConnectionString)
        {
        }
        
        public SqlSysLogRepository(string connectionString) : base(connectionString)
        {
        }

        #region ISysLogRepository members

        public int FindApplicationId(string product, string host, string universe) {
            var command = this.Connection.CreateCommand();
            command.CommandText = "FindApplicationId";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("Product", product);
            command.Parameters.AddWithValue("Host", host);
            command.Parameters.AddWithValue("Universe", universe);
            var result = command.ExecuteScalar();
            if (result == null)
                throw new UnknownApplicationException(product, host, universe);

            return (int)result;
        }

        public Application FindApplication(string product, string host, string universe) {
            var command = this.Connection.CreateCommand();
            command.CommandText = "FindApplication";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("Product", product);
            command.Parameters.AddWithValue("Host", host);
            command.Parameters.AddWithValue("Universe", universe);
            using (var reader = command.ExecuteReader()) {
                if (reader.Read()) {
                    var app = new Application(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetInt32(3),
                        reader.GetInt32(5)) {
                            ProductName = reader.GetString(2),
                            UniverseName = reader.GetString(4),
                            HostName = reader.GetString(6),
                        };
                    return app;
                } else {
                    return null;
                }
            }
        }

        public void Write(short logEntryType, int applicationId, string applicationVersion, string path, string remoteClient, string identity, short errorLevel, string data)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (identity == null)
                throw new ArgumentNullException("identity");
            if (applicationVersion == null)
                throw new ArgumentNullException("applicationVersion");
            if (remoteClient == null)
                throw new ArgumentNullException("remoteClient");

            var command = this.Connection.CreateCommand();
            command.CommandText = "AddLogEntry";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("UtcDateTime", DateTime.UtcNow);
            command.Parameters.AddWithValue("Type", logEntryType);
            command.Parameters.AddWithValue("ApplicationId", applicationId);
            command.Parameters.AddWithValue("ApplicationVersion", applicationVersion);
            command.Parameters.AddWithValue("Path", path);
            command.Parameters.AddWithValue("RemoteClient", remoteClient);
            command.Parameters.AddWithValue("Identity", identity);
            command.Parameters.AddWithValue("Error", errorLevel);
            command.Parameters.AddWithValue("Data", data);
            var result = command.ExecuteNonQuery();

            if (result != 1)
                throw new DataException("AddLogEntry failed with return value " + result);
        }

        #endregion
    }
}
