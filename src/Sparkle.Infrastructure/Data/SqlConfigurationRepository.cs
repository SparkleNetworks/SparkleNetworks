
#if SSC
namespace SparkleSystems.Configuration.Data
#else
namespace Sparkle.Infrastructure.Data
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
#if !SSC
    using Sparkle.Infrastructure.Data.Objects;
#endif

    /// <summary>
    /// SQL Server implementation of <see cref="IConfigurationRepository"/>.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SqlConfigurationRepository : SqlRepository, IConfigurationRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConfigurationRepository"/> class using the SparkleSystems.AppConfiguration connection string in config.
        /// </summary>
        public SqlConfigurationRepository()
            : base(ConfigurationManager.ConnectionStrings["SparkleSystems.AppConfiguration"].ConnectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConfigurationRepository"/> class using the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlConfigurationRepository(string connectionString)
            : base(connectionString)
        {
        }

        #region IConfigurationRepository members

        public int FindApplicationId(string product, string host, string universe)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "FindApplicationId";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("Product", product);
            command.Parameters.AddWithValue("Host", host);
            command.Parameters.AddWithValue("Universe", universe);
            var result = command.ExecuteScalar();
            if (result == null || result is DBNull)
                throw new UnknownApplicationException(product, host, universe);

            return (int)result;
        }

        public int FindApplicationIdByDomainName(string product, string host, string domainName)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "FindApplicationIdByDomainName";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("Product", product);
            command.Parameters.AddWithValue("Host", host);
            command.Parameters.AddWithValue("DomainName", domainName);
            var result = command.ExecuteScalar();
            if (result == null)
                throw new UnknownApplicationException(product, host, domainName);

            return (int)result;
        }

        public Application FindApplicationById(int applicationId)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "FindApplicationById2";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("ApplicationId", applicationId);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    // A.[Id], A.Status,
                    //     0       1
                    // P.Id ProductId, P.Name ProductName,
                    //         2                3
                    // U.Id UniverseId, U.Name UniverseName, U.Status UniverseStatus,
                    //         4                5                       6
                    // H.Id HostId, H.Name HostName
                    //         7                8
                    var entry = new Application(
                        reader.GetInt32(0),
                        reader.GetInt32(2),
                        reader.GetInt32(4),
                        reader.GetInt32(7))
                    {
                        Status = reader.GetInt16(1),
                        ProductName = reader.GetString(3),
                        UniverseName = reader.GetString(5),
                        HostName = reader.GetString(8),
                        UniverseStatus = reader.GetInt16(6),
                    };
                    return entry;
                }
                else
                {
                    return null;
                }
            }
        }

        public IDictionary<string, AppConfigurationEntry> FetchValues(int applicationId)
        {
            var list = new Dictionary<string, AppConfigurationEntry>();

            var command = this.Connection.CreateCommand();
            command.CommandText = "GetConfigurationValuesForApplication";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("ApplicationId", applicationId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int keyId = reader.GetInt32(1);
                    string key = reader.GetString(2);
                    AppConfigurationEntry entry = null;
                    if (list.ContainsKey(key))
                    {
                        entry = list[key];
                    }
                    else
                    {
                        entry = new AppConfigurationEntry
                        {
                            Id = id,
                            KeyId = keyId,
                            Key = key,
                            BlittableType = reader.GetString(3),
                            Summary = reader.GetString(4),
                            IsRequired = reader.GetBoolean(5),
                            IsCollection = reader.GetBoolean(6),
                            DefaultRawValue = reader.IsDBNull(7) ? null : reader.GetString(7),
                        };
                        list.Add(key, entry);
                    }

                    if (entry.IsCollection)
                    {
                        if (entry.RawValues == null)
                            entry.RawValues = new List<string>();

                        if (!reader.IsDBNull(8))
                            entry.RawValues.Add(reader.GetString(8));
                    }
                    else
                    {
                        var coverride = ConfigurationManager.AppSettings["Override." + key];
                        if (coverride != null)
                            entry.RawValue = coverride;
                        else if (!reader.IsDBNull(8))
                            entry.RawValue = reader.GetString(8);
                    }
                }
            }

            return list;
        }

        public IList<AppConfigurationEntry> FetchKeys()
        {
            var list = new List<AppConfigurationEntry>();

            var command = this.Connection.CreateCommand();
            command.CommandText = "GetConfigurationKeys";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int keyId = reader.GetInt32(0);
                    string key = reader.GetString(1);
                    AppConfigurationEntry entry = null;
                    entry = new AppConfigurationEntry
                    {
                        KeyId = keyId,
                        Key = key,
                        BlittableType = reader.GetString(2),
                        Summary = reader.GetString(3),
                        IsRequired = reader.GetBoolean(4),
                        IsCollection = reader.GetBoolean(5),
                        DefaultRawValue = reader.IsDBNull(6) ? null : reader.GetString(6),
                    };
                    list.Add(entry);
                }
            }

            return list;
        }

        public IList<Application> FindApplications(string product, string host)
        {
            var list = new List<Application>();

            var command = this.Connection.CreateCommand();
            command.CommandText = "FindApplications2";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("Product", product); // @Product nvarchar(48),
            command.Parameters.AddWithValue("Host", host);       // @Host nvarchar(48)
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // A.[Id], A.Status,
                    //     0       1
                    // P.Id ProductId, P.Name ProductName,
                    //         2                3
                    // U.Id UniverseId, U.Name UniverseName, U.Status UniverseStatus,
                    //         4                5                       6
                    // H.Id HostId, H.Name HostName
                    //         7                8
                    var entry = new Application(
                        reader.GetInt32(0),
                        reader.GetInt32(2),
                        reader.GetInt32(4),
                        reader.GetInt32(7))
                    {
                        Status = reader.GetInt16(1),
                        ProductName = reader.GetString(3),
                        UniverseName = reader.GetString(5),
                        HostName = reader.GetString(8),
                        UniverseStatus = reader.GetInt16(6),
                    };
                    list.Add(entry);
                }
            }

            return list;
        }

        public Application FindApplication(string product, string host, string universe)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "FindApplication2";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("Product", product); // @Product nvarchar(48),
            command.Parameters.AddWithValue("Host", host);       // @Host nvarchar(48)
            command.Parameters.AddWithValue("Universe", universe);       // @Universe nvarchar(48)
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // A.[Id], A.Status,
                    //     0       1
                    // P.Id ProductId, P.Name ProductName,
                    //         2                3
                    // U.Id UniverseId, U.Name UniverseName, U.Status UniverseStatus,
                    //         4                5                       6
                    // H.Id HostId, H.Name HostName
                    //         7                8
                    var entry = new Application(
                        reader.GetInt32(0),
                        reader.GetInt32(2),
                        reader.GetInt32(4),
                        reader.GetInt32(7))
                    {
                        Status = reader.GetInt16(1),
                        ProductName = reader.GetString(3),
                        UniverseName = reader.GetString(5),
                        HostName = reader.GetString(8),
                        UniverseStatus = reader.GetInt16(6),
                    };
                    return entry;
                }
            }

            return null;
        }

        public IList<Application> GetAllApplications()
        {
            var list = new List<Application>();

            var command = this.Connection.CreateCommand();
            command.CommandText = "SELECT * FROM dbo.ApplicationsView2";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // A.[Id], A.Status,
                    //     0       1
                    // P.Id ProductId, P.Name ProductName,
                    //         2                3
                    // U.Id UniverseId, U.Name UniverseName, U.Status UniverseStatus,
                    //         4                5                       6
                    // H.Id HostId, H.Name HostName
                    //         7                8
                    var entry = new Application(
                        reader.GetInt32(0),
                        reader.GetInt32(2),
                        reader.GetInt32(4),
                        reader.GetInt32(7))
                    {
                        Status = reader.GetInt16(1),
                        ProductName = reader.GetString(3),
                        UniverseName = reader.GetString(5),
                        HostName = reader.GetString(8),
                        UniverseStatus = reader.GetInt16(6),
                    };
                    list.Add(entry);
                }
            }

            return list;
        }

        public IList<Host> GetAllHosts()
        {
            var list = new List<Host>();

            var command = this.Connection.CreateCommand();
            command.CommandText = "SELECT Id, Name FROM dbo.Hosts";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var entry = new Host
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                    };
                    list.Add(entry);
                }
            }

            return list;
        }

        public IList<Universe> GetAllUniverses()
        {
            var list = new List<Universe>();

            var command = this.Connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, DisplayName, Status FROM dbo.Universes";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var entry = new Universe
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        DisplayName = reader.GetString(2),
                        Status = reader.GetInt16(3),
                    };
                    list.Add(entry);
                }
            }

            return list;
        }

        public IList<Product> GetAllProducts()
        {
            var list = new List<Product>();

            var command = this.Connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, DisplayName FROM dbo.Products";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var entry = new Product
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        DisplayName = reader.GetString(2),
                    };
                    list.Add(entry);
                }
            }

            return list;
        }

        public Application CreateApplication(int product, int host, int universe, short status)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "INSERT INTO dbo.Applications (ProductId, HostId, UniverseId, Status) VALUES (@0, @1, @2, @3);SELECT SCOPE_IDENTITY() AS NewID;";
            command.Parameters.AddWithValue("@0", product);
            command.Parameters.AddWithValue("@1", host);
            command.Parameters.AddWithValue("@2", universe);
            command.Parameters.AddWithValue("@3", status);
            var obj = command.ExecuteScalar();
            var id = System.Convert.ToInt32(obj);
            return this.FindApplicationById(id);
        }

        public AppConfigurationEntry GetValue(int applicationId, int keyId, int index)
        {
            AppConfigurationEntry entry = null;

            var command = this.Connection.CreateCommand();
            command.CommandText = "SELECT Id, ConfigKeyId, [Index], Value, ApplicationId " +
                "FROM dbo.ApplicationConfigValues " +
                "WHERE ApplicationId = @0 AND ConfigKeyId = @1 AND [Index] = @2";
            command.Parameters.AddWithValue("@0", applicationId);
            command.Parameters.AddWithValue("@1", keyId);
            command.Parameters.AddWithValue("@2", index);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    entry = new AppConfigurationEntry
                    {
                        Id = reader.GetInt32(0),
                        KeyId = reader.GetInt32(1),
                        RawValue = reader.GetString(3),
                    };

                    break;
                }
            }

            return entry;
        }

        public IList<UniverseDomainName> GetUniversesDomainNames(int universeId)
        {
            var list = new List<UniverseDomainName>();

            var command = this.Connection.CreateCommand();
            command.CommandText = "GetUniversesDomainNames";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("UniverseId", universeId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Id, UniverseId, DomainName, RedirectToMain
                    var entry = new UniverseDomainName
                    {
                        Id = reader.GetInt32(0),
                        UniverseId = reader.GetInt32(1),
                        Name = reader.GetString(2),
                        RedirectToMain = reader.GetBoolean(3),
                    };
                    list.Add(entry);
                }
            }

            return list;
        }

        public int SetValue(int applicationId, int keyId, int index, string rawValue)
        {
            var entry = this.GetValue(applicationId, keyId, index);

            if (entry != null)
            {
                var command = this.Connection.CreateCommand();
                command.CommandText = "UPDATE dbo.ApplicationConfigValues " +
                    "SET Value = @0 " +
                    "WHERE ApplicationId = @1 AND ConfigKeyId = @2 AND [Index] = @3";
                command.Parameters.AddWithValue("@0", rawValue);
                command.Parameters.AddWithValue("@1", applicationId);
                command.Parameters.AddWithValue("@2", keyId);
                command.Parameters.AddWithValue("@3", index);
                var obj = command.ExecuteNonQuery();
                return entry.Id;
            }
            else
            {
                var command = this.Connection.CreateCommand();
                command.CommandText = "INSERT INTO dbo.ApplicationConfigValues " +
                    "(ApplicationId, ConfigKeyId, [Index], Value) VALUES " +
                    "(@0, @1, @2, @3);" +
                    "SELECT SCOPE_IDENTITY() AS NewID;";
                command.Parameters.AddWithValue("@0", applicationId);
                command.Parameters.AddWithValue("@1", keyId);
                command.Parameters.AddWithValue("@2", index);
                command.Parameters.AddWithValue("@3", rawValue);
                var obj = command.ExecuteScalar();
                var id = System.Convert.ToInt32(obj);
                return id;
            }
        }

        public int AddKey(string name, string blitableType, string summary, bool isRequired, bool isCollection, string defaultValue)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "INSERT INTO dbo.[ConfigKeys] " +
                "([Name],[BlitableType],[Summary],[IsRequired],[IsCollection],[DefaultValue]) VALUES " +
                "(@0, @1, @2, @3, @4, @5);" +
                "SELECT SCOPE_IDENTITY() AS NewID;";
            command.Parameters.AddWithValue("@0", name);
            command.Parameters.AddWithValue("@1", blitableType);
            command.Parameters.AddWithValue("@2", summary);
            command.Parameters.AddWithValue("@3", isRequired);
            command.Parameters.AddWithValue("@4", isCollection);
            command.Parameters.AddWithValue("@5", (object)defaultValue ?? DBNull.Value);
            var obj = command.ExecuteScalar();
            var id = System.Convert.ToInt32(obj);
            return id;
        }

        public void UpdateKey(int id, string name, string blitableType, string summary, bool isRequired, bool isCollection, string defaultValue)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "UPDATE dbo.[ConfigKeys] SET " +
                "[Name] = @0, " +
                "[BlitableType] = @1, " +
                "[Summary] = @2, " +
                "[IsRequired] = @3, " +
                "[IsCollection] = @4, " +
                "[DefaultValue] = @5 " +
                "WHERE [Id] = @6";
            command.Parameters.AddWithValue("@0", name);
            command.Parameters.AddWithValue("@1", blitableType);
            command.Parameters.AddWithValue("@2", summary);
            command.Parameters.AddWithValue("@3", isRequired);
            command.Parameters.AddWithValue("@4", isCollection);
            command.Parameters.AddWithValue("@5", (object)defaultValue ?? DBNull.Value);
            command.Parameters.AddWithValue("@6", id);
            var obj = command.ExecuteNonQuery();
        }

        public void DeleteValue(int applicationId, int keyId, int index)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "DELETE FROM dbo.ApplicationConfigValues " +
                "WHERE ApplicationId = @0 AND ConfigKeyId = @1 AND [Index] = @2";
            command.Parameters.AddWithValue("@0", applicationId);
            command.Parameters.AddWithValue("@1", keyId);
            command.Parameters.AddWithValue("@2", index);
            var obj = command.ExecuteNonQuery();
        }

        public int AddProduct(string name, string displayName)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "INSERT INTO dbo.[Products] " +
                "([Name],[DisplayName]) VALUES " +
                "(@0, @1);" +
                "SELECT SCOPE_IDENTITY() AS NewID;";
            command.Parameters.AddWithValue("@0", name);
            command.Parameters.AddWithValue("@1", displayName);
            var obj = command.ExecuteScalar();
            var id = System.Convert.ToInt32(obj);
            return id;
        }

        public void UpdateProduct(int id, string name, string displayName)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "UPDATE dbo.[Products] SET " +
                "[Name] = @0, " +
                "[DisplayName] = @1 " +
                "WHERE [Id] = @2";
            command.Parameters.AddWithValue("@0", name);
            command.Parameters.AddWithValue("@1", displayName);
            command.Parameters.AddWithValue("@2", id);
            var obj = command.ExecuteNonQuery();
        }

        public int AddUniverse(string name, string displayName, short status)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "INSERT INTO dbo.[Universes] " +
                "([Name],[DisplayName],[Status]) VALUES " +
                "(@0, @1, @2);" +
                "SELECT SCOPE_IDENTITY() AS NewID;";
            command.Parameters.AddWithValue("@0", name);
            command.Parameters.AddWithValue("@1", displayName);
            command.Parameters.AddWithValue("@2", status);
            var obj = command.ExecuteScalar();
            var id = System.Convert.ToInt32(obj);
            return id;
        }

        public void UpdateUniverse(int id, string name, string displayName, short status)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "UPDATE dbo.[Universes] SET " +
                "[Name] = @0, " +
                "[DisplayName] = @1, " +
                "[Status] = @2 " +
                "WHERE [Id] = @3";
            command.Parameters.AddWithValue("@0", name);
            command.Parameters.AddWithValue("@1", displayName);
            command.Parameters.AddWithValue("@2", status);
            command.Parameters.AddWithValue("@3", id);
            var obj = command.ExecuteNonQuery();
        }

        public void ChangeUniverseStatus(int id, short status)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "UPDATE dbo.[Universes] SET " +
                "[Status] = @0 " +
                "WHERE [Id] = @1";
            command.Parameters.AddWithValue("@0", status);
            command.Parameters.AddWithValue("@1", id);
            var obj = command.ExecuteNonQuery();
        }

        public int AddHost(string name)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "INSERT INTO dbo.[Hosts] " +
                "([Name]) VALUES " +
                "(@0);" +
                "SELECT SCOPE_IDENTITY() AS NewID;";
            command.Parameters.AddWithValue("@0", name);
            var obj = command.ExecuteScalar();
            var id = System.Convert.ToInt32(obj);
            return id;
        }

        public void UpdateHost(int id, string name)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "UPDATE dbo.[Hosts] SET " +
                "[Name] = @0 " +
                "WHERE [Id] = @1";
            command.Parameters.AddWithValue("@0", name);
            command.Parameters.AddWithValue("@1", id);
            var obj = command.ExecuteNonQuery();
        }

        public void ChangeAppStatus(int id, short status)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "UPDATE dbo.[Applications] SET " +
                "[Status] = @0 " +
                "WHERE [Id] = @1";
            command.Parameters.AddWithValue("@0", status);
            command.Parameters.AddWithValue("@1", id);
            var obj = command.ExecuteNonQuery();
        }

        public int AddUniverseDomainName(int universeId, string name, bool redirectToMain)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "INSERT INTO dbo.[UniverseDomainNames] " +
                "(UniverseId, [DomainName], RedirectToMain) VALUES " +
                "(@0, @1, @2);" +
                "SELECT SCOPE_IDENTITY() AS NewID;";
            command.Parameters.AddWithValue("@0", universeId);
            command.Parameters.AddWithValue("@1", name);
            command.Parameters.AddWithValue("@2", redirectToMain);
            var obj = command.ExecuteScalar();
            var id = System.Convert.ToInt32(obj);
            return id;
        }

        public void UpdateUniverseDomainName(int id, string name, bool redirectToMain)
        {
            var command = this.Connection.CreateCommand();
            command.CommandText = "UPDATE dbo.[UniverseDomainNames] SET " +
                "[DomainName] = @0, " +
                "[RedirectToMain] = @1 " +
                "WHERE [Id] = @2";
            command.Parameters.AddWithValue("@0", name);
            command.Parameters.AddWithValue("@1", redirectToMain);
            command.Parameters.AddWithValue("@2", id);
            var obj = command.ExecuteNonQuery();
        }

        #endregion
    }
}
