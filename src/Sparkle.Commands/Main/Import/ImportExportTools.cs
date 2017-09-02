
namespace Sparkle.Commands.Main.Import
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class ImportExportTools
    {
        public static bool SplitFirstAndLastName(string fullName, out string firstName, out string lastName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                firstName = lastName = null;
                return false;
            }

            fullName = fullName.Trim();

            int namePos = fullName.IndexOf(' ');
            if (namePos > 0)
            {
                firstName = fullName.Substring(0, namePos);
                lastName = fullName.Substring(namePos + 1);
                return true;
            }
            else
            {
                firstName = lastName = null;
                return false;
            }
        }

        public static bool SplitLastAndFirstName(string fullName, out string firstName, out string lastName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                firstName = lastName = null;
                return false;
            }

            fullName = fullName.Trim();

            int namePos = fullName.LastIndexOf(' ');
            if (namePos > 0)
            {
                lastName = fullName.Substring(0, namePos);
                firstName = fullName.Substring(namePos + 1);
                return true;
            }
            else
            {
                firstName = lastName = null;
                return false;
            }
        }
    }
}
