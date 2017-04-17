
namespace Sparkle.Common.DataAnnotations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class UrlAttribute : RegularExpressionAttribute
    {
        public UrlAttribute()
            : base("^(https?://)"
                 + "?(([0-9a-zA-Z_!~*'().&=+$%-]+: )?[0-9a-zA-Z_!~*'().&=+$%-]+@)?" //user@
                 + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184
                 + "|" // allows either IP or domain
                 + @"([0-9a-zA-Z_!~*'()-]+\.)*" // tertiary domain(s)- www.
                 + @"([0-9a-zA-Z][0-9a-zA-Z-]{0,61})?[0-9a-zA-Z]\." // second level domain
                 + "[a-zA-Z]{2,6})" // first level domain- .com or .museum
                 + "(:[0-9]{1,4})?" // port number- :80
                 + "((/?)|" // a slash isn't required if there is no file name
                 + "(/[0-9a-zA-Z_!~*'().;?:@&=+$,%#-]+)+/?)$")
        {
            this.ErrorMessage = "Cette adresse n'est pas correcte.";
        }
    }
}
