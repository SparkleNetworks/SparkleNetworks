
namespace Sparkle.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// Utility method that help transform strings.
    /// </summary>
    public static class StringTransformer
    {
        //
        // INFORMATION
        //
        // Most of this class has moved to SrkToolkit.Common
        //
        public enum AmountCurrency : short
        {
            USD = 840,
            EUR = 978,
        }

        public static string ToAmount(this decimal value, AmountCurrency currency)
        {
            string result;
            string number;
            switch (currency)
            {
                case AmountCurrency.USD:
                    number = value.ToString("F2");
                    result = "$ " + number;
                    break;

                case AmountCurrency.EUR:
                    number = value.ToString("F2");
                    result = "€ " + number;
                    break;

                default:
                    throw new ArgumentException("Unknown currency " + currency, "currency");
            }

            return result;
        }
    }
}
