
namespace Sparkle.Services.Networks.Models
{
    using SrkToolkit;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Resources;

    public class EnumValueText<TEnum>
        where TEnum : struct
    {
        private readonly Func<CultureInfo, string> getText;
        private readonly TEnum value;

        public EnumValueText(TEnum value)
        {
            this.value = value;
            this.Text = value.ToString();
        }

        public EnumValueText(TEnum value, ResourceManager resourceManager)
        {
            this.value = value;

            this.Text = EnumTools.GetDescription<TEnum>(value, resourceManager);

            this.getText = cultureInfo => EnumTools.GetDescription<TEnum>(value, resourceManager);
        }

        public EnumValueText(TEnum value, ResourceManager resourceManager, params object[] args)
        {
            this.value = value;

            this.Text = String.Format(EnumTools.GetDescription<TEnum>(value, resourceManager), args);

            this.getText = cultureInfo => String.Format(EnumTools.GetDescription<TEnum>(value, resourceManager), args);
        }

        public TEnum Value
        {
            get { return this.value; }
        }

        public string Text { get; set; }

        public string GetText(CultureInfo cultureInfo)
        {
            return this.getText != null ? this.getText(cultureInfo) : this.Value.ToString();
        }
    }
}
