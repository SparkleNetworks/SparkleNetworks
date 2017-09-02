
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Sparkle.UI;

    public static class SparkleLang
    {
        public static Strings CreateStrings(string baseDirectory, string universeName, string networkTypeName)
        {
            if (string.IsNullOrEmpty(baseDirectory))
                throw new ArgumentException("The value cannot be empty", "baseDirectory");
            if (string.IsNullOrEmpty(universeName))
                throw new ArgumentException("The value cannot be empty", "universeName");

            try
            {
                var pots = new List<PotLoad>();
                // Network-specific sourcs
                string networksPotDirectory = Path.Combine(baseDirectory, "Networks");
                pots.Add(new PotLoad(networksPotDirectory, universeName, "fr-fr"));
                pots.Add(new PotLoad(networksPotDirectory, "default", "fr-fr"));

                // NetworkType-specific source
                string networkTypesPotDirectory = Path.Combine(baseDirectory, "NetworkTypes");
                if (networkTypeName != null)
                {
                    pots.Add(new PotLoad(networkTypesPotDirectory, networkTypeName, "fr-fr"));
                }
                else
                {
                    pots.Add(new PotLoad(networkTypesPotDirectory, "default", "fr-fr"));
                }

                // root source
                string commonPotDirectory = Path.Combine(baseDirectory, "Common");
                pots.Add(new PotLoad(commonPotDirectory, "default", "fr-fr"));

                Strings thePot = null, rootPot = null;
                string files = "pot files: ";
                for (int i = 0; i < pots.Count; i++)
                {
                    var pot = pots[i];
                    files += Environment.NewLine + pot.ToString() + ", ";
                    try
                    {
                        if (thePot == null)
                        {
                            rootPot = thePot = Strings.Load(pot.Directory, pot.Application, pot.Culture);
                        }
                        else
                        {
                            thePot.Fallback = Strings.Load(pot.Directory, pot.Application, pot.Culture);
                            thePot = thePot.Fallback;
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        Trace.TraceError("ConfigureLangSource: failed to load pot file (duplicate key?) '" + pot.Directory + "' '" + pot.Application + "' '" + pot.Culture + "'" + ex.ToString());
                        throw;
                    }
                }

                Debug.WriteLine("ConfigureLangSource: loaded files: " + files);

                return rootPot;
            }
            catch (ArgumentException ex)
            {
                // duplicate key in pot files
                Trace.TraceError("ConfigureLangSource: " + ex.ToString());
                throw new InvalidOperationException("Failed to ConfigureLangSource", ex);
            }
        }

        public class PotLoad
        {
            public PotLoad()
            {
            }

            public PotLoad(string directory, string application, string defaultCulture)
            {
                this.Directory = directory;
                this.Application = application;
                this.Culture = defaultCulture;
            }

            public string Directory { get; set; }
            public string Application { get; set; }
            public string Culture { get; set; }

            public override string ToString()
            {
                return "'" + this.Directory + "/" + this.Application + "/" + this.Culture + "'";
            }
        }
    }
}
