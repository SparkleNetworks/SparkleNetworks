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
    public partial class ProjectMemberPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int ProjectId
        {
            get { return _projectId; }
            set
            {
                if (this._projectId != value)
                {
                    if (this.Project != null && this.Project.Id != value)
                    {
                        this.Project = null;
                    }
                    this._projectId = value;
                }
            }
        }
        private int _projectId;
    
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
    
        public virtual ProjectPoco Project
        {
            get { return _project; }
            set
            {
                if (!ReferenceEquals(_project, value))
                {
                    var previousValue = _project;
                    _project = value;
                    FixupProject(previousValue);
                }
            }
        }
        private ProjectPoco _project;
    
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

        #endregion

        #region Association Fixup
    
        private void FixupProject(ProjectPoco previousValue)
        {
            if (previousValue != null && previousValue.ProjectMembers.Contains(this))
            {
                previousValue.ProjectMembers.Remove(this);
            }
    
            if (Project != null)
            {
                if (!Project.ProjectMembers.Contains(this))
                {
                    Project.ProjectMembers.Add(this);
                }
                if (ProjectId != Project.Id)
                {
                    ProjectId = Project.Id;
                }
            }
        }
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.ProjectMembers.Contains(this))
            {
                previousValue.ProjectMembers.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.ProjectMembers.Contains(this))
                {
                    User.ProjectMembers.Add(this);
                }
                if (UserId != User.Id)
                {
                    UserId = User.Id;
                }
            }
        }

        #endregion

    }
}
