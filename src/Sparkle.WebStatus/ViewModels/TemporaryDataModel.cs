
namespace Sparkle.WebStatus.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TemporaryDataModel
    {
        public string JsonItems { get; set; }

        public TemporaryDataModel(string json)
        {
            this.JsonItems = json;
        }
    }
}