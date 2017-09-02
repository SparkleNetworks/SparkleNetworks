
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Entity.Networks.Sql;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using System;
    using System.Collections.Generic;
    using System.Data.EntityClient;
    using System.Data.Objects;
    using System.Data.Objects.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class CompaniesRepository : BaseNetworkRepository<Company, int>, ICompanyRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public CompaniesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Companies)
        {
        }

        public IList<CompanyExtended> GetWithStatsAndSkills(int networkId)
        {
            var companies = this.Context.Companies
                .ByNetwork(networkId)
                .Active()
                .OrderBy(c => c.Name)
                .Select(c => new CompanyExtended
                {
                    Company = c,
                    ////PeopleCount = c.Users.ActiveAccount().Count(),
                    InvitedCount = c.Inviteds.Where(i => i.User == null).Count(),
                    NetworkName = c.Network.Name,
                })
                .ToList();

            var cskills = this.Context.CompanySkills
                .Include("Skill")
                .Where(s => s.Company.NetworkId == networkId)
                .ToList();

            foreach (var item in companies)
            {
                item.Users = this.Context.Users.WithCompanyId(item.Company.ID).ActiveAccount().ToList();
                item.PeopleCount = item.Users.Count();

                item.Skills = cskills
                    .Where(s => s.CompanyId == item.Company.ID)
                    .Select(s => s.Skill)
                    .ToList();
            }

            return companies;
        }

        public IList<CompanyExtended> GetAllWithStatsAndSkills()
        {
            var companies = this.Context.Companies
                .Active()
                .OrderBy(c => c.Name)
                .Select(c => new CompanyExtended
                {
                    Company = c,
                    PeopleCount = c.Users.Count,
                    InvitedCount = c.Inviteds.Where(i => i.User == null).Count(),
                    NetworkName = c.Network.Name,
                })
                .ToList();

            var cskills = this.Context.CompanySkills
                .Include("Skill")
                .ToList();

            foreach (var item in companies)
            {
                item.Skills = cskills
                    .Where(s => s.CompanyId == item.Company.ID)
                    .Select(s => s.Skill)
                    .ToList();
            }

            return companies;
        }

        public IList<Sparkle.Entities.Networks.Neutral.CompanyPoco> GetWithStatsAndSkillsAndJobs(int networkId)
        {
            var companies = this.Context.Companies
                .ByNetwork(networkId)
                .Active()
                .Enabled()
                .OrderBy(c => c.Name)
                .ToList()
                .Select(c => new Sparkle.Entities.Networks.Neutral.CompanyPoco
                {
                    Alias = c.Alias,
                    ApprovedDateUtc = c.ApprovedDateUtc,
                    Baseline = c.Baseline,
                    CategoryId = c.CategoryId,
                    CreatedDateUtc = c.CreatedDateUtc,
                    EmailDomain = c.EmailDomain,
                    ID = c.ID,
                    IsApproved = c.IsApproved,
                    Name = c.Name,
                    NetworkId = c.NetworkId,
                    Users = c.Users
                        .Where(u => u.CompanyAccessLevel > 0 && u.NetworkAccessLevel > 0)
                        .Select(u => new Sparkle.Entities.Networks.Neutral.UserPoco
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            Gender = u.Gender,
                            JobId = u.JobId,
                            Job = u.JobId != null ? new Sparkle.Entities.Networks.Neutral.JobPoco
                            {
                                Id = u.JobId.Value,
                                Libelle = u.Job.Libelle,
                                Alias = u.Job.Alias,
                            } : null,
                            LastName = u.LastName,
                            Login = u.Login,
                            NetworkId = u.NetworkId,
                            Picture = u.Picture,
                            Score = u.Score,
                            UserId = u.UserId,
                        })
                        .ToList(),

                    CompanySkills = c.CompanySkills
                        .Select(cs => new Sparkle.Entities.Networks.Neutral.CompanySkillPoco
                        {
                            Id = cs.Id,
                            CompanyId = c.ID,
                            Date = cs.Date,
                            SkillId = cs.SkillId,
                            Skill = new Sparkle.Entities.Networks.Neutral.SkillPoco
                            {
                                Id = cs.SkillId,
                                TagName = cs.Skill.TagName,
                                Date = cs.Skill.Date,
                            }
                        })
                        .ToList(),

                    ////PeopleCount = c.Users.Count(),
                    ////InvitedCount = c.Inviteds.Where(i => i.User == null).Count(),
                    ////NetworkName = c.Network.Name,
                })
                .ToList();

            ////var cskills = this.Context.CompanySkills
            ////    .Include("Skill")
            ////    .ToList();

            ////foreach (var item in companies)
            ////{
            ////    item.CompanySkills = cskills
            ////        .Where(s => s.CompanyId == item.Company.ID)
            ////        .Select(s => s.Skill)
            ////        .ToList();
            ////}

            return companies;
        }

        public IList<CompanyExtended> GetWithStats(int networkId)
        {
            var companies = this.Context.Companies
                .ByNetwork(networkId)
                .Active()
                .OrderBy(c => c.Name)
                .Select(c => new CompanyExtended
                {
                    Company = c,
                    PeopleCount = c.Users.Where(u => u.CompanyAccessLevel > 0 && u.NetworkAccessLevel > 0).Count(),
                    InvitedCount = c.Inviteds.Where(i => i.User == null).Count(),
                    NetworkName = c.Network.Name,
                })
                .ToList();

            return companies;
        }

        public IList<CompanyExtended> GetInactiveWithStats(int networkId)
        {
            var companies = this.Context.Companies
                .ByNetwork(networkId)
                .Inactive()
                .OrderBy(c => c.Name)
                .Select(c => new CompanyExtended
                {
                    Company = c,
                    PeopleCount = c.Users.Where(u => u.CompanyAccessLevel > 0 && u.NetworkAccessLevel > 0).Count(),
                    InvitedCount = c.Inviteds.Where(i => i.User == null).Count(),
                    NetworkName = c.Network.Name,
                })
                .ToList();

            return companies;
        }

        public IList<CompanyExtended> GetWithNetworkName()
        {
            var companies = this.Context.Companies
                .Active()
                .Select(c => new CompanyExtended
                {
                    Company = c,
                    PeopleCount = c.Users.Count,
                    NetworkName = c.Network.Name,
                })
                .ToList();

            return companies;
        }

        protected override Company GetById(NetworksEntities model, int id)
        {
            return this.GetSet(model).SingleOrDefault(c => c.ID == id);
        }

        protected override int GetEntityId(Company item)
        {
            return item.ID;
        }

        public IList<CompanyExtended> GetWaitingApprobation(int networkId)
        {
            var companies = this.Context.Companies
                .ByNetwork(networkId)
                .Where(c => !c.IsApproved)
                .OrderBy(c => c.Name)
                .Select(c => new CompanyExtended
                {
                    Company = c,
                    PeopleCount = c.Users.Count,
                    InvitedCount = c.Inviteds.Where(i => i.User == null).Count(),
                    NetworkName = c.Network.Name,
                })
                .ToList();

            return companies;
        }

        public int CountWaitingApprobation(int networkId)
        {
            var companies = this.Context.Companies
                .ByNetwork(networkId)
                .Where(c => !c.IsApproved)
                .Count();

            return companies;
        }

        public Company GetByAlias(string alias, CompanyOptions options)
        {
            return this
                .CreateQuery(options)
                .SingleOrDefault(c => c.Alias == alias);
        }

        public IQueryable<Company> CreateQuery(CompanyOptions options)
        {
            ObjectQuery<Company> query = this.Context.Companies;

            if ((options & CompanyOptions.CompanyAdmins) == CompanyOptions.CompanyAdmins)
                query = query.Include("CompanyAdmins");
            if ((options & CompanyOptions.CompanyNews) == CompanyOptions.CompanyNews)
                query = query.Include("CompanyNews");
            if ((options & CompanyOptions.CompanySkills) == CompanyOptions.CompanySkills)
                query = query.Include("CompanySkills");
            if ((options & CompanyOptions.Events) == CompanyOptions.Events)
                query = query.Include("Events");
            if ((options & CompanyOptions.Network) == CompanyOptions.Network)
                query = query.Include("Network");
            if ((options & CompanyOptions.Users) == CompanyOptions.Users)
                query = query.Include("Users");

            return query;
        }

        public int? GetIdByAlias(string alias)
        {
            var obj = this.Set
                .Where(c => c.Alias == alias)
                .Select(u => new { Id = u.ID, })
                .SingleOrDefault();

            return obj != null ? obj.Id : default(int?);
        }

        public Company GetById(int id, CompanyOptions options)
        {
            return this.Set
                .Where(g => id == g.ID)
                .SingleOrDefault();
        }

        public IList<Company> GetById(int[] ids)
        {
            return this.Set
                .Where(g => ids.Contains(g.ID))
                .ToList();
        }

        public IList<Company> GetActiveById(int[] ids)
        {
            return this.Set
                .Active()
                .Where(g => ids.Contains(g.ID))
                .ToList();
        }

        public IList<Company> GetById(int[] ids, int networkId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(g => ids.Contains(g.ID))
                .ToList();
        }

        public IList<Company> GetActiveById(int[] ids, int networkId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Active()
                .Where(g => ids.Contains(g.ID))
                .ToList();
        }

        public IList<Company> GetAllForImportScripts(int networkId)
        {
            return this.Set
                .ByNetwork(networkId)
                .OrderBy(c => c.Name)
                .ToList();
        }

        public IList<Person> GetAdministrators(int companyId, int networkId)
        {
            return this.Context.UsersViews
                .ByNetwork(networkId)
                .WithCompanyId(companyId)
                .ActiveAccount()
                .Where(u => u.CompanyAccessLevel == (int)CompanyAccessLevel.Administrator)
                .LiteSelect()
                .ToList();
        }

        public IDictionary<int, Company> GetById(IList<int> ids, CompanyOptions options)
        {
            return this
                .CreateQuery(options)
                .Where(g => ids.Contains(g.ID))
                .OrderBy(c => c.Name)
                .ToDictionary(c => c.ID, c => c);
        }

        public IList<GetCompaniesAccessLevelReport_Result> GetCompaniesAccessLevelReport(int networkId)
        {
            return this.Context.GetCompaniesAccessLevelReport(networkId).ToList();
        }

        public GetCompaniesAccessLevelReport_Result GetCompaniesAccessLevelReport(int networkId, int companyId)
        {
            return this.Context.GetCompaniesAccessLevelReport(networkId)
                .Where(r => r.Id == companyId)
                .SingleOrDefault();
        }

        public IList<Company> GetAllActive(int networkId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(o => o.IsApproved && o.IsEnabled)
                .ToList();
        }

        public IList<Company> GetActiveByName(int networkId, string name)
        {
            return this.Set
                .ByNetwork(networkId)
                .Enabled()
                .Active()
                .Where(o => o.Name == name)
                .OrderBy(o => o.Alias)
                .ToList();
        }

        public IList<CompanySearchRow> Search(int[] networkId, string[] keywords, string[] geocodes, int[] tagIds, bool combinedTags, int offset, int count, CompanyOptions options)
        {
            int radius = 30;
            var numberStyleFloat = NumberStyles.Float;
            var invariantCulture = CultureInfo.InvariantCulture;
            var numberFormat = invariantCulture.NumberFormat;
            string[] newgeocodes = null;
            if (geocodes != null)
            {
                newgeocodes = new string[geocodes.Length];
                double lat;
                double lon;
                for (int i = 0; i < geocodes.Length; i++)
                {
                    var geoSplit = geocodes[i].Split(new char[] { ' ', });
                    if (geoSplit.Length == 2
                        && double.TryParse(geoSplit[0], numberStyleFloat, invariantCulture, out lat)
                        && double.TryParse(geoSplit[1], numberStyleFloat, invariantCulture, out lon))
                    {
                        newgeocodes[i] = SqlGeometryPoint(lat, lon);
                    }
                }
            }

            var sql = new StringBuilder();
            var cmd = ((EntityConnection)this.Context.Connection).StoreConnection.EnsureOpen().CreateCommand();

            if (tagIds != null && tagIds.Length > 0)
            {
                sql.AppendLine(";WITH ByTags AS (");
                if (combinedTags)
                {
                    string sep = "    ";
                    for (int i = 0; i < tagIds.Length; i++)
                    {
                        sql.Append(sep);
                        sql.AppendLine("select distinct CompanyId from dbo.CompanyTags where TagId = " + tagIds[i]);
                        sep = combinedTags ? "    intersect " : "    union ";
                    }
                }
                else
                {
                    sql.Append("select distinct CompanyId from dbo.CompanyTags where TagId IN ( ");
                    sql.Append(string.Join(", ", tagIds));
                    sql.AppendLine(")");
                }

                sql.AppendLine(")");
                sql.AppendLine("");
            }

            sql.AppendLine("select");
            sql.AppendLine("    C.Id, C.NetworkId, C.Name, C.Alias");

            if (newgeocodes != null)
            {
                sql.AppendLine("    , CP.PlaceId, CPP.Geography");
                string sep = "    ";
                for (int i = 0; i < newgeocodes.Length; i++)
                {
                    var geocode = newgeocodes[i];
                    sql.Append(sep);
                    sql.Append(", (CPP.[Geography].STDistance(");
                    sql.Append(geocode);
                    sql.AppendLine(") / 1000) Distance" + i + " ");
                }
            }
            else
            {
                sql.AppendLine("    , null as PlaceId, null as Geography");
            }

            sql.AppendLine();
            sql.AppendLine("from dbo.Companies C");
            sql.AppendLine();

            if (newgeocodes != null)
            {
                sql.AppendLine("inner join dbo.CompanyPlaces CP on CP.CompanyId = C.ID");
                sql.AppendLine("inner join dbo.Places CPP on CPP.Id = CP.PlaceId");
                sql.AppendLine();
            }

            if (tagIds != null && tagIds.Length > 0)
            {
                sql.AppendLine("inner join ByTags ON ByTags.CompanyId = C.ID");
                sql.AppendLine();
            }

            string where = "where ";
            var getWhereClause = new Func<string>(() =>
            {
                var val = where;
                where = " and ";
                return val;
            });

            if (networkId != null)
            {
                sql.Append(getWhereClause());
                sql.Append(" C.NetworkId IN (");
                sql.Append(string.Join(",", networkId));
                sql.AppendLine(")");
                sql.AppendLine();
            }

            if (newgeocodes != null)
            {
                sql.Append(getWhereClause());
                sql.AppendLine(" ( ");
                string sep = "       ";
                for (int i = 0; i < newgeocodes.Length; i++)
                {
                    var geocode = newgeocodes[i];
                    sql.Append(sep);
                    sql.Append("((CPP.[Geography].STDistance(");
                    sql.Append(geocode);
                    sql.AppendLine(") / 1000) < " + radius + ")");
                    sep = "    or ";
                }

                sql.AppendLine(")");
                sql.AppendLine();
            }

            if (keywords != null && keywords.Length > 0)
            {
                sql.Append(getWhereClause());
                sql.AppendLine(" ( ");
                string sep = "        ";
                for (int i = 0; i < keywords.Length; i++)
                {
                    var name = "@Keyword" + i;
                    var value = "%" + keywords[i] + "%";
                    cmd.AddParameter(name, value);
                    sql.Append(sep);
                    sql.Append("(C.Name LIKE ");
                    sql.Append(name);
                    sql.Append(" OR C.Alias LIKE ");
                    sql.Append(name);
                    sql.AppendLine(")");
                    sep = "    and ";
                }

                sql.AppendLine(")");
                sql.AppendLine();
            }

            sql.AppendLine();
            cmd.CommandText = sql.ToString();

            var values = new List<CompanySearchRow>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var row = new CompanySearchRow
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        NetworkId = reader.GetInt32(reader.GetOrdinal("NetworkId")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Alias = reader.GetString(reader.GetOrdinal("Alias")),
                    };
                    row.PlaceId = !reader.IsDBNull(reader.GetOrdinal("PlaceId")) ? reader.GetInt32(reader.GetOrdinal("PlaceId")) : default(int?);

                    if (geocodes != null)
                    {
                        var distances = new double[geocodes.Length];
                        for (int i = 0; i < geocodes.Length; i++)
                        {
                            distances[i] = reader.GetDouble(reader.GetOrdinal("Distance" + i));
                        }

                        row.Distance = distances.Min();

                    }

                    values.Add(row);
                }
            }

            return values;
        }

        private static string SqlGeometryPoint(double latitude, double longitude)
        {
            var numberFormat = CultureInfo.InvariantCulture.NumberFormat;
            var value = "geography::STGeomFromText('POINT(" + longitude.ToString(numberFormat) + " " + latitude.ToString(numberFormat) + ")', 4326)";
            return value;
        }
    }

    public class CompanyAdminRepository : BaseNetworkRepositoryInt<CompanyAdmin>, ICompanyAdminRepository
    {
        public CompanyAdminRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.CompanyAdmins)
        {
        }
    }
}