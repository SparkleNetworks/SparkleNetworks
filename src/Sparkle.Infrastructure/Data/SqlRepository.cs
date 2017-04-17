
#if SSC
namespace SparkleSystems.Configuration.Data
#else
namespace Sparkle.Infrastructure.Data
#endif
{
    using System;
    using System.Data.SqlClient;

    /// <summary>
    /// Base class for repositories using SQL Server.
    /// </summary>
    public class SqlRepository : IDisposable
    {
        private readonly string connectionString;
        private SqlConnection connection;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlRepository"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        protected SqlRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Gets the connection (and creates it).
        /// Dispose the objet to disconnect.
        /// </summary>
        protected SqlConnection Connection
        {
            get
            {
                if (this.connection == null)
                {
                    this.connection = new SqlConnection(this.connectionString);
                    this.connection.Open();
                }

                return this.connection;
            }
        }

        #region IDisposable members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.connection != null)
                    {
                        this.connection.Close();
                        this.connection = null;
                    }
                }

                this.disposed = true;
            }
        }

        #endregion
    }
}
