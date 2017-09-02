
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DeleteGroup
    {
        public DeleteGroup()
        {
        }

        public DeleteGroup(Group item)
        {
            this.Group = new GroupModel(item);
            this.MessageForMembers = "";
        }

        public GroupModel Group { get; set; }

        public string MessageForMembers { get; set; }
    }
}
