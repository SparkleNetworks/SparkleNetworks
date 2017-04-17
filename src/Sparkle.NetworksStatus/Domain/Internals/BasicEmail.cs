
namespace Sparkle.NetworksStatus.Domain.Internals
{
    using SrkToolkit.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BasicEmail<TModel>
    {
        private readonly StringReplacer<BasicEmailModel<TModel>> replacer = new StringReplacer<BasicEmailModel<TModel>>();

        public BasicEmail()
        {
        }

        public StringReplacer<BasicEmailModel<TModel>> Replacer
        {
            get { return this.replacer; }
        }

        public TModel Model { get; set; }

        public string MasterTempalte { get; set; }

        public string Template { get; set; }
    }
}
