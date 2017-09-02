
namespace Sparkle.Commands.Main
{
    using Newtonsoft.Json;
    using Sparkle.Commands.Main.Import;
    using Sparkle.Common.CommandLine;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Networks;
    using SrkToolkit.Common.Validation;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Web.Security;
    using CompanyProfileFieldModel = Sparkle.Services.Networks.Companies.CompanyProfileFieldModel;
    using EditPlaceRequest = Sparkle.Services.Networks.Places.EditPlaceRequest;
    using IProfileFieldValueModel = Sparkle.Services.Networks.Models.IProfileFieldValueModel;
    using Tag2Model = Sparkle.Services.Networks.Tags.Tag2Model;
    using TagCategoryModel = Sparkle.Services.Networks.Tags.TagCategoryModel;
    using UserProfileFieldModel = Sparkle.Services.Networks.Users.UserProfileFieldModel;

    public class CsvUserImport : BaseSparkleCommand, ISparkleCommandsInitializer
    {
        private string remoteClient;
        private JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Formatting = Formatting.None,
        };

        /// <summary>
        /// Process entry point.
        /// </summary>
        /// <param name="args"></param>
        public override void RunUniverse(SparkleCommandArgs args)
        {
            this.remoteClient = Environment.MachineName + "/" + System.Diagnostics.Process.GetCurrentProcess().Id;
            var services = args.App.GetNewServiceFactoryWithoutCache();
            services.HostingEnvironment.Identity = ServiceIdentity.Unsafe.Root;
            services.HostingEnvironment.LogBasePath = "CsvUserImport";
            services.HostingEnvironment.RemoteClient = this.remoteClient;

            var context = new Context
            {
                Args = args,
                SessionId = DateTime.UtcNow.ToString("'CSV'yyMMddHHmm"),
            };
            context.Services = services;

            if (!this.Run(args, context))
            {
                services.Logger.Error("CsvUserImport failed", ErrorLevel.Business);
            }
        }

        private bool Run(SparkleCommandArgs args, Context context)
        {
            if (args.Application == null)
            {
                this.Error.WriteLine("No universe selected");
                return false;
            }

            this.Out.WriteLine();

            var services = context.Services;

            // network
            string networkName = args.ApplicationConfiguration.Tree.NetworkName ?? args.Application.UniverseName;
            var network = services.Networks.GetByName(networkName);
            if (network == null)
            {
                this.Out.WriteLine("Network \"" + networkName + "\" does not exist; not running.");
                args.SysLogger.Info("CsvUserImport", remoteClient, Environment.UserName, ErrorLevel.Input, "Network \"" + networkName + "\" does not exist; not running.");
                return false;
            }

            // simulation: not supported
            if (args.Simulation)
            {
                this.Out.WriteLine("Simulation is not supported; not running.");
                args.SysLogger.Info("CsvUserImport", remoteClient, Environment.UserName, ErrorLevel.Success, "Simulation is not supported; not running.");
                return false;
            }

            services.NetworkId = network.Id;

            if (!File.Exists(args.InFile))
            {
                this.EndWithError(context, ErrorLevel.Input, "File '" + args.InFile + "' does not exist.");
                return false;
            }

            if (!this.ParseArgs(context))
                return false;

            if (!this.PrepareParser(context))
                return false;

            if (!this.ParseCsv(context))
                return false;

            if (!this.BuildDataset(context))
                return false;

            if (!this.CreateChangeset(context))
                return false;

            if (!this.Execute(context))
                return false;

            return true;
        }

        /// <summary>
        /// Interpret the command line arguments.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true on success, false on failure</returns>
        protected bool ParseArgs(Context context)
        {
            const string headerParam = "header", fileEncodingParam = "encoding", csvStartAtLineParam = "firstcsvline",
                csvSeparatorParam = "csvseparator", allowCreateCompanyParam = "allowcreateCompany",
                noConfirmChangesetParam = "NoConfirmChangeset", cultureParam = "culture", userModeParam = "usermode",
                approverParam = "approver", inviterParam = "inviter", defaultsParam = "default", emailDelayParam = "emaildelay",
                dbCompanyCorrelationParam = "DbCompanyCorrelation", fileCompanyCorrelationParam = "FileCompanyCorrelation";

            foreach (var arg in context.Args.Arguments)
            {
                if (arg.ToLowerInvariant() == "CsvUserImport".ToLowerInvariant())
                {
                }
                else if (arg.StartsWith("--"))
                {
                    var eqIndex = arg.IndexOf('=');
                    var carg = arg.Substring(2, eqIndex > 0 ? eqIndex - 2 : arg.Length - 2);
                    var carglo = carg.ToLowerInvariant();
                    var cargValue = eqIndex > 0 ? arg.Substring(eqIndex + 1) : null;
                    var cargValueLo = cargValue != null ? cargValue.ToLowerInvariant() : null;

                    if (carglo == headerParam.ToLowerInvariant())
                    {
                        if (context.Header != null)
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "The CSV header is already set.");
                        }
                        else
                        {
                            context.Header = cargValue.Split(new string[] { ",", }, StringSplitOptions.None);
                        }
                    }
                    else  if (carglo == defaultsParam.ToLowerInvariant())
                    {
                        var parts = cargValue.Split(new char[] { '=', }, 2);
                        context.Defaults[parts[0]] = parts[1];
                    }
                    else if (carglo == fileEncodingParam.ToLowerInvariant())
                    {
                        Encoding encoding;
                        if (cargValueLo == "utf8" || cargValueLo == "utf-8")
                        {
                            context.FileEncoding = Encoding.UTF8;
                        }
                        else if (cargValueLo == "ascii")
                        {
                            context.FileEncoding = Encoding.ASCII;
                        }
                        else if ((encoding = Encoding.GetEncoding(cargValue)) != null)
                        {
                            context.FileEncoding = encoding;
                        }
                        else
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "Invalid encoding '" + cargValue + "'");
                        }
                    }
                    else if (carglo == csvStartAtLineParam.ToLowerInvariant())
                    {
                        int line;
                        if (int.TryParse(cargValue, out line))
                        {
                            context.CsvFirstLine = line;
                        }
                        else
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "Invalid index of first line '" + cargValue + "'");
                        }
                    }
                    else if (carglo == csvSeparatorParam.ToLowerInvariant())
                    {
                        if (cargValue.Length == 1)
                        {
                            context.CsvSeparator = cargValue[0];
                        }
                        else
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "A CSV separator must be 1 character long");
                        }
                    }
                    else if (carglo == allowCreateCompanyParam.ToLowerInvariant())
                    {
                        context.AllowCreateCompany = true;
                    }
                    else if (carglo == noConfirmChangesetParam.ToLowerInvariant())
                    {
                        context.NoConfirmChangeset = true;
                    }
                    else if (carglo == cultureParam.ToLowerInvariant())
                    {
                        try
                        {
                            context.DataCulture = new CultureInfo(cargValue);
                        }
                        catch (Exception)
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "Culture '" + arg + "' does not exist");
                        }
                    }
                    else if (carglo == userModeParam.ToLowerInvariant())
                    {
                        UserMode userMode;
                        if (Enum.TryParse<UserMode>(cargValue, true, out userMode))
                        {
                            context.UserMode = userMode;
                        }
                        else 
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "Invalid UserMode '" + cargValue + "'");
                        }
                    }
                    else if (carglo == approverParam.ToLowerInvariant())
                    {
                        var user = context.Services.People.GetActiveByLogin(cargValue, Data.Options.PersonOptions.Company);
                        if (user != null)
                        {
                            if (user.IsActive && user.NetworkAccessLevel.Value > NetworkAccessLevel.User)
                            {
                                context.Approver = user;
                            }
                            else
                            {
                                return this.EndWithError(context, ErrorLevel.Input, "Invalid approver '" + cargValue + "' (access level too low)");
                            }
                        }
                        else
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "Invalid approver '" + cargValue + "' (invalid username)");
                        }
                    }
                    else if (carglo == inviterParam.ToLowerInvariant())
                    {
                        var user = context.Services.People.GetActiveByLogin(cargValue, Data.Options.PersonOptions.Company);
                        if (user != null)
                        {
                            if (user.IsActive)
                            {
                                context.Inviter = user;
                            }
                            else
                            {
                                return this.EndWithError(context, ErrorLevel.Input, "Invalid inviter '" + cargValue + "' (access level too low)");
                            }
                        }
                        else
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "Invalid inviter '" + cargValue + "' (invalid username)");
                        }
                    }
                    else if (carglo == emailDelayParam.ToLowerInvariant())
                    {
                        TimeSpan value;
                        if (TimeSpan.TryParse(cargValue, out value) && value.TotalMilliseconds > 0)
                        {
                            context.EmailDelay = value;
                        }
                        else
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "Invalid -EmailDelay '" + cargValue + "' (expected format: 00:02:00.000)");
                        }
                    }
                    else if (carglo == dbCompanyCorrelationParam.ToLowerInvariant())
                    {
                        EntityCorrelationMode mode;
                        if (Enum.TryParse < EntityCorrelationMode>(cargValue, out mode))
                        {
                            context.DbCompanyCorrelationMode = mode;
                        }
                        else
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "Invalid -" + dbCompanyCorrelationParam + " '" + cargValue + "' (expected format: " + string.Join(", ", Enum.GetNames(typeof(EntityCorrelationMode))) + ")");
                        }
                    }
                    else if (carglo == fileCompanyCorrelationParam.ToLowerInvariant())
                    {
                        EntityCorrelationMode mode;
                        if (Enum.TryParse < EntityCorrelationMode>(cargValue, out mode))
                        {
                            context.FileCompanyCorrelationMode = mode;
                        }
                        else
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "Invalid -" + fileCompanyCorrelationParam + " '" + cargValue + "' (expected format: " + string.Join(", ", Enum.GetNames(typeof(EntityCorrelationMode))) + ")");
                        }
                    }
                    ////else if (carglo == skipCompanyChangesetValidationParam.ToLowerInvariant())
                    ////{
                    ////    context.SkipCompanyChangesetValidation = true;
                    ////}
                    ////else if (carglo == skipUserChangesetValidationParam.ToLowerInvariant())
                    ////{
                    ////    context.SkipUserChangesetValidation = true;
                    ////}
                    else
                    {
                        return this.EndWithError(context, ErrorLevel.Input, "Invalid parameter '" + arg + "'");
                    }
                }
                else
                {
                    return this.EndWithError(context, ErrorLevel.Input, "File not found '" + arg + "'");
                }
            }

            if (context.UserMode != UserMode.Ignore && context.Approver == null)
            {
                return this.EndWithError(context, ErrorLevel.Input, "When using UserMode<>Ignore, an approver is required (--approver=username).");
            }

            return true;
        }

        /// <summary>
        /// Create a parser that will interpret the columns using the specification from command line.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true on success, false on failure</returns>
        protected bool PrepareParser(Context context)
        {
            this.Out.WriteLine("Preparing CSV parser...");
            if (context.Header == null)
                return this.EndWithError(context, ErrorLevel.Input, "Missing CSV header --header=");

            var columns = new List<Column>(context.Header.Length);
            int i = 0;
            var extraSeps = new char[] { '+', };
            foreach (var col in context.Header)
            {
                var names = col.Split(extraSeps, StringSplitOptions.RemoveEmptyEntries);
                foreach (var name in names)
                {
                    var column = new Column(name)
                    {
                        Index = i,
                    };
                    columns.Add(column); 
                }

                i++;
            }

            var defaults = new List<Column>(context.Defaults.Count);
            foreach (var col in context.Defaults)
            {
                var column = new Column(col.Key)
                {
                    Index = -1,
                    DefaultValue = col.Value,
                };
                defaults.Add(column); 
            }

            context.Table = new Table()
            {
                Columns = columns,
                Defaults = defaults,
            };

            this.Out.WriteLine("CSV parser is ready");
            this.Out.WriteLine();
            for (int j = 0; j < context.Table.Columns.Count; j++)
            {
                var col = context.Table.Columns[j];
                this.Out.WriteLine("Col " + col.Index.ToString().PadLeft(3) + ": " + col.Name);
            }
            this.Out.WriteLine();

            return true;
        }

        /// <summary>
        /// Read the CSV file into memory.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true on success, false on failure</returns>
        protected bool ParseCsv(Context context)
        {
            var filePath = context.Args.InFile;

            var watch = Stopwatch.StartNew();
            this.Out.WriteLine("CsvUserImport reading '" + filePath + "'...");

            var fileStream = new FileStream(context.Args.InFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                var reader = new StreamReader(fileStream, context.FileEncoding);
                return this.ParseCsv(context, reader);
            }
            catch (Exception ex)
            {
                return this.EndWithError(context, ErrorLevel.Input, "Cannot open file for read '" + filePath + "': " + ex.Message);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Dispose();
            }
        }

        /// <summary>
        /// Parse the CSV data into rows and cells.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reader"></param>
        /// <returns>true on success, false on failure</returns>
        protected bool ParseCsv(Context context, TextReader reader)
        {
            var data = new List<List<string>>();

            var errors = new List<string>();
            var err = new Action<int, string>((int lineIndex1, string error) =>
            {
                errors.Add("CSV:" + lineIndex1 + " Error: " + error);
            });

            var warnings = new List<string>();
            var warn = new Action<int, string>((int lineIndex1, string error) =>
            {
                warnings.Add("CSV:" + lineIndex1 + " Warning: " + error);
            });

            string line;
            int lineIndex = -1;
            bool inCell = false, inQuotes = false;
            StringBuilder cellBuilder = null;
            int startOfCell = 0;
            bool lineContinues = false;
            List<string> dataList = new List<string>();
            while ((line = reader.ReadLine()) != null)
            {
                lineIndex++;

                if (!lineContinues && string.IsNullOrEmpty(line))
                {
                    warn(lineIndex, "Line is empty.");
                    continue;
                }

                if (!lineContinues)
                {
                    dataList = new List<string>();
                    data.Add(dataList);
                    inCell = false;
                    inQuotes = false;
                }

                startOfCell = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    var c = line[i];

                    if (!inCell)
                    {
                        if (c == context.CsvSeparator)
                        {
                            inCell = false;
                            startOfCell = i;
                        }
                        else
                        {
                            startOfCell = i;
                            if (c == '"')
                            {
                                startOfCell = i + 1;
                                inQuotes = true;
                                cellBuilder = new StringBuilder(line.Length - i);
                            }

                            inCell = true;
                        }
                    }
                    else if (inQuotes)
                    {
                        if (c == '"' && line.Length > (i + 1) && line[i + 1] == '"')
                        {
                            // escaped quote => ignore
                            i++;
                            cellBuilder.Append(c);
                        }
                        else if (c == '"' && line.Length > (i + 1) && line[i + 1] == context.CsvSeparator)
                        {
                            // end of quoted cell
                            i++;
                            inQuotes = false;
                            inCell = false;
                        }
                        else if (c == '"' && line.Length == (i + 1))
                        {
                            // end of quoted cell and end of line
                            i++;
                            inQuotes = false;
                            inCell = false;
                        }
                        else
                        {
                            cellBuilder.Append(c);
                        }
                    }
                        
                    if (c == context.CsvSeparator)
                    {
                        if (inQuotes)
                        {
                            // do nothing
                        }
                        else if (inCell)
                        {
                            inCell = false;
                        }
                        else
                        {
                            startOfCell = i;
                        }
                    }

                    if (!inCell)
                    {
                        // end of cell => capture
                        if (cellBuilder != null)
                        {
                            // when the line is quoted, we use a StringBuilder to skip some chars
                            dataList.Add(cellBuilder.ToString());
                            cellBuilder = null;
                        }
                        else
                        {
                            dataList.Add(line.Substring(startOfCell, i - startOfCell));
                        }
                    }

                    if ((i + 1) == line.Length)
                    {
                        // end of line => capture
                        if (inQuotes)
                        {
                            lineContinues = true;
                        }
                        else
                        {
                            if (inCell)
                            {
                                dataList.Add(line.Substring(startOfCell, i - startOfCell + 1));
                            }
                            else
                            {
                                // when last entry is empty
                                dataList.Add(string.Empty);
                            }

                            lineContinues = false;
                            cellBuilder = null;
                        }
                    }
                }
            }

            int colCount = -1;
            
            for (int i = 0; i < data.Count; i++)
            {
                if (i == 0)
                {
                    colCount = data[i].Count;
                }
                else if (data[i].Count != colCount)
                {
                    err(i, "Invalid cell count '" + data[i].Count + "'; expected '" + colCount + "'");
                }
            }

            if (errors.Count > 0)
            {
                var message = "Parsing of CSV data failed with " + errors.Count + " error" + Environment.NewLine + string.Join(Environment.NewLine, errors);
                this.Out.WriteLine(message);

                if (!this.AskConfirmation("Continue anyway? (Y/N)"))
                    return false;
            }

            if (warnings.Count > 0)
            {
                this.Out.WriteLine("Parsing of CSV data generated " + errors.Count + " warnings" + Environment.NewLine + string.Join(Environment.NewLine, warnings));
            }

            this.Out.WriteLine("CsvUserImport file read.");
            this.Out.WriteLine("  Read " + data.Count + " rows");

            context.CsvData = data;
            return true;
            
        }

        /// <summary>
        /// Execute parsers on each CSV cell to create objects.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true on success, false on failure</returns>
        protected bool BuildDataset(Context context)
        {
            this.Out.WriteLine("Transforming data...");

            var table = context.Table;
            table.Rows = new List<Row>(context.CsvData.Count);

            var errors = new List<string>();
            var err = new Action<int, string>((int line, string error) =>
            {
                errors.Add("CSV." + line + ": " + error);
            });

            for (int i = 0; i < context.CsvData.Count; i++)
            {
                var line = context.CsvData[i];

                if (i < context.CsvFirstLine)
                {
                    continue;
                }

                try
                {
                    var row = new Row(i)
                    {
                        Culture = context.DataCulture,
                    };
                    table.Rows.Add(row);

                    var maxIndex = table.Columns.Max(x => x.Index);
                    if ((maxIndex + 1) != line.Count)
                    {
                        err(i, "row contains " + line.Count + " rows but expected " + (maxIndex + 1));
                        continue;
                    }

                    var cells = new List<Cell>(line.Count + table.Columns.Count);

                    for (int c = 0; c < table.Columns.Count; c++)
                    {
                        var col = table.Columns[c];
                        var cell = line.Count >= (col.Index + 1) ? line[col.Index] : null;
                        if (cell == null)
                        {
                            err(i, "row does not contain column '" + col.Name + "' because index " + col.Index + " does not exist");
                        }

                        cells.Add(new Cell
                        {
                            Column = col,
                            Row = row,
                            Value = cell,
                        });
                    }

                    if (context.Defaults != null && context.Defaults.Count > 0)
                    {
                        foreach (var def in table.Defaults)
                        {
                            var cell = new Cell
                            {
                                Column = def,
                                Row = row,
                                Value = def.DefaultValue,
                            };
                            cells.Insert(0, cell);
                        }
                    }

                    for (int c = 0; c < cells.Count; c++)
                    {
                        var cell = cells[c];
                        cell.Column.Parse(cell);
                        cell.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    err(i, "Internal error: " + ex.Message);
                }
            }

            if (errors.Count > 0)
            {
                var message = "Build of dataset failed with " + errors.Count + " error" + Environment.NewLine + string.Join(Environment.NewLine, errors);
                this.Out.WriteLine(message);

                if (!this.AskConfirmation("Continue anyway? (Y/N)"))
                    return false;
            }

            this.Out.WriteLine("Dataset ready.");

            return true;
        }

        /// <summary>
        /// Create a list of changes that may be pushed to the database.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true on success, false on failure</returns>
        private bool CreateChangeset(Context context)
        {
            var watch = Stopwatch.StartNew();
            this.Out.WriteLine("Generating data changeset...");

            int errorId = 0;
            var errors = new List<string>();
            var err = new Action<string>((string error) =>
            {
                errors.Add("Error[" + ++errorId + "]:  " + error);
            });

            var companies = new List<ImportEntityCompany>(100);
            var users = new List<ImportEntityUser>(200);
            context.Changeset = new Changeset
            {
                Companies = companies,
                Users = users,
            };

            var dbCompanies = context.Services.Company.GetAllForImportScripts();
            companies.AddRange(dbCompanies.Select(c => new ImportEntityCompany(c)));
            var companyPlaceCategory = context.Services.Repositories.PlacesCategories.GetDefaultPlaceCategory();
            var companiesIndex = new Dictionary<int, ImportEntityCompany>(dbCompanies.Count);
            foreach (var item in companies)
            {
                if (item.ExistsInDatabase)
                {
                    companiesIndex.Add(item.DbId, item);
                }
            }

            // find company contact emails to help correlate with the file (UNTESTED)
            if (context.DbCompanyCorrelationMode.HasFlag(EntityCorrelationMode.ByEmail))
            {
                var emailFields = context.Services.Repositories.CompanyProfileFields.GetByFieldType(context.Services.NetworkId, new ProfileFieldType[] { ProfileFieldType.Email, });
                foreach (var field in emailFields)
                {
                    var fields = companiesIndex[field.CompanyId].EmailFields;
                    if (fields == null)
                    {
                        fields = companiesIndex[field.CompanyId].EmailFields = new List<CompanyProfileField>();
                    }

                    fields.Add(field);
                }
            }

            // find companies in DB
            // ------------------------
            //

            this.Out.WriteLine();
            this.Out.WriteLine("Searching for existing companies... ");
            foreach (var row in context.Table.Rows)
            {
                if (row.Company == null || row.Company.Name == null)
                {
                    continue;
                }

                var matches = companies
                    .Where(c =>
                    {
                        bool isMatch = false;

                        EntityCorrelationMode mode;
                        if (c.ExistsInDatabase)
                        {
                            mode = context.DbCompanyCorrelationMode;
                        }
                        else
                        {
                            mode = context.FileCompanyCorrelationMode;
                        }

                        if (mode != EntityCorrelationMode.Skip)
                        {
                            if (mode.HasFlag(EntityCorrelationMode.ByEmail) && c.EmailFields != null && c.EmailFields.Count > 0 && row.Company.Email != null)
                            {
                                isMatch |= c.EmailFields.Where(f => f.Value.Equals(row.Company.Email.Value, StringComparison.OrdinalIgnoreCase) || f.Value.Equals(row.Company.Email.ValueWithoutTag, StringComparison.OrdinalIgnoreCase)).Any();
                            }

                            if (mode.HasFlag(EntityCorrelationMode.ByName))
                            {
                                isMatch |= c.Name.Equals(row.Company.Name, StringComparison.OrdinalIgnoreCase);
                            }
                        }

                        return isMatch;
                    })
                    .ToArray();

                ////var lowerCompanyName = row.Company.Name.ToLowerInvariant();
                ////var matches = companies
                ////    .Where(c => c.LowerName == lowerCompanyName)
                ////    .ToArray();
                if (matches.Length > 1)
                {
                    row.Errors.Add("Multiple company match: " + string.Join(", ", matches.Select(m => m.ToString())));
                }
                else if (matches.Length == 1)
                {
                    row.SetAttachment<ImportEntityCompany>(matches[0]);
                }
                else if (context.AllowCreateCompany)
                {
                    var company = new Company();
                    var comp = new ImportEntityCompany(company);
                    comp.LineIndex = row.Index;

                    if (row.Company.Name != null)
                        comp.Update.UpdateField("Name", company.Name, row.Company.Name, c => c.Name = row.Company.Name);
                    if (row.Company.EmailDomain != null)
                        comp.Update.UpdateField("EmailDomain", company.EmailDomain, row.Company.EmailDomain, c => c.EmailDomain = row.Company.EmailDomain);

                    comp.Tags = row.CompanyTags;

                    if (row.Company.WebSite != null)
                    {
                        ////comp.Update.UpdateField("Website", company.Website, row.Company.WebSite, c => c.Website = row.Company.WebSite);
                        if (comp.Fields == null)
                            comp.Fields = new List<IProfileFieldValueModel>();
                        comp.Fields.Add(new CompanyProfileFieldModel(0, ProfileFieldType.Site, row.Company.WebSite, ProfileFieldSource.None));
                    }

                    if (row.Company.Phone != null)
                    {
                        ////comp.Update.UpdateField("Phone", company.Phone, row.Company.Phone, c => c.Phone = row.Company.Phone);
                        if (comp.Fields == null)
                            comp.Fields = new List<IProfileFieldValueModel>();
                        comp.Fields.Add(new CompanyProfileFieldModel(0, ProfileFieldType.Phone, row.Company.Phone, ProfileFieldSource.None));
                    }

                    if (row.Company.Email != null)
                    {
                        ////comp.Update.UpdateField("Email", company.Email, row.Company.Email.Value, c => c.Email = row.Company.Email.Value);
                        if (comp.Fields == null)
                            comp.Fields = new List<IProfileFieldValueModel>();
                        comp.Fields.Add(new CompanyProfileFieldModel(0, ProfileFieldType.Email, row.Company.Email.Value, ProfileFieldSource.None));
                    }

                    if (row.CompanyPlace != null)
                    {
                        comp.Place = new EditPlaceRequest();
                        comp.Place.Name = comp.UpdatedEntity.Name.TrimToLength(80);
                        comp.Place.Address = row.CompanyPlace.Address;
                        comp.Place.City = row.CompanyPlace.City;
                        comp.Place.ZipCode = row.CompanyPlace.ZipCode;
                        comp.Place.CategoryId = companyPlaceCategory.Id;
                        comp.Place.ActingUserId = context.Approver.Id;
                        comp.Place.Country = "France";
                        if (row.CompanyPlace.Latitude != null && row.CompanyPlace.Longitude != null)
                        {
                            comp.Place.Latitude = row.CompanyPlace.Latitude;
                            comp.Place.Longitude = row.CompanyPlace.Longitude;
                        }
                        // CompanyAlias, Alias
                    }

                    companies.Add(comp);
                    row.SetAttachment<ImportEntityCompany>(comp);
                }
                else
                {
                    row.Errors.Add("No company match for '" + row.Company.Name + "'");
                }
            }

            {
                int matches = context.Table.Rows
                    .Select(r => new { Row = r, Company = r.GetAttachment<ImportEntityCompany>(), })
                    .Where(r => r.Company != null && r.Company.ExistsInDatabase)
                    .GroupBy(r => r.Company.DbId)
                    .Count();
                int creates = context.Table.Rows
                    .Select(r => new { Row = r, Company = r.GetAttachment<ImportEntityCompany>(), })
                    .Where(r => r.Company != null && !r.Company.ExistsInDatabase)
                    .GroupBy(r => r.Row.Index)
                    .Count();
                int nomatch = context.Table.Rows
                    .Select(r => new { Row = r, Company = r.GetAttachment<ImportEntityCompany>(), })
                    .Where(r => r.Company == null)
                    .Count();
                this.Out.WriteLine("  Rows with existing company: " + matches.ToString().PadLeft(5));
                this.Out.WriteLine("  Rows with new company:      " + creates.ToString().PadLeft(5));
                this.Out.WriteLine("  Rows no company:            " + nomatch.ToString().PadLeft(5));

                if (nomatch > 0)
                    err("Not all rows match a company");
            }

            // find users
            // ------------------------
            //
            if (context.UserMode != UserMode.Ignore)
            {
                this.Out.WriteLine();
                this.Out.WriteLine("Searching for existing users... ");
                var networkId = context.Services.NetworkId;
                foreach (var row in context.Table.Rows)
                {
                    User user = null;
                    ImportEntityUser userImport = null;
                    bool collect = false;

                    var addresses = row.User.Email != null ? new string[] { row.User.Email.Value, row.User.Email.ValueWithoutTag, } : null;
                    var matchesByEmailInDb = addresses != null ? context.Services.People.GetLiteByEmail(addresses) : null;
                    var applies = row.User.Email != null && false ? context.Services.Repositories.ApplyRequests.FindByImportedId(networkId, "%;Email=" + row.User.Email.Value + ";%") : null;
                    if (matchesByEmailInDb != null && matchesByEmailInDb.Count > 1)
                    {
                        // many users in DB with this email address
                        row.Errors.Add("Multiple user match: " + string.Join(", ", matchesByEmailInDb.Select(m => m.ToString())));
                    }
                    else if (matchesByEmailInDb != null && matchesByEmailInDb.Count == 1)
                    {
                        // one user in database with this email address
                        row.SetAttachment(matchesByEmailInDb[0]);
                        var company = context.Services.Company.GetById(matchesByEmailInDb[0].CompanyId);
                        if (row.Company != null)
                        {
                            int matchIssues = 0;
                            if (row.Company.Name != null)
                            {
                                if (row.Company.Name.ToLowerInvariant() != company.Name.ToLowerInvariant())
                                {
                                    matchIssues++;
                                    row.Warnings.Add("Found user '" + matchesByEmailInDb[0] + "' with a different company '" + company + "' which name does not match '" + row.Company.Name + "'");
                                }
                            }
                        }

                        user = context.Services.People.SelectWithId(matchesByEmailInDb[0].Id);
                        userImport = users.SingleOrDefault(i => i.DbId == user.Id);
                        if (userImport == null)
                        {
                            userImport = new ImportEntityUser(user);
                            userImport.LineIndex = row.Index;
                            users.Add(userImport);
                        }

                        row.SetAttachment(userImport);

                        collect = true;
                    }
                    else if (applies != null && applies.Count > 0)
                    {
                        row.Warnings.Add("User create skipped: email matches ApplyRequests: " + string.Join(", ", applies.Select(x => "Id:" + x.Id + " ImportedId:" + x.ImportedId)));
                    }
                    else if (addresses != null)
                    {
                        // no user match in database
                        // prevent double-insert nonetheless
                        //userInserts.Add(row);
                        var matchesByEmail = users.Where(u => addresses.Any(a => u.Email == a)).ToArray();
                        if (matchesByEmail.Length > 0)
                        {
                            row.Warnings.Add("Email address already in pending changes: '" + string.Join("', '", matchesByEmail.Select(x => x.ToString()).ToArray()) + "'");
                        }
                        else
                        {
                            user = new User();
                            userImport = new ImportEntityUser(user);
                            userImport.LineIndex = row.Index;
                            users.Add(userImport);
                            row.SetAttachment<ImportEntityUser>(userImport);

                            collect = true;

                            var companyImport = row.GetAttachment<ImportEntityCompany>();
                            if (companyImport != null)
                            {
                                userImport.Company = companyImport;
                                userImport.UpdatedEntity.CompanyID = companyImport.DbId;
                            }
                            else
                            {
                                row.Errors.Add("Found no company for this user.");
                            }
                        }
                    }

                    if (collect)
                    {
                        if (user.Email == null && row.User.Email != null)
                        {
                            userImport.Update.UpdateField("Email", user.Email, row.User.Email.Value, (u) => u.Email = row.User.Email.Value);
                        }

                        if (row.User.Phone != null)
                        {
                            ////userImport.Update.UpdateField("Phone", user.Phone, row.User.Phone, (u) => u.Phone = row.User.Phone);
                            if (userImport.Fields == null)
                                userImport.Fields = new List<IProfileFieldValueModel>();
                            userImport.Fields.Add(new UserProfileFieldModel(user.Id, ProfileFieldType.Phone, row.User.Phone, ProfileFieldSource.None));
                        }

                        if (user.FirstName == null && row.User.FirstName != null)
                        {
                            userImport.Update.UpdateField("FirstName", user.FirstName, row.User.FirstName, (u) => u.FirstName = row.User.FirstName);
                        }

                        if (user.LastName == null && row.User.LastName != null)
                        {
                            userImport.Update.UpdateField("LastName", user.LastName, row.User.LastName, (u) => u.LastName = row.User.LastName);
                        }

                        if (user.Gender == null && row.User.Gender != null)
                        {
                            userImport.Update.UpdateField("Gender", user.GenderValue.ToString(), row.User.Gender.Value.ToString(), (u) => u.GenderValue = row.User.Gender.Value);
                        }

                        if (user.Culture == null && row.User.Culture != null)
                        {
                            userImport.Update.UpdateField("Culture", user.Culture, row.User.Culture, u => u.Culture = row.User.Culture);
                        }

                        if (user.Timezone == null && row.User.TimeZone != null)
                        {
                            userImport.Update.UpdateField("Timezone", user.Timezone, row.User.TimeZone, u => u.Timezone = row.User.TimeZone);
                        }
                    }
                }

                int matches = context.Table.Rows
                    .Select(r => new { Row = r, User = r.GetAttachment<ImportEntityUser>(), })
                    .Where(r => r.User != null && r.User.ExistsInDatabase)
                    .Count();
                int creates = context.Table.Rows
                    .Select(r => new { Row = r, User = r.GetAttachment<ImportEntityUser>(), })
                    .Where(r => r.User != null && !r.User.ExistsInDatabase)
                    .Count();
                int nomatch = context.Table.Rows
                    .Select(r => new { Row = r, User = r.GetAttachment<ImportEntityUser>(), })
                    .Where(r => r.User == null)
                    .Count();
                this.Out.WriteLine("  Existing users:          " + matches.ToString().PadLeft(5));
                this.Out.WriteLine("  Users to create:         " + creates.ToString().PadLeft(5));
                this.Out.WriteLine("  Lines with missing data: " + nomatch.ToString().PadLeft(5));

                ////if (nomatch > 0)
                ////    err("Not all rows create or match a user");
            }
            else
            {
                this.Out.WriteLine();
                this.Out.WriteLine("Not searching for existing users. ");
            }

            // find email domains
            // ------------------------
            //
            foreach (var row in context.Table.Rows)
            {
                if (row.Company == null)
                    continue;

                var company = row.Company;
                var import = row.GetAttachment<ImportEntityCompany>();

                var emails = new List<EmailAddress>();
                var domains = new List<string>();

                if (company != null && company.Email != null)
                {
                    emails.Add(company.Email);
                    domains.Add(company.Email.DomainPart);
                }

                if (import != null && import.EmailFields != null)
                {
                    var values = import.EmailFields.Select(x => new EmailAddress(x.Value)).ToArray();
                    emails.AddRange(values);
                    domains.AddRange(values.Select(x => x.DomainPart));
                }

                var domainNames = domains
                    .GroupBy(x => x)
                    .Select(g => new KeyValuePair<string, int>(g.Key, g.Count()))
                    .OrderByDescending(x => x.Value)
                    .ToArray();

                foreach (var item in domainNames)
                {
                    var domainName = item.Key;
                    if (context.Services.Company.FindEmailDomainFromNameAndEmail(company.Name, "contact@" + domainName) != null)
                    {
                        import.Update.UpdateField("EmailDomain", import.EmailDomain, domainName, c => c.EmailDomain = domainName);
                    }
                }
            }

            /*
             * Previous version of the company domain name detector.
             *
            foreach (var company in companies.Where(c => c.EmailDomain == null))
            {
                var emails = users.Where(u => u.Company == company).Select(u => u.Email).ToArray();
                var domainNames = emails.GroupBy(e => e.DomainPart).ToArray();
                var validDomainNames = domainNames
                    .Select(dn => new
                    {
                        DomainName = dn,
                        IsValid = context.Services.Company.FindEmailDomainFromNameAndEmail(company.Name, "contact@" + dn.Key),
                    })
                    .Where(x => x.IsValid != null)
                    .GroupBy(x => x.IsValid)
                    .ToArray();

                if (validDomainNames.Length >= 1)
                {
                    string domainName = validDomainNames[0].Key;

                    if (company.Update.DataObject != null)
                    {
                        company.Update.UpdateField("EmailDomain", company.EmailDomain, domainName, c => c.EmailDomain = domainName);
                    }
                    else
                    {
                        errors.Add("Company change '" + company + "' has an empty entity and cannot be updated");
                    }
                }
                else
                {
                }
            }
            */
            errors.AddRange(context.Table.Rows.Where(r => r.Errors != null && r.Errors.Count > 0).SelectMany(r => r.Errors.Select(re => "Row error (line " + r.Index + "): " + re)));
            
            var warnings = context.Table.Rows.Where(r => r.Warnings != null && r.Warnings.Count > 0).SelectMany(r => r.Warnings.Select(re => "Row warning: " + re)).ToArray();

            if (warnings.Length > 0)
            {
                this.Out.WriteLine(warnings.Length + " warnings were raised. Please review them. ");
                this.Out.Write("- ");
                this.Out.WriteLine(string.Join(Environment.NewLine + "- ", warnings));

                if (!this.AskConfirmation("Continue anyway? (y/n)"))
                    return false;
            }

            if (errors.Count > 0)
            {
                this.Out.WriteLine(errors.Count + " error occured. Fix them to continue. ");
                this.Out.Write("- ");
                this.Out.WriteLine(string.Join(Environment.NewLine + "- ", errors));
                return false;
            }

            // Confirm companies changeset
            // ------------------------
            //
            bool companyChangesAccepted = context.NoConfirmChangeset;
            bool userChangesAccepted = context.NoConfirmChangeset;
            if (!context.NoConfirmChangeset)
            {
                this.Out.WriteLine("Changeset created. Please review changes.");
                this.Out.WriteLine();
                this.Out.WriteLine("COMPANY CHANGESET.");
                this.Out.WriteLine("====================");
                this.Out.WriteLine();

                int i = 0, page = 10;
                foreach (var item in companies)
                {
                    if (item.ExistsInDatabase)
                    {
                        if (item.HasPendingDatabaseChanges)
                        {
                            this.Out.WriteLine("#" + i + " Update company   " + item.ToString());
                        }
                        else
                        {
                            ////this.Out.WriteLine("#" + i + " No change to existing company   " + item.ToString());
                            continue;
                        }
                    }
                    else
                    {
                        if (item.HasPendingDatabaseChanges)
                        {
                            this.Out.WriteLine("#" + i + " Create company   " + item.ToString());
                        }
                        else
                        {
                            this.Out.WriteLine("#" + i + " No change no company   " + item.ToString());
                            continue;
                        }
                    }

                    if (item.Update.HasFieldUpdates)
                    {
                        this.Out.WriteLine("  " + "Field".PadRight(20) + " " + "Previous value".PadRight(30) + " " + "New value".PadRight(30));
                        foreach (var field in item.Update.Fields)
                        {
                            this.Out.WriteLine("  "
                                + (field.Field ?? "-").TrimTextRight(20).PadRight(20) + " "
                                + (field.OldValueString ?? "-").TrimTextRight(30).PadRight(30) + " "
                                + (field.NewValueString ?? "-").TrimTextRight(30).PadRight(30));
                        }
                    }

                    if (item.Fields != null)
                    {
                        foreach (var field in item.Fields)
                        {
                            this.Out.WriteLine("  "
                                + field.Type.ToString().TrimTextRight(20).PadRight(20) + " "
                                + ("-").TrimTextRight(30).PadRight(30) + " "
                                + (field.Value ?? "-").TrimTextRight(30).PadRight(30));
                        }
                    }

                    if (item.Place != null)
                    {
                        var fields = new List<string[]>();
                        if (item.Place.Alias != null)
                            fields.Add(new string[] { "Place.Alias", null, item.Place.Alias, });
                        if (item.Place.Name != null)
                            fields.Add(new string[] { "Place.Name", null, item.Place.Name.TrimToLength(80), });
                        if (item.Place.CategoryId > 0)
                            fields.Add(new string[] { "Place.CategoryId", null, item.Place.CategoryId.ToString(), });
                        if (item.Place.CompanyAlias != null)
                            fields.Add(new string[] { "Place.CompanyAlias", null, item.Place.CompanyAlias, });
                        if (item.Place.Address != null)
                            fields.Add(new string[] { "Place.Address", null, item.Place.Address, });
                        if (item.Place.ZipCode != null)
                            fields.Add(new string[] { "Place.ZipCode", null, item.Place.ZipCode, });
                        if (item.Place.City != null)
                            fields.Add(new string[] { "Place.City", null, item.Place.City, });
                        if (item.Place.Country != null)
                            fields.Add(new string[] { "Place.Country", null, item.Place.Country, });
                        if (item.Place.Description != null)
                            fields.Add(new string[] { "Place.Description", null, item.Place.Description, });

                        if (item.Place.Latitude != null && item.Place.Longitude != null)
                        {
                            fields.Add(new string[] { "Place.Latitude", null, item.Place.Latitude.Value.ToString(), });
                            fields.Add(new string[] { "Place.Longitude", null, item.Place.Longitude.Value.ToString(), });
                        }

                        foreach (var field in fields)
                        {
                            this.Out.WriteLine("  "
                                + field[0].ToString().TrimTextRight(20).PadRight(20) + " "
                                + (field[1] ?? "-").TrimTextRight(30).PadRight(30) + " "
                                + (field[2] ?? "-").TrimTextRight(30).PadRight(30));
                        }
                    }

                    this.Out.WriteLine();

                    if ((i++ % page) == (page - 1))
                    {
                        this.Out.WriteLine("Hit enter to continue...");
                        this.In.ReadLine();
                        this.Out.WriteLine();
                    }
                }

                var companyChanges = companies.Count(c => c.HasPendingDatabaseChanges);
                if (companyChanges == 0)
                {
                    this.Out.WriteLine("No company changes.");
                    companyChangesAccepted = true;
                }
                else
                {
                    companyChangesAccepted = this.AskConfirmation("Do you accept these " + companyChanges + " changes? (y/n)");
                }

                // Confirm users changeset
                // ------------------------
                //
                this.Out.WriteLine();
                this.Out.WriteLine("USER CHANGESET.");
                this.Out.WriteLine("====================");
                this.Out.WriteLine();

                i = 0;
                foreach (var item in users)
                {
                    if (item.ExistsInDatabase && item.HasPendingDatabaseChanges)
                    {
                        this.Out.WriteLine("#" + i + " WILL NOT Update user " + item.ToString() + " (not implemented)");
                    }
                    else if (item.ExistsInDatabase && !item.HasPendingDatabaseChanges)
                    {
                        this.Out.WriteLine("#" + i + " WILL NOT Update user " + item.ToString() + " (no changes to perform)");
                    }
                    else if (!item.ExistsInDatabase)
                    {
                        this.Out.WriteLine("#" + i + " Create user          " + item.ToString());
                    }
                    else
                    {
                        continue;
                    }

                    if (item.Update.HasFieldUpdates)
                    {
                        this.Out.WriteLine("  Company: " + item.Company);
                        this.Out.WriteLine("  " + "Field".PadRight(20) + " " + "Previous value".PadRight(30) + " " + "New value".PadRight(30));
                        this.Out.WriteLine("  " + string.Concat(Enumerable.Repeat("-", 20+2*30+3+1)));
                        foreach (var field in item.Update.Fields)
                        {
                            this.Out.WriteLine("  "
                                + (field.Field ?? "-").TrimTextRight(20).PadRight(20) + " "
                                + (field.OldValueString ?? "-").TrimTextRight(30).PadRight(30) + " "
                                + (field.NewValueString ?? "-").TrimTextRight(30).PadRight(30));
                        }
                    }

                    this.Out.WriteLine();

                    if ((i++ % page) == (page - 1))
                    {
                        this.Out.WriteLine("Hit enter to continue...");
                        this.In.ReadLine();
                        this.Out.WriteLine();
                    }
                }

                userChangesAccepted = this.AskConfirmation("Do you accept these changes? (y/n)");
            }

            return userChangesAccepted && companyChangesAccepted;
        }

        /// <summary>
        /// Push the changeset to the database.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true on success, false on failure</returns>
        protected bool Execute(Context context)
        {
            var watch = Stopwatch.StartNew();
            var companyCat = context.Services.Company.GetDefaultCategory();
            var now = DateTime.UtcNow;

            // Commit companies
            // ------------------------
            //
            int companyIndex = -1;
            foreach (var item in context.Changeset.Companies)
            {
                companyIndex++;

                if (item.HasPendingDatabaseChanges || !item.ExistsInDatabase)
                {
                    var currentCompanyId = 0;
                    List<KeyValuePair<string, string>> importId;

                    if (item.ExistsInDatabase)
                    {
                        importId = ImportedIdHelper.Parse(item.UpdatedEntity.ImportedId) ?? new List<KeyValuePair<string, string>>();
                        importId.Add(new KeyValuePair<string, string>("Sess", context.SessionId));
                        importId.Add(new KeyValuePair<string, string>("L", item.LineIndex.ToString()));
                        ////if (item.RowCompany.Email != null)
                        ////    importId.Add(new KeyValuePair<string, string>("Email", item.RowCompany.Email.Value));
                        context.Services.Company.Update(item.UpdatedEntity);
                        currentCompanyId = item.UpdatedEntity.ID;
                    }
                    else
                    {
                        importId = new List<KeyValuePair<string, string>>();
                        importId.Add(new KeyValuePair<string, string>("Sess", context.SessionId));
                        importId.Add(new KeyValuePair<string, string>("L", item.LineIndex.ToString()));
                        ////if (item.RowCompany.Email != null )
                        ////    importId.Add(new KeyValuePair<string, string>("Email", item.RowCompany.Email.Value));
                        if (item.UpdatedEntity.Alias == null)
                        {
                            item.UpdatedEntity.Alias = context.Services.Company.MakeAlias(item.UpdatedEntity.Name);
                        }

                        item.UpdatedEntity.CreatedDateUtc = now;
                        item.UpdatedEntity.IsApproved = true;
                        item.UpdatedEntity.ApprovedDateUtc = now;
                        item.UpdatedEntity.IsEnabled = true;
                        item.UpdatedEntity.CategoryId = companyCat.Id;
                        item.UpdatedEntity.ImportedId = ImportedIdHelper.Compute(importId);

                        var createdCompany = context.Services.Company.Insert(item.UpdatedEntity);
                        currentCompanyId = item.DbId = createdCompany.ID;
                    }

                    if (item.Tags != null && item.Tags.Count > 0)
                    {
                        foreach (var tagCategory in item.Tags)
                        {
                            var cat = context.GetTagCategoryByAlias(tagCategory.Key);
                            if (cat != null)
                            {
                                foreach (var tagName in tagCategory.Value)
                                {
                                    int id;
                                    IList<Tag2Model> tag;
                                    if (int.TryParse(tagName, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
                                    {
                                        tag = new List<Tag2Model>() { context.Services.Tags.GetTagsById(id), };
                                    }
                                    else
                                    {
                                        tag = context.Services.Tags.GetByNameAndCategory(cat.Id, tagName);
                                    }

                                    var result = context.Services.Tags.AddOrRemoveTag(new Services.Networks.Tags.AddOrRemoveTagRequest
                                    {
                                        AddTag = true,
                                        ActingUserId = context.Approver.Id,
                                        CategoryAlias = cat.Alias,
                                        EntityIdentifier = item.UpdatedEntity.Alias,
                                        EntityTypeName = "Company",
                                        TagId = tag.Count > 0 ? tag[0].Id.ToString() : tagName,
                                    });
                                }
                            }
                        }
                    }

                    if (item.Fields != null && item.Fields.Count > 0)
                    {
                        foreach (var field in item.Fields)
                        {
                            var result = context.Services.ProfileFields.SetCompanyProfileField(item.DbId, field.Type, field.Value, field.Source);
                        }
                    }

                    if (item.Place != null)
                    {
                        item.Place.CompanyAlias = item.UpdatedEntity.Alias;
                        item.Place.Alias = context.Services.Places.MakeAlias(item.Place.Name);
                        var placeImportId = ImportedIdHelper.Parse(item.Place.GetImportedId()) ?? new List<KeyValuePair<string, string>>();
                        placeImportId.Add(new KeyValuePair<string, string>("Sess", context.SessionId));
                        placeImportId.Add(new KeyValuePair<string, string>("L", item.LineIndex.ToString()));
                        ////if (item.RowCompany.Email != null)
                        ////    importId.Add(new KeyValuePair<string, string>("Email", item.RowCompany.Email.Value));
                        item.Place.SetImportedId(ImportedIdHelper.Compute(placeImportId));
                        context.Services.Places.Create(item.Place);
                    }
                }
            }

            var membership = new System.Web.Security.SqlMembershipProvider();
            var membershipConfig = new System.Collections.Specialized.NameValueCollection();
            membershipConfig.Add("requiresQuestionAndAnswer", "False");
            membershipConfig.Add("minRequiredNonalphanumericCharacters", "0");
            membershipConfig.Add("passwordStrengthRegularExpression", null);
            membershipConfig.Add("connectionStringName", null);
            membershipConfig.Add("connectionString", context.Args.ApplicationConfiguration.Tree.ConnectionStrings.NetworkApplicationServices);
            membershipConfig.Add("enablePasswordRetrieval", null);
            membershipConfig.Add("enablePasswordReset", null);
            membershipConfig.Add("applicationName", "/");
            membershipConfig.Add("requiresUniqueEmail", null);
            membershipConfig.Add("maxInvalidPasswordAttempts", null);
            membershipConfig.Add("passwordAttemptWindow", null);
            membershipConfig.Add("commandTimeout", null);
            ////membershipConfig.Add("providerName", "AspNetSqlMembershipProvider");
            membershipConfig.Add("passwordFormat", null);
            membershipConfig.Add("name", "AspNetSqlMembershipProvider");
            membershipConfig.Add("minRequiredPasswordLength", null);
            membershipConfig.Add("passwordCompatMode", null);
            membership.Initialize("AspNetSqlMembershipProvider", membershipConfig);

            // Commit users
            // ------------------------
            //
            int createdUsers = 0, errorUsers = 0;
            if (context.UserMode != UserMode.Ignore)
            {
                foreach (var item in context.Changeset.Users)
                {
                    if (item.ExistsInDatabase && item.HasPendingDatabaseChanges)
                    {
                        errorUsers++;
                        this.Out.WriteLine("CsvUserImport ERROR UNABLE TO UPDATE user " + item + ": this operation is not implemented");
                    }
                    else if (item.ExistsInDatabase && !item.HasPendingDatabaseChanges)
                    {
                        errorUsers++;
                        this.Out.WriteLine("CsvUserImport no action to perform on user " + item);
                    }
                    else
                    {
                        if (context.UserMode == UserMode.Apply || context.UserMode == UserMode.SilentApply)
                        {
                            this.ExecuteUserChangeViaApply(context, item, ref createdUsers, ref errorUsers);
                        }
                        else
                        {
                            this.ExecuteUserChangeDirectly(context, membership, ref createdUsers, ref errorUsers, item);
                        }
                    }
                } 
            }

            this.Out.WriteLine();
            this.Out.WriteLine("CsvUserImport " + createdUsers + "/" + context.Changeset.Users.Count + " users imported successfully in " + watch.Elapsed);
            this.Out.WriteLine();
            return true;
        }

        private void ExecuteUserChangeViaApply(Context context, ImportEntityUser item, ref int createdUsers, ref int errorUsers)
        {
            var now = DateTime.UtcNow;
            var apply = new Sparkle.Services.Networks.Users.ApplyRequestModel();
            var applyEntity = new ApplyRequest();
            var importId = new List<KeyValuePair<string, string>>();
            importId.Add(new KeyValuePair<string, string>("Sess", context.SessionId));
            importId.Add(new KeyValuePair<string, string>("L", item.LineIndex.ToString()));
            if (item.UpdatedEntity.Email != null)
                importId.Add(new KeyValuePair<string, string>("Email", item.UpdatedEntity.Email));
            applyEntity.ImportedId = ImportedIdHelper.Compute(importId);

            if (item.UpdatedEntity != null)
            {
                if (item.UpdatedEntity.FirstName != null)
                    apply.UserDataModel.User.FirstName = item.UpdatedEntity.FirstName;
                if (item.UpdatedEntity.LastName != null)
                    apply.UserDataModel.User.LastName = item.UpdatedEntity.LastName;
                if (item.UpdatedEntity.Username != null)
                    apply.UserDataModel.User.Login = item.UpdatedEntity.Username;
                if (item.UpdatedEntity.CompanyAccessLevel != 0)
                    apply.UserDataModel.User.CompanyAccess = item.UpdatedEntity.CompanyAccess;
                if (item.UpdatedEntity.Email != null)
                    apply.UserDataModel.User.Email = item.UpdatedEntity.Email;
                if (item.UpdatedEntity.Culture != null)
                    apply.UserDataModel.User.Culture=item.UpdatedEntity.Culture;
            }

            if (item.Company != null)
            {
                if (item.Company.DbId > 0)
                {
                    apply.JoinCompanyId = item.Company.DbId;
                    applyEntity.JoinCompanyId = item.Company.DbId;
                }
                else
                {
                    this.Out.WriteLine("CsvUserImport ERROR MISSING CompanyId on user " + item);
                    errorUsers++;
                    return;
                }
            }

            apply.ProcessDataModel.IsApprovedBy = context.Approver.Id;

            applyEntity.Key = Guid.NewGuid();
            applyEntity.DateCreatedUtc = now;

            if (apply.UserDataModel.User.FirstName != null && apply.UserDataModel.User.LastName != null && apply.UserDataModel.User.Email != null)
            {
                applyEntity.DateSubmitedUtc = now;
            }

            applyEntity.NetworkId = context.Services.NetworkId;
            applyEntity.Data = apply.Data;
            applyEntity.CompanyData = apply.CompanyData;
            applyEntity.ProcessData = apply.ProcessData;

            if (context.Inviter != null)
            {
                applyEntity.InvitedByUserId = context.Inviter.Id;
                applyEntity.DateInvitedUtc = now;
            }

            context.Services.Repositories.ApplyRequests.Insert(applyEntity);
            createdUsers++;
            this.Out.WriteLine("CsvUserImport ApplyRequest " + applyEntity + " for " + item);

            if (context.UserMode != UserMode.SilentApply || context.UserMode == UserMode.Apply)
            {
                throw new NotImplementedException("UserMode.Apply is not yet supported.");

                if (context.EmailDelay.TotalMilliseconds > 0)
                {
                    Thread.Sleep(context.EmailDelay);
                }
            }
        }

        private void ExecuteUserChangeDirectly(Context context, SqlMembershipProvider membership, ref int createdUsers, ref int errorUsers, ImportEntityUser item)
        {
            if (item.ExistsInDatabase)
                throw new InvalidOperationException();
            if (!item.HasPendingDatabaseChanges)
                return;

            try
            {
                string firstname = item.UpdatedEntity.FirstName, lastname = item.UpdatedEntity.LastName;
                if (string.IsNullOrWhiteSpace(firstname) || string.IsNullOrWhiteSpace(lastname))
                {
                    errorUsers++;
                    throw new InvalidOperationException("Row " + item.ToString() + " is missing a firstname or lastname");
                }

                var username = item.UpdatedEntity.Username;
                if (username == null)
                {
                    username = context.Services.People.MakeUsernameFromName(firstname, lastname);
                }

                var password = Guid.NewGuid().ToString();
                var email = item.UpdatedEntity.Email;
                if ((email = Validate.EmailAddress(email)) == null)
                {
                    throw new InvalidOperationException("Row " + item.ToString() + " has an invalid email address '" + email + "'");
                }

                MembershipCreateStatus status;
                var mbs = membership.CreateUser(username, password, email, null, null, true, Guid.NewGuid(), out status);
                if (status == MembershipCreateStatus.DuplicateUserName || status == MembershipCreateStatus.DuplicateEmail)
                {
                    mbs = membership.GetUser(username, false);
                }
                else if (status == MembershipCreateStatus.Success)
                {
                    // the user has no password
                    // locking the account will help the login form help the user to reset its password
                    context.Services.People.LockMembershipAccount(mbs.UserName);
                }

                var companyId = item.Company.DbId;
                // TODO: pass that as param?
                var companyAccess = Sparkle.Entities.Networks.CompanyAccessLevel.User;

                var entity = item.UpdatedEntity;
                entity.UserId = (Guid)mbs.ProviderUserKey;
                var importId = new List<KeyValuePair<string, string>>();
                importId.Add(new KeyValuePair<string, string>("Sess", context.SessionId));
                importId.Add(new KeyValuePair<string, string>("L", item.LineIndex.ToString()));
                if (item.UpdatedEntity.Email != null)
                    importId.Add(new KeyValuePair<string, string>("Email", item.UpdatedEntity.Email));
                ////entity.ImportedId = ImportedIdHelper.Compute(importId); // not implemented yes

                entity.AccountClosed = false;
                entity.CompanyAccess = companyAccess;
                entity.CompanyID = companyId;
                entity.Login = username;
                entity.NetworkAccess = Sparkle.Entities.Networks.NetworkAccessLevel.User;
                entity.CreatedDateUtc = DateTime.UtcNow;

                if (entity.GenderValue == NetworkUserGender.Unspecified)
                    entity.GenderValue = context.Services.People.GetDefaultGender();

                entity = context.Services.People.Insert(entity);

                context.Services.Notifications.InitializeNotifications(entity);

                createdUsers++;
            }
            catch (Exception ex)
            {
                errorUsers++;
                this.Error.WriteLine("CsvUserImport failed for user '" + item.ToString() + "' '" + ex.Message + "'" + Environment.NewLine + ex.ToString());
            }
        }

        private bool EndWithError(Context context, ErrorLevel errorLevel, string message)
        {
            this.Out.WriteLine(message);
            context.Args.SysLogger.Info("CsvUserImport", remoteClient, Environment.UserName, errorLevel, message);
            return false;
        }

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

  sparkle -in={file.csv} -u={universe} CsvUserImport
    --header=<HeaderFormat> [--default=<ColumnFormat>={Value}]*
    [--encoding=<Encoding>]
    [--firstcsvline={N}] [--csvseparator={S}] [--AllowCreateCompany]
    [--NoConfirmChangeset]

Arguments: 

  -in={file.csv}              The file path to use for import.
  --header=<HeaderFormat>     Data interpretation instructions for each CSV column.
  [--encoding=<Encoding>]     Input file encoding. Default is UTF8.
  [--firstcsvline={N}]        The index if the first line to parse. Default is 0.
  [--csvseparator={S}]        The CSV column separator. Default is ','.
  [--AllowCreateCompany]      Allows the creation of missing companies.
  [--NoConfirmChangeset]      If the changeset is valid, will not ask for confirmation.
  [--culture]                 The language-country for value interpretation. (fr-FR, en-US...)
  [--UserMode=Ignore]         Do not create users in any way.
  [--UserMode=Create]         Create User entity.         Send emails.
  [--UserMode=Silent]         Create User entity.         Do not send emails.
  [--UserMode=Apply]          Create ApplyRequest entity. Send emails.
  [--UserMode=SilentApply]    Obsolete. Create ApplyRequest entity. Do not send emails.
  [--UserMode=Invite]         Obsolete. Create Invite entity.       Send emails.
  [--approver={username}]     Username of approving user (when UserMode is Apply).
  [--inviter={username}]      Username of inviting user (when UserMode is Apply).
  [--emaildelay=00:00:00]     When sending email, delay each send to pass quotas.
  [--DbCompanyCorrelation=]   Correlate DB   companies with <CorrelationMode>.
  [--FileCompanyCorrelation=] Correlate file companies with <CorrelationMode>.

HeaderFormat:

  [ColumnFormat[+ColumnFormat][={Value}]][,ColumnFormat[+ColumnFormat]]*

ColumnFormat: 

  " + string.Join(", ", Column.Parsers.Select(p => p.Name))+ @"

Encoding:

  UTF-8, ISO-8859-1, UTF-16, UTF-32, US-ASCII...

CorrelationMode:

  " + string.Join(", ", Enum.GetNames(typeof(EntityCorrelationMode))) + @"

WARNING: This command is not suitable to profile fields !!
";

            registry.Register(
                "CsvUserImport",
                "Imports data interactively.",
                () => new CsvUserImport(),
                longDesc);
        }

        private bool AskConfirmation(string message)
        {
            var yesChars = new char[] { 'y', 'Y', };
            var noChars = new char[] { 'n', 'N', };
            for (;;)
            {
                this.Out.WriteLine(message);
                char[] inputBuffer = new char[1];
                //this.In.Read(inputBuffer, 0, 1);
                inputBuffer[0] = (char)this.In.Read();
                if (yesChars.Contains(inputBuffer[0]))
                {
                    return true;
                }

                if (noChars.Contains(inputBuffer[0]))
                {
                    return false;
                }
            }
        }

        public class Context
        {
            private Dictionary<string, string> defaults = new Dictionary<string, string>();
            private TimeSpan emailDelay = TimeSpan.Zero;

            public Context()
            {
                this.FileEncoding = Encoding.UTF8;
                this.DataCulture = CultureInfo.InvariantCulture;
            }

            public IServiceFactory Services { get; set; }

            public SparkleCommandArgs Args { get; set; }

            public Encoding FileEncoding { get; set; }

            public string[] Header { get; set; }

            public Dictionary<string, string> Defaults
            {
                get { return this.defaults; }
            }

            public Table Table { get; set; }

            public int CsvFirstLine { get; set; }

            public char CsvSeparator { get; set; }

            public List<List<string>> CsvData { get; set; }

            public bool AllowCreateCompany { get; set; }

            public bool NoConfirmChangeset { get; set; }

            public CultureInfo DataCulture { get; set; }

            public Changeset Changeset { get; set; }

            public UserMode UserMode { get; set; }

            public Services.Networks.Models.UserModel Approver { get; set; }

            public Services.Networks.Models.UserModel Inviter { get; set; }

            public string SessionId { get; set; }

            public Dictionary<string, TagCategoryModel> TagCategoriesByAlias { get; set; }

            public TimeSpan EmailDelay
            {
                get { return this.emailDelay; }
                set { this.emailDelay = value; }
            }

            internal TagCategoryModel GetTagCategoryByAlias(string alias)
            {
                TagCategoryModel cat = null;
                if (this.TagCategoriesByAlias == null)
                {
                    this.TagCategoriesByAlias = new Dictionary<string, TagCategoryModel>();
                }

                if (this.TagCategoriesByAlias.ContainsKey(alias))
                {
                    cat = this.TagCategoriesByAlias[alias];
                }
                else
                {
                    cat = this.Services.Tags.GetCategoryByAlias(alias);
                }

                return cat;
            }

            public EntityCorrelationMode DbCompanyCorrelationMode { get; set; }

            public EntityCorrelationMode FileCompanyCorrelationMode { get; set; }

            public bool SkipCompanyChangesetValidation { get; set; }

            public bool SkipUserChangesetValidation { get; set; }
        }

        public class Changeset
        {
            public List<ImportEntityUser> Users { get; set; }

            public List<ImportEntityCompany> Companies { get; set; }
        }

        public enum UserMode
        {
            /// <summary>
            /// Do not check or create users.
            /// </summary>
            Ignore,

            Create,

            Invite,

            /// <summary>
            /// Create a Apply form and invite them by email.
            /// </summary>
            Apply,

            /// <summary>
            /// Create a Apply form but do not invite them by email.
            /// </summary>
            SilentApply,
        }

        [Flags]
        public enum EntityCorrelationMode
        {
            Default = 0x0001,
            Skip =    0x0000,
            ByName =  0x0001,
            ByEmail = 0x0002,
        }
    }
}
