
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Services.Networks.Lang;
    using SrkToolkit;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ExportPeopleModel
    {
        [DataType(DataType.DateTime)]
        [Display(Name = "SubscriptionVerificationDate", ResourceType = typeof(NetworksLabels))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy hh:mm}")]
        public DateTime? SubscriptionDate { get; set; }

        [Required]
        [Display(Name = "ExportType", ResourceType = typeof(NetworksLabels))]
        public ExportType ExportType { get; set; }
        public IList<ExportSeparatorsType> AvailableExportTypes { get; set; }

        [Required]
        [Display(Name = "Encoding", ResourceType = typeof(NetworksLabels))]
        public EncodingType Encoding { get; set; }
        public IDictionary<EncodingType, string> AvailableEncodings { get; set; }

        public int? TargetId { get; set; }

        public bool CanSpecifyDate { get; set; }

        public ExportPeopleModel()
        {
            this.CanSpecifyDate = true;
        }
    }

    public class ExportSeparatorsType
    {
        public ExportType Type { get; set; }

        public string TypeTitle
        {
            get { return EnumTools.GetDescription(this.Type, NetworksEnumMessages.ResourceManager); }
        }

        public string ColumnSeparator { get; set; }

        public string ValuesSeparator { get; set; }

        public string FileExtension { get; set; }

        public ExportSeparatorsType()
        {
        }

        public ExportSeparatorsType(ExportType type, string columnSeparator, string valuesSeparator, string fileExtension)
        {
            this.Type = type;
            this.ColumnSeparator = columnSeparator;
            this.ValuesSeparator = valuesSeparator;
            this.FileExtension = fileExtension;
        }
    }

    public enum EncodingType
    {
        Utf8,
        Utf16,
    }

    public enum ExportType
    {
        CsvCommaSeparated,
        CsvSemicolonSeparated,
        CsvTabSeparated,
    }
}
