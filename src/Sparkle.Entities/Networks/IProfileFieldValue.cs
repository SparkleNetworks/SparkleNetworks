
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IProfileFieldValue
    {
        int Id { get; set; }

        ProfileFieldType ProfileFieldType { get; set; }

        ProfileFieldSource SourceType { get; set; }

        int ProfileFieldId { get; set; }

        string Value { get; set; }

        DateTime DateCreatedUtc { get; set; }

        DateTime? DateUpdatedUtc { get; set; }

        short UpdateCount { get; set; }

        byte Source { get; set; }

        string Data { get; set; }
    }

    public interface ICompanyProfileFieldValue : IProfileFieldValue
    {
        int CompanyId { get; set; }
    }
}
