
namespace Sparkle.Services.Networks
{
    using Sparkle.Entities.Networks;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class AddGroupTagRequest : BaseRequest
    {
        public AddGroupTagRequest()
            : base()
        {
    
        }

        public AddGroupTagRequest(int userid, int groupid, string tagname)
            : base()
        {
            UserId = userid;
            GroupId = groupid;
            TagName = tagname;
        }

        public int UserId { get; set; }

        public int GroupId { get; set; }

        public string TagName { get; set; }

        public int TagId { get; set; }

        public WallItemDeleteReason Reason { get; set; }
    }
}
