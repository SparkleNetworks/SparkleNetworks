
namespace Sparkle.Services.Networks.Models.Tags
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TagsListEditable
    {
        public TagsListEditable()
        {
        }
        
        public int CompanyId { get; set; }
        
        public int UserId { get; set; }
        
        public int GroupId { get; set; }
        
        public int EventId { get; set; }

        public string ModelId { get; set; }

        public IList<Sparkle.Services.Networks.Models.Tags.TagModel> Items { get; set; }
    }
}
