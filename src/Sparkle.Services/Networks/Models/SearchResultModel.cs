
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SearchResultModel<T>
    {
        private T item;
        
        public SearchResultModel()
        {
        }

        public SearchResultModel(T item)
        {
            this.item = item;
        }

        public T Item
        {
            get { return this.item; }
            set { this.item = value; }
        }
    }
}
