//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Sparkle.Entities.Networks.Neutral
{
    public partial class TeamMemberPoco
    {
        #region Primitive Properties
    
        public int TeamId
        {
            get { return _teamId; }
            set
            {
                if (this._teamId != value)
                {
                    if (this.Team != null && this.Team.Id != value)
                    {
                        this.Team = null;
                    }
                    this._teamId = value;
                }
            }
        }
        private int _teamId;
    
        public System.DateTime Date
        {
            get;
            set;
        }
    
        public Nullable<int> Notifications
        {
            get;
            set;
        }
    
        public Nullable<int> Rights
        {
            get;
            set;
        }
    
        public int UserId
        {
            get { return _userId; }
            set
            {
                if (this._userId != value)
                {
                    if (this.User != null && this.User.Id != value)
                    {
                        this.User = null;
                    }
                    this._userId = value;
                }
            }
        }
        private int _userId;

        #endregion

        #region Navigation Properties
    
        public virtual UserPoco User
        {
            get { return _user; }
            set
            {
                if (!ReferenceEquals(_user, value))
                {
                    var previousValue = _user;
                    _user = value;
                    FixupUser(previousValue);
                }
            }
        }
        private UserPoco _user;
    
        public virtual TeamPoco Team
        {
            get { return _team; }
            set
            {
                if (!ReferenceEquals(_team, value))
                {
                    var previousValue = _team;
                    _team = value;
                    FixupTeam(previousValue);
                }
            }
        }
        private TeamPoco _team;

        #endregion

        #region Association Fixup
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.TeamMembers.Contains(this))
            {
                previousValue.TeamMembers.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.TeamMembers.Contains(this))
                {
                    User.TeamMembers.Add(this);
                }
                if (UserId != User.Id)
                {
                    UserId = User.Id;
                }
            }
        }
    
        private void FixupTeam(TeamPoco previousValue)
        {
            if (previousValue != null && previousValue.TeamMembers.Contains(this))
            {
                previousValue.TeamMembers.Remove(this);
            }
    
            if (Team != null)
            {
                if (!Team.TeamMembers.Contains(this))
                {
                    Team.TeamMembers.Add(this);
                }
                if (TeamId != Team.Id)
                {
                    TeamId = Team.Id;
                }
            }
        }

        #endregion

    }
}
