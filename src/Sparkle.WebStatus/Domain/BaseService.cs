
namespace Sparkle.WebStatus.Domain
{
    using Newtonsoft.Json;
    using SrkToolkit;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;

    public class BaseService<TData> : BaseService
        where TData : new()
    {
        private readonly string serviceName;
        private readonly ServiceConfiguration configuration;
        private readonly ServiceFactory factory;

        protected BaseService(ServiceConfiguration configuration, string serviceName)
            : base(null, serviceName)
        {
            this.serviceName = serviceName;
            this.configuration = configuration;
        }

        protected BaseService(ServiceFactory factory, string serviceName)
            : base(factory, serviceName)
        {
            this.serviceName = serviceName;
            this.factory = factory;
            this.configuration = factory.Configuration;
        }

        protected Dictionary<Guid, TData> Read()
        {
            var path = Path.Combine(this.configuration.DataDirectory, serviceName + ".json");
            if (!File.Exists(path))
            {
                return new Dictionary<Guid, TData>();
            }

            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = new StreamReader(fileStream, Encoding.Unicode);
                var json = reader.ReadToEnd();
                var data = JsonConvert.DeserializeObject<DataTable<TData>>(json);
                return data.Items;
            }
        }

        protected Transaction<TData> Write()
        {
            var path = Path.Combine(this.configuration.DataDirectory, serviceName + ".json");

            if (!File.Exists(path))
            {
                var data = this.CreateDataTable();

                using (var fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    var json = JsonConvert.SerializeObject(data);
                    var writer = new StreamWriter(fileStream, Encoding.Unicode);
                    writer.Write(json);
                    writer.Flush();
                }
            }

            {
                var fileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                var transaction = new Transaction<TData>(fileStream);

                return transaction;
            }
        }

        private DataTable<TData> CreateDataTable()
        {
            return new DataTable<TData>()
            {
                Name = this.serviceName,
                Items = new Dictionary<Guid, TData>(),
                DateCreatedUtc = DateTime.UtcNow,
                DateUpdatedUtc = DateTime.UtcNow,
                Updates = 0,
            };
        }

        public class Transaction<TData> : IDisposable
        {
            private readonly FileStream stream;
            private bool isDisposed;
            private DataTable<TData> data;

            public Transaction(FileStream stream)
            {
                this.stream = stream;
            }

            public Dictionary<Guid, TData> Items
            {
                get
                {
                    this.Read();
                    return this.data.Items;
                }
            }

            public void Save()
            {
                if (this.isDisposed)
                    throw new ObjectDisposedException(this.ToString());

                this.data.DateUpdatedUtc = DateTime.UtcNow;
                this.data.Updates++;

                this.stream.Seek(0L, SeekOrigin.Begin);
                this.stream.SetLength(0L);
                var json = JsonConvert.SerializeObject(this.data);
                var writer = new StreamWriter(this.stream, Encoding.Unicode);
                writer.Write(json);
                writer.Flush();

                this.Close();
                this.Dispose();
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Close()
            {
                this.stream.Dispose();
            }

            private void Read()
            {
                if (this.isDisposed)
                    throw new ObjectDisposedException(this.ToString());

                if (this.data == null)
                {
                    this.stream.Seek(0L, SeekOrigin.Begin);
                    var reader = new StreamReader(this.stream, Encoding.Unicode);
                    var json = reader.ReadToEnd();
                    this.data = JsonConvert.DeserializeObject<DataTable<TData>>(json);
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!this.isDisposed)
                {
                    if (disposing)
                    {
                        this.Close();
                        this.data = null;
                    }

                    this.isDisposed = true;
                }
            }
        }

        public class DataTable<TData>
        {
            public DataTable()
            {
            }

            public string Name { get; set; }
            public DateTime DateCreatedUtc { get; set; }
            public DateTime DateUpdatedUtc { get; set; }
            public long Updates { get; set; }
            public Dictionary<Guid, TData> Items { get; set; }
        }
    }
}