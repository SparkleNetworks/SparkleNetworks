
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Data.Spatial;

    partial class Place : IEntityInt32Id, INetworkEntity
    {
        public DbGeography Geography { get; set; }

        public override string ToString()
        {
            return "Place #" + this.Id + (this.ParentId != null ? (" Parent=" + this.ParentId.Value) : null) + " " + this.Name;
        }
    }

    public enum PlaceOptions
    {
        None = 0x000000,
        CreatedBy = 0x000001,
        Category = 0x000002,
    }

    partial class PlaceCategory : IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " " + this.Name;
        }
    }

    partial class PlaceHistory : IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " Place " + this.PlaceId + " by " + this.UserId + " on " + this.Day;
        }
    }

    partial class Number : IEntityInt32Id, INetworkEntity
    {
        public override string ToString()
        {
            return this.Id + " " + this.Name;
        }
    }

    partial class Project : IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " " + this.Name;
        }
    }

    partial class Invited : IEntityInt32Id
    {
        public override string ToString()
        {
            return "Invitation " + this.Id + " for " + this.Email + " " + (this.UserId.HasValue ? ("is user " + this.UserId.Value) : ("was not accepted"));
        }
    }

    partial class CompanyAdmin : IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " user " + this.UserId + " if admin of company " + this.CompanyId;
        }
    }

    partial class Job : IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " " + this.Libelle;
        }
    }

    public enum CompanyTagOptions
    {
        None = 0x0000,
        Company = 0x0001,
        Tag = 0x0002,
    }

    public enum GroupTagOptions
    {
        None = 0x0000,
        Group = 0x0001,
        Tag = 0x0002,
    }

    public enum UserTagOptions
    {
        None = 0x0000,
        User = 0x0001,
        Tag = 0x0002,
    }

    partial class ResumeSkill : IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " resume " + this.ResumeId + " has skill " + this.SkillId;
        }
    }

    partial class TouchCommunication : IEntityInt32Id, INetworkEntity
    {
        public override string ToString()
        {
            return this.Id + " " + this.Date;
        }
    }

    partial class TouchCommunicationItem : IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " in " + this.ParentId + " " + this.Title;
        }
    }

    partial class Menu : IEntityInt32Id
    {
    }

    partial class MenuPlanning : IEntityInt32Id
    {
    }

    partial class Relationship : IEntityInt32Id
    {
    }

    partial class Team : IEntityInt32Id
    {
    }

    partial class Album : IEntityInt32Id
    {
    }

    partial class Picture : IEntityInt32Id
    {
    }

    partial class ExchangeSkill : IEntityInt32Id, INetworkEntity
    {
    }

    partial class ExchangeMaterial : IEntityInt32Id, INetworkEntity
    {
    }
}

