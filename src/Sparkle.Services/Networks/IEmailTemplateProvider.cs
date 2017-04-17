
namespace Sparkle.Services.Networks
{
    using System;
    using System.Globalization;
    using Sparkle.Services.EmailTemplates;

    public interface IEmailTemplateProvider
    {
        string Process<T>(string key, string network, CultureInfo culture, TimeZoneInfo timeZone, T model)
            where T : BaseEmailModel;
    }
}
