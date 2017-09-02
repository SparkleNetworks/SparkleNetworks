
namespace Sparkle.Commands.Main.Import
{
    using Sparkle.Common.DataAnnotations;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using SrkToolkit.Common.Validation;
    using SrkToolkit.DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;

    public class Column
    {
        private static Dictionary<string, ColumnParser> parsers;
        private Action<Cell> parser;

        static Column()
        {
            var separators = new char[] { ',', ';', };
            parsers = new Dictionary<string, ColumnParser>();
            AddParser(new ColumnParser("Company.Name", c =>
            {
                c.Row.Company.Name = c.Value.Trim();
                if (c.Row.Company.Name.Length > 100)
                    c.Row.Errors.Add("Company.Name is too long (" + c.Row.Company.Name.Length + "/100)");
            }));
            AddParser(new ColumnParser("Company.Website", c =>
            {
                if (string.IsNullOrWhiteSpace(c.Value))
                    return;

                var value = c.Value.Trim();
                if (value.StartsWith("www."))
                    value = "http://" + value;

                c.Row.Company.WebSite = c.Value.NullIfEmptyOrWhitespace(true);
                if (c.Row.Company.WebSite.Length > 100)
                    c.Row.Errors.Add("Company.WebSite is too long (" + c.Row.Company.WebSite.Length + "/100)");
            }));
            AddParser(new ColumnParser("User.Website", c =>
            {
                if (string.IsNullOrWhiteSpace(c.Value))
                    return;

                c.Row.Errors.Add("The field User.Website is not implemented");

                var value = c.Value.Trim();
                if (value.StartsWith("www."))
                    value = "http://" + value;

                c.Row.User.WebSite = c.Value.NullIfEmptyOrWhitespace(true);
                if (c.Row.User.WebSite.Length > 100)
                    c.Row.Errors.Add("User.WebSite is too long (" + c.Row.User.WebSite.Length + "/100)");
            }));
            AddParser(new ColumnParser("Company.Phone", c =>
            {
                if (!string.IsNullOrEmpty(c.Value))
                {
                    try
                    {
                        string converted;
                        if (PhoneNumberAttribute.ConvertNationalToInternational(c.Value, c.Row.Culture ?? CultureInfo.InvariantCulture, out converted))
                        {
                            c.Row.Company.Phone = converted;
                            if (c.Row.Company.Phone.Length > 20)
                                c.Row.Errors.Add("Company.Phone is too long (" + c.Row.Company.Phone.Length + "/20)");
                        }
                        else
                        {
                            c.Row.Errors.Add("Invalid phone number '" + c.Value + "'");
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        c.Row.Errors.Add("Invalid phone number '" + c.Value + "': " + ex.Message);
                    }
                }
            }));
            AddParser(new ColumnParser("Company.Email", c =>
            {
                if (string.IsNullOrWhiteSpace(c.Value))
                    return;

                c.Row.Company.Email = EmailAddress.TryCreate(c.Value.Trim());
                if (c.Row.Company.Email == null)
                {
                    c.Row.Warnings.Add("Invalid company email address '" + c.Value + "'");
                }
                else
                {
                    if (c.Row.Company.Email.Value.Length > 100)
                        c.Row.Errors.Add("Company.Email is too long (" + c.Row.Company.Email.Value.Length + "/10)");
                }
            }));
            AddParser(new ColumnParser("User.Email", c =>
            {
                if (string.IsNullOrWhiteSpace(c.Value))
                    return;

                c.Row.User.Email = EmailAddress.TryCreate(c.Value.Trim());
                if (c.Row.User.Email == null)
                {
                    c.Row.Warnings.Add("Invalid user email address '" + c.Value + "'");
                }
                else
                {
                    if (c.Row.User.Email.Value.Length > 100)
                        c.Row.Errors.Add("User.Email is too long (" + c.Row.User.Email.Value.Length + "/10)");
                }
            }));
            AddParser(new ColumnParser("User.Phone", c =>
            {
                if (!string.IsNullOrEmpty(c.Value))
                {
                    string converted;
                    if (PhoneNumberAttribute.ConvertNationalToInternational(c.Value.Trim(), c.Row.Culture ?? CultureInfo.InvariantCulture, out converted))
                    {
                        c.Row.User.Phone = converted;
                        if (c.Row.User.Phone.Length > 20)
                            c.Row.Errors.Add("User.Phone is too long (" + c.Row.User.Phone.Length + "/20)");
                    }
                    else
                    {
                        throw new ArgumentException("Invalid phone number '" + c.Value + "'");
                    }
                }
            }));
            AddParser(new ColumnParser("User.Firstname", c =>
            {
                c.Row.User.FirstName = c.Value.Trim();
                if (c.Row.User.FirstName.Length > 100)
                    c.Row.Errors.Add("User.FirstName is too long (" + c.Row.User.FirstName.Length + "/100)");
            }));
            AddParser(new ColumnParser("User.Lastname", c =>
            {
                c.Row.User.LastName = c.Value.Trim();
                if (c.Row.User.LastName.Length > 100)
                    c.Row.Errors.Add("User.LastName is too long (" + c.Row.User.LastName.Length + "/100)");
            }));
            AddParser(new ColumnParser("User.Firstandlastname", c =>
            {
                string first, last;
                if (ImportExportTools.SplitFirstAndLastName(c.Value, out first, out last))
                {
                    c.Row.User.FirstName = first;
                    c.Row.User.LastName = last;
                    if (c.Row.User.FirstName.Length > 100)
                        c.Row.Errors.Add("User.FirstName is too long (" + c.Row.User.FirstName.Length + "/100)");
                    if (c.Row.User.LastName.Length > 100)
                        c.Row.Errors.Add("User.LastName is too long (" + c.Row.User.LastName.Length + "/100)");
                }
                else
                {
                    throw new InvalidOperationException("Failed to parse '" + c.Value + "' in column '" + c.Column.Name + "' ");
                }
            }));
            AddParser(new ColumnParser("User.Lastandfirstname", c =>
            {
                string first, last;
                if (ImportExportTools.SplitLastAndFirstName(c.Value, out first, out last))
                {
                    c.Row.User.FirstName = first;
                    c.Row.User.LastName = last;
                    if (c.Row.User.FirstName.Length > 100)
                        c.Row.Errors.Add("User.FirstName is too long (" + c.Row.User.FirstName.Length + "/100)");
                    if (c.Row.User.LastName.Length > 100)
                        c.Row.Errors.Add("User.LastName is too long (" + c.Row.User.LastName.Length + "/100)");
                }
                else
                {
                    throw new InvalidOperationException("Failed to parse '" + c.Value + "' in column '" + c.Column.Name + "' ");
                }
            }));
            AddParser(new ColumnParser("User.Gender", c =>
            {
                var female = new string[] { "f", "fem", "female", "femme", "w", "woman", "women", };
                var male = new string[] { "h", "hom", "homme", "m", "man", "men", };
                var value = (c.Value ?? "").ToLowerInvariant().Trim();
                if (string.IsNullOrEmpty(value))
                {
                    throw new InvalidOperationException("Empty value in column '" + c.Column.Name + "' ");
                }
                else if (male.Contains(value))
                {
                    c.Row.User.Gender = NetworkUserGender.Male;
                }
                else if (female.Contains(value))
                {
                    c.Row.User.Gender = NetworkUserGender.Female;
                }
                else
                {
                    throw new InvalidOperationException("Failed to parse '" + c.Value + "' in column '" + c.Column.Name + "' ");
                }
            }));
            AddParser(new ColumnParser("User.Culture", c =>
            {
                if (string.IsNullOrEmpty(c.Value))
                    return;

                try
                {
                    var culture = new CultureInfo(c.Value.Trim());
                    c.Row.User.Culture = culture.Name;
                }
                catch (CultureNotFoundException)
                {
                    c.Row.Warnings.Add("Invalid User.Culture '" + c.Value + "'.");
                }
            }));
            AddParser(new ColumnParser("User.TimeZone", c =>
            {
                if (string.IsNullOrEmpty(c.Value))
                    return;

                try
                {
                    var culture = TimeZoneInfo.FindSystemTimeZoneById(c.Value.Trim());
                    c.Row.User.TimeZone = culture.Id;
                }
                catch (TimeZoneNotFoundException)
                {
                    c.Row.Warnings.Add("Invalid User.TimeZone '" + c.Value + "'.");
                }
            }));
            AddParser(new ColumnParser("Company.Tags.Skills", c =>
            {
                if (c.Value == null)
                    return;
                if (c.Row.CompanyTags == null)
                    c.Row.CompanyTags = new Dictionary<string, List<string>>();
                if (!c.Row.CompanyTags.ContainsKey("Skill"))
                    c.Row.CompanyTags.Add("Skill", new List<string>());
                var items = c.Value.Split(separators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => x.Length > 1).Distinct();
                foreach (var item in items)
                {
                    c.Row.CompanyTags["Skill"].Add(item);
                    if (item.Length > 150)
                        c.Row.Errors.Add("Company.Tags.Skills is too long (" + item.Length + "/150)");
                }
            }));
            AddParser(new ColumnParser("Company.Place.Street", c =>
            {
                if (c.Value == null)
                    return;
                if (c.Row.CompanyPlace == null)
                    c.Row.CompanyPlace = new Sparkle.Services.Networks.Places.EditPlaceRequest();
                c.Row.CompanyPlace.Address = c.Value.Trim();
                if (c.Row.CompanyPlace.Address.Length > 100)
                    c.Row.Errors.Add("Company.Place.Street is too long (" + c.Row.CompanyPlace.Address.Length + "/100)");
            }));
            AddParser(new ColumnParser("Company.Place.City", c =>
            {
                if (c.Value == null)
                    return;
                if (c.Row.CompanyPlace == null)
                    c.Row.CompanyPlace = new Sparkle.Services.Networks.Places.EditPlaceRequest();
                c.Row.CompanyPlace.City = c.Value.Trim();
                if (c.Row.CompanyPlace.City.Length > 100)
                    c.Row.Errors.Add("Company.Place.City is too long (" + c.Row.CompanyPlace.City.Length + "/100)");
            }));
            AddParser(new ColumnParser("Company.Place.Postcode", c =>
            {
                if (c.Value == null)
                    return;
                if (c.Row.CompanyPlace == null)
                    c.Row.CompanyPlace = new Sparkle.Services.Networks.Places.EditPlaceRequest();
                c.Row.CompanyPlace.ZipCode = c.Value.Trim();
                if (c.Row.CompanyPlace.ZipCode.Length > 100)
                    c.Row.Errors.Add("Company.Place.Postcode is too long (" + c.Row.CompanyPlace.ZipCode.Length + "/100)");
            }));
            AddParser(new ColumnParser("Company.Place.Longitude", c =>
            {
                if (c.Value == null)
                    return;
                if (c.Row.CompanyPlace == null)
                    c.Row.CompanyPlace = new Sparkle.Services.Networks.Places.EditPlaceRequest();

                decimal val;
                if (decimal.TryParse(c.Value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out val))
                {
                    c.Row.CompanyPlace.Longitude = val;
                    if (val < -180 || val > 180)
                    {
                        c.Row.Errors.Add("Company.Place.Longitude is out of range (" + val + ").");
                    }
                }
                else
                {
                    c.Row.Errors.Add("Company.Place.Longitude is not a number.");
                }
            }));
            AddParser(new ColumnParser("Company.Place.Latitude", c =>
            {
                if (c.Value == null)
                    return;
                if (c.Row.CompanyPlace == null)
                    c.Row.CompanyPlace = new Sparkle.Services.Networks.Places.EditPlaceRequest();

                decimal val;
                if (decimal.TryParse(c.Value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out val))
                {
                    c.Row.CompanyPlace.Latitude = val;
                    if (val < -90 || val > 90)
                    {
                        c.Row.Errors.Add("Company.Place.Latitude is out of range (" + val + ").");
                    }
                }
                else
                {
                    c.Row.Errors.Add("Company.Place.Latitude is not a number.");
                }
            }));
            AddParser(new ColumnParser("Company.Tags", c =>
            {
                if (c.Value == null)
                    return;
                if (c.Column.Data == null || c.Column.Data.Length < 1)
                    return;

                var alias = c.Column.Data[0];

                if (c.Row.CompanyTags == null)
                    c.Row.CompanyTags = new Dictionary<string, List<string>>();
                if (!c.Row.CompanyTags.ContainsKey(alias))
                    c.Row.CompanyTags.Add(alias, new List<string>());
                var values = c.Row.CompanyTags[alias];
                var items = c.Value.Split(separators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => x.Length > 1).Distinct();
                foreach (var item in items)
                {
                    if (!values.Contains(item))
                    {
                        values.Add(item);
                        if (item.Length > 150)
                        {
                            c.Row.Errors.Add("Tag '" + item + "' in " + c.Column.Name + " is too long (" + item.Length + "/150)");
                        }
                    }
                }
            }));
        }

        public Column(string col)
        {
            this.Name = col;

            this.CreateParser();
        }

        public int Index { get; set; }

        public string Name { get; set; }

        public string DefaultValue { get; set; }

        public string[] Data { get; set; }

        public static IList<ColumnParser> Parsers
        {
            get { return parsers.Select(p => new ColumnParser { Name = p.Key, }).ToArray(); }
        }

        public void Parse(Cell cell)
        {
            this.parser(cell);
        }

        private static void AddParser(ColumnParser parser)
        {
            parsers.Add(parser.Name.ToLowerInvariant(), parser);
        }

        private void CreateParser()
        {
            var nameParts = this.Name.Split(':');
            var lowerName = nameParts[0].ToLowerInvariant();
            if (parsers.ContainsKey(lowerName))
            {
                this.parser = parsers[lowerName].Action;
                this.Data = new string[nameParts.Length - 1];
                Array.Copy(nameParts, 1, this.Data, 0, nameParts.Length - 1);
            }
            else
            {
                throw new InvalidOperationException("Unrecognized column '" + this.Name + "' ");
            }
        }
    }

    public class ColumnParser
    {
        public ColumnParser()
        {
        }

        public ColumnParser(string name, Action<Cell> action)
        {
            this.Name = name;
            this.Action = action;
        }

        public string Name { get; set; }

        public Action<Cell> Action { get; set; }
    }

    public class Row
    {
        private RowCompany company;
        private RowUser user;
        private int i;
        private readonly Dictionary<string, object> data = new Dictionary<string, object>();

        public Row(int i)
        {
            this.i = i;
            this.Errors = new List<string>();
            this.Warnings = new List<string>();
        }

        public int Index
        {
            get { return this.i; }
        }

        public RowCompany Company
        {
            get { return this.company ?? (this.company = new RowCompany()); }
        }

        public RowUser User
        {
            get { return this.user ?? (this.user = new RowUser()); }
        }

        public IList<string> Errors { get; set; }
        public IList<string> Warnings { get; set; }

        public void SetAttachment<T>(T data)
        {
            this.data[typeof(T).FullName] = data;
        }

        public T GetAttachment<T>()
        {
            var key = typeof(T).FullName;
            return this.data.ContainsKey(key) ? (T)this.data[key] : default(T);
        }

        public CultureInfo Culture { get; set; }

        public Dictionary<string, List<string>> CompanyTags { get; set; }

        public Services.Networks.Places.EditPlaceRequest CompanyPlace { get; set; }

        public override string ToString()
        {
            return this.i + " "
                + (this.Company != null ? ("Company:" + this.Company) : null)
                + (this.User != null ? ("User:" + this.User) : null)
                + (this.Errors != null && this.Errors.Count > 0 ? ("Errors:" + this.Errors.Count) : null)
                + (this.Warnings != null && this.Warnings.Count > 0 ? ("Warnings:" + this.Warnings.Count) : null)
                ;
        }
    }

    public class Cell : IDisposable
    {
        public Row Row { get; set; }
        public Column Column { get; set; }
        public string Value { get; set; }

        private bool isDisposed;

        #region IDisposable members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.Row = null;
                    this.Column = null;
                }

                this.isDisposed = true;
            }
        }

        #endregion
    }

    public class RowCompany
    {
        private string name;

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string WebSite { get; set; }

        public string Phone { get; set; }

        public EmailAddress Email { get; set; }

        public string EmailDomain { get; set; }
    }

    public class RowUser
    {
        private string lastName;
        private string firstName;

        public string FirstName
        {
            get { return this.firstName; }
            set { this.firstName = value; }
        }

        public string LastName
        {
            get { return this.lastName; }
            set { this.lastName = value; }
        }

        public string Phone { get; set; }

        public EmailAddress Email { get; set; }

        public NetworkUserGender? Gender { get; set; }

        public string Culture { get; set; }

        public string TimeZone { get; set; }

        public string WebSite { get; set; }
    }

    public class Table
    {
        public List<Column> Columns { get; set; }

        public List<Row> Rows { get; set; }

        public List<Column> Defaults { get; set; }
    }

    public class InsertChange<TImport, TData>
    {
        public TImport ImportObject { get; set; }
        public TData DataObject { get; set; }
    }

    public class UpdateChange<TData, TImport>
    {
        private readonly List<FieldUpdate> fieldUpdates = new List<FieldUpdate>();

        public UpdateChange(TData dataObject)
        {
            this.DataObject = dataObject;
            this.ImportObjects = new List<TImport>();
        }

        public TData DataObject { get; set; }
        public IList<TImport> ImportObjects { get; set; }

        public IEnumerable<FieldUpdate> Fields
        {
            get { return this.fieldUpdates.AsReadOnly(); }
        }

        public bool HasFieldUpdates
        {
            get { return this.fieldUpdates != null && this.fieldUpdates.Count > 0; }
        }

        public void UpdateField(string field, string oldValue, string newValue, Action<TData> update)
        {
            this.fieldUpdates.Add(new FieldUpdate
            {
                Field = field,
                NewValueString = newValue != null ? newValue.ToString() : "null",
                OldValueString = oldValue != null ? oldValue.ToString() : "null",
            });
            update(this.DataObject);
        }

        public class FieldUpdate
        {
            public string OldValueString { get; set; }
            public string NewValueString { get; set; }
            public string Field { get; set; }
        }
    }

    public class ImportEntity
    {
        public bool ExistsInDatabase { get; set; }
        public bool HasPendingDatabaseChanges { get; set; }
        public int DbId { get; set; }
        public int LineIndex { get; set; }

        public List<Services.Networks.Models.IProfileFieldValueModel> Fields { get; set; }
    }

    public class ImportEntityUser : ImportEntity
    {
        private UpdateChange<User, Row> update;

        public ImportEntityUser(User item)
        {
            this.DbId = item.Id;
            if (item.Id > 0)
            {
                this.EntityFromDatabase = item;
                this.ExistsInDatabase = true;
            }
            else
            {
                this.UpdatedEntity = item;
                this.HasPendingDatabaseChanges = true;
            }
        }

        public User EntityFromDatabase { get; set; }
        public User UpdatedEntity { get; set; }

        public UpdateChange<User, Row> Update
        {
            get { return this.update ?? (this.update = new UpdateChange<User, Row>(this.EntityFromDatabase ?? this.UpdatedEntity)); }
        }

        public EmailAddress Email
        {
            get { return this.EntityFromDatabase != null ? new EmailAddress(this.EntityFromDatabase.Email) : this.UpdatedEntity != null ? new EmailAddress(this.UpdatedEntity.Email) : null; }
        }

        public ImportEntityCompany Company { get; set; }

        public override string ToString()
        {
            return this.DbId + " " + this.Email + " " + (HasPendingDatabaseChanges ? "pending save" : "");
        }
    }

    public class ImportEntityCompany : ImportEntity
    {
        private string lowerName;
        private UpdateChange<Company, RowCompany> update;

        public ImportEntityCompany(CompanyPoco item)
        {
            this.DbId = item.ID;
            if (item.ID > 0)
            {
                this.EntityFromDatabase = item;
                this.ExistsInDatabase = item.ID > 0;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public ImportEntityCompany(Company item)
        {
            this.DbId = item.ID;
            if (item.ID == 0)
            {
                this.UpdatedEntity = item;
                this.HasPendingDatabaseChanges = true;
            }
            else
            {
                this.UpdatedEntity = item;
                this.ExistsInDatabase = item.ID > 0;
            }
        }

        public CompanyPoco EntityFromDatabase { get; set; }
        public Company UpdatedEntity { get; set; }
        public RowCompany RowCompany { get; set; }

        public UpdateChange<Company, RowCompany> Update
        {
            get { return this.update ?? (this.update = new UpdateChange<Company, RowCompany>(this.UpdatedEntity)); }
        }

        public string Name
        {
            get { return this.EntityFromDatabase != null ? this.EntityFromDatabase.Name : this.UpdatedEntity != null ? this.UpdatedEntity.Name : null; }
        }

        public string LowerName
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.lowerName ?? (this.lowerName = this.Name.ToLowerInvariant()); }
        }

        public string EmailDomain
        {
            get { return this.EntityFromDatabase != null ? this.EntityFromDatabase.EmailDomain : this.UpdatedEntity != null ? this.UpdatedEntity.EmailDomain : null; }
        }

        public override string ToString()
        {
            return this.DbId + " " + this.Name + " " + (HasPendingDatabaseChanges ? "pending save" : "");
        }

        public Dictionary<string, List<string>> Tags { get; set; }

        public Services.Networks.Places.EditPlaceRequest Place { get; set; }

        public List<CompanyProfileField> EmailFields { get; set; }
    }

    public static class CsvImportExtensions
    {
        public static UpdateChange<TData, TImport> GetOrCreate<TKey, TData, TImport>(this IDictionary<TKey, UpdateChange<TData, TImport>> collection, TKey id, TData item)
        {
            var item1 = collection.ContainsKey(id) ? collection[id] : null;
            if (item1 == null)
            {
                item1 = new UpdateChange<TData, TImport>(item);
                collection.Add(id, item1);
            }

            return item1;
        }
    }
}
