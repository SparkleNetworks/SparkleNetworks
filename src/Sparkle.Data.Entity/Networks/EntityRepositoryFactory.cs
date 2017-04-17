
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Entity.Networks.Sql;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Data;
    using System.Data.EntityClient;
    using System.Data.Metadata.Edm;
    using System.Data.SqlClient;
    using System.Diagnostics;

    [DebuggerStepThrough]
    public partial class EntityRepositoryFactory : IDisposable
    {
        private bool disposed;
        private NetworksEntities context;
        private Func<NetworksEntities> factory;
        private readonly DataDisposable disposable = new DataDisposable();
        private string transaction;

        [System.Diagnostics.DebuggerStepThrough]
        public EntityRepositoryFactory(string connectionString)
        {
            if (!connectionString.StartsWith("metadata="))
            {
                connectionString = "metadata=res://Sparkle.Data.Entity/Networks.Model.NetworksModel.csdl|res://Sparkle.Data.Entity/Networks.Model.NetworksModel.ssdl|res://Sparkle.Data.Entity/Networks.Model.NetworksModel.msl;provider=System.Data.SqlClient;provider connection string=\"" + connectionString.Replace("\"", "\\\"") + "\"";
            }

            this.factory = () =>
            {
                var obj = new NetworksEntities(connectionString);
                obj.ContextOptions.LazyLoadingEnabled = true;
                return obj;
            };
            this.context = this.factory();
        }

        [System.Diagnostics.DebuggerStepThrough]
        public EntityRepositoryFactory(NetworksEntities context, Func<NetworksEntities> factory)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.context = context;
            this.factory = factory;
        }

        public IDbCommand CreateStoreCommand()
        {
            var context = (this.context ?? (this.context = this.factory()));
            var connection = (EntityConnection)context.Connection;

            return connection.StoreConnection.EnsureOpen().CreateCommand();
        }

        public IRepositoryFactory Clone()
        {
            return new EntityRepositoryFactory(this.factory(), this.factory);
        }

        public IDataTransaction NewTransaction()
        {
            var transaction = new EntityDataTransaction(this.factory(), this.factory);
            return transaction;
        }

        public IDataTransaction NewTransaction(IDataTransaction parentTransaction)
        {
            if (parentTransaction == null)
                throw new ArgumentNullException("parentTransaction");

            ////var connection = new EntityConnection(new MetadataWorkspace(), parentTransaction.EntityConnection);
            var connection = (EntityConnection)parentTransaction.EntityConnection;
            var entities = new NetworksEntities(connection, true);
            var transaction = new EntityDataTransaction(entities, null);
            return transaction;
        }

        private string CreateTransactionName()
        {
            return "tran" + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 28);
        }

        public void BeginTransaction(IsolationLevel isolation)
        {
            if (this.context != null)
            {
                if (this.transaction == null)
                {
                    if (this.context.Connection.State == System.Data.ConnectionState.Closed)
                    {
                        this.context.Connection.Open();
                    }

                    var connection = ((EntityConnection)this.context.Connection).StoreConnection;
                    this.transaction = this.CreateTransactionName();

                    if (isolation != IsolationLevel.Unspecified)
                    {
                        var isolationTitle = "";
                        switch (isolation)
                        {
                            case IsolationLevel.ReadCommitted:
                                isolationTitle = "READ COMMITED";
                                break;
                            case IsolationLevel.ReadUncommitted:
                                isolationTitle = "READ UNCOMMITED";
                                break;
                            case IsolationLevel.RepeatableRead:
                                isolationTitle = "REPEATABLE READ";
                                break;
                            case IsolationLevel.Unspecified:
                            case IsolationLevel.Chaos:
                            case IsolationLevel.Serializable:
                            case IsolationLevel.Snapshot:
                            default:
                                throw new InvalidOperationException("Isolation level " + isolation.ToString() + " is not implemented.");
                        }

                        var isolationCommand = connection.CreateCommand();
                        isolationCommand.CommandType = System.Data.CommandType.Text;
                        isolationCommand.CommandText = "SET TRANSACTION ISOLATION LEVEL " + isolationTitle;
                        isolationCommand.ExecuteNonQuery();
                    }
                    
                    var command = connection.CreateCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "BEGIN TRANSACTION " + this.transaction;
                    command.ExecuteNonQuery();
                }
                else
                {
                    throw new InvalidOperationException("Transaction already opened");
                }
            }
            else
            {
            }
        }

        public void CompleteTransaction()
        {
            if (this.context != null)
            {
                this.context.SaveChanges();
                if (this.transaction != null)
                {
                    var connection = ((EntityConnection)this.context.Connection).StoreConnection;
                    var transaction = this.transaction;
                    this.transaction = null;

                    var command = connection.CreateCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "COMMIT TRANSACTION " + transaction;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (InvalidOperationException ex)
                    {
                        if (ex.HResult == -2146233079)
                        {
                            // Handle the "There is already an open DataReader associated with this Command which must be closed first." case
                            // something has broken the transaction. it will be hard to find the issue.
                            // in this case we want to kill the connection to avoid keeping the database in a locked state. sorry.
                            connection.Dispose();
                        }
                        else
                        {
                            // not sure what happend. please document this case
                            // kill the connection to avoid keeping the database in a locked state. sorry.
                            connection.Dispose();
                        }

                        throw;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Transaction already closed");
                }
            }
        }

        public void AbortTransaction()
        {
            this.AbortTransaction(true);
        }

        public void AbortTransaction(bool throwIfAlreadyClosed)
        {
            if (this.context != null)
            {
                if (this.transaction != null)
                {
                    var connection = ((EntityConnection)this.context.Connection).StoreConnection;
                    this.transaction = null;

                    var command = connection.CreateCommand();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "ROLLBACK TRANSACTION";
                    command.ExecuteNonQuery();
                }
                else if (throwIfAlreadyClosed)
                {
                    throw new InvalidOperationException("Transaction already closed");
                }
            }
        }

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
                    this.disposable.Dispose();

                    if (this.transaction != null)
                    {
                        this.AbortTransaction();
                    }

                    if (this.context != null)
                    {
                        this.context.Dispose();
                        this.context = null;
                    }

                    this.factory = null;
                }

                this.disposed = true;
            }
        }
    }
}
