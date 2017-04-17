
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Common;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.PartnerResources;
    using SrkToolkit.Common.Validation;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    
    public class AdminWorkModel
    {
        public AdminWorkModel()
        {
        }

        public AdminWorkModel(AdminWorkType type, AdminWorkTask task, AdminWorkPriority priority, DateTime dateUtc)
        {
            this.Type = type;
            this.Task = task;
            this.Priority = priority;
            this.DateUtc = dateUtc;
        }

        public AdminWorkPriority Priority { get; set; }

        public AdminWorkType Type { get; set; }

        public AdminWorkTask Task { get; set; }

        public int IntId { get; set; }

        public DateTime DateUtc { get; set; }

        public string ItemName { get; set; }
        public string ItemAlias { get; set; }
        public Uri ItemUrl { get; set; }

        public string TaskUrl { get; set; }
        public string TaskLocalUrl { get; set; }

        public string PriorityTitle
        {
            get { return SrkToolkit.EnumTools.GetDescription(this.Priority, NetworksEnumMessages.ResourceManager); }
        }

        public string TaskTitle
        {
            get { return SrkToolkit.EnumTools.GetDescription(this.Task, NetworksEnumMessages.ResourceManager); }
        }

        public string TypeTitle
        {
            get { return SrkToolkit.EnumTools.GetDescription(this.Type, NetworksEnumMessages.ResourceManager); }
        }

        public string Age
        {
            get { return this.DateUtc.AsUtc().ToNiceDelay(DateTime.UtcNow); }
        }

        public static AdminWorkModel For(IServiceFactory services, CompanyRequest model, AdminWorkPriority priority)
        {
            var item = new AdminWorkModel
            {
                Type = AdminWorkType.NewCompanyRequestToManage,
                Priority = priority,
                Task = AdminWorkTask.AcceptOrRefuse,
                IntId = model.Id,
                DateUtc = model.CreatedDateUtc.AsUtc(),
                ItemName = model.Name,
            };

            if (model.ClosedDateUtc == null && model.BlockedDateUtc == null)
            {
                item.Task = AdminWorkTask.AcceptOrRefuse;
            }
            else
            {
                item.Task = AdminWorkTask.None;
            }

            return Setup(services, item);
        }

        public static AdminWorkModel For(IServiceFactory services, Users.ApplyRequestModel model, AdminWorkPriority priority)
        {
            var item = new AdminWorkModel
            {
                Type = AdminWorkType.NewApplyRequestToManage,
                Priority = priority,
                Task = AdminWorkTask.AcceptOrRefuse,
                IntId = model.Id,
                DateUtc = model.DateSubmitedUtc ?? model.DateCreatedUtc,
                ItemName = model.UserDataModel != null && model.UserDataModel.User != null ? model.UserDataModel.User.Email : null,
            };

            if (model.IsPendingAccept)
            {
                item.Task = AdminWorkTask.AcceptOrRefuse;
            }
            else
            {
                item.Task = AdminWorkTask.None;
            }
            
            return Setup(services, item);
        }

        public static AdminWorkModel For(IServiceFactory services, PartnerResource model, AdminWorkPriority priority)
        {
            var item = new AdminWorkModel
            {
                Type = AdminWorkType.PartnerResourceToApprove,
                Priority = priority,
                Task = AdminWorkTask.AcceptOrRefuse,
                IntId = model.Id,
                ItemAlias = model.Alias,
                DateUtc = model.DateCreatedUtc,
            };

            if (!model.IsApproved && !model.DateDeletedUtc.HasValue)
            {
                item.Task = AdminWorkTask.AcceptOrRefuse;
            }
            else
            {
                item.Task = AdminWorkTask.None;
            }

            return Setup(services, item);
        }

        public static AdminWorkModel For(IServiceFactory services, UserEmailChangeRequest model, AdminWorkPriority priority)
        {
            var item = new AdminWorkModel
            {
                Type = AdminWorkType.UserEmailChangeRequest,
                Priority = priority,
                Task = AdminWorkTask.Other,
                IntId = model.Id,
                DateUtc = model.CreateDateUtc.AsUtc(),
                ItemName = model.User != null ? model.User.FullName : null,
                ItemUrl = model.User != null ? new Uri(services.People.GetProfileUrl(model.User, UriKind.Absolute), UriKind.Absolute) : null,
            };

            return Setup(services, item);
        }

        public static AdminWorkModel For(IServiceFactory services, RegisterRequest model, AdminWorkPriority priority)
        {
            var item = new AdminWorkModel
            {
                Type = AdminWorkType.RegisterRequest,
                Priority = priority,
                Task = AdminWorkTask.AcceptOrRefuse,
                IntId = model.Id,
                DateUtc = model.DateCreatedUtc.AsUtc(),
                ItemName = model.EmailAddress,
            };

            if (model.StatusCode == RegisterRequestStatus.New)
            {
                item.Task = AdminWorkTask.AcceptOrRefuse;
            }
            else
            {
                item.Task = AdminWorkTask.None;
            }

            return Setup(services, item);
        }

        public static AdminWorkModel For(IServiceFactory services, GetCompaniesAccessLevelReport_Result model, AdminWorkPriority priority, AdminWorkType type)
        {
            var item = new AdminWorkModel
            {
                Type = type,
                Priority = priority,
                Task = AdminWorkTask.Other,
                IntId = model.Id,
                DateUtc = DateTime.UtcNow,
                ItemName = model.Name,
                ItemAlias = model.Alias,
                ItemUrl = new Uri(services.Company.GetProfileUrl(model.Alias, UriKind.Absolute), UriKind.Absolute),
            };

            return Setup(services, item);
        }

        public static AdminWorkModel For(IServiceFactory services, Ads.AdModel model, AdminWorkPriority priority)
        {
            var item = new AdminWorkModel
            {
                Type = model.PendingEditDate != null ? AdminWorkType.EditAdToApprove : AdminWorkType.NewAdToApprove,
                Priority = priority,
                Task = AdminWorkTask.AcceptOrRefuse,
                IntId = model.Id,
                ItemAlias = model.Alias,
                DateUtc = model.PendingEditDate ?? model.Date,
            };

            if (model.PendingEditDate != null || model.IsValidated == null)
            {
                item.Task = AdminWorkTask.AcceptOrRefuse;
            }
            else
            {
                item.Task = AdminWorkTask.None;
            }

            return Setup(services, item);
        }

        private static AdminWorkModel Setup(IServiceFactory services, AdminWorkModel item)
        {
            var query = new Dictionary<string, string>();
            query.Add("Type", item.Type.ToString());
            query.Add("Task", item.Task.ToString());
            if (item.IntId != null)
                query.Add("IntId", item.IntId.ToString());
            if (item.ItemAlias != null)
                query.Add("ItemAlias", item.ItemAlias);
            item.TaskUrl = services.GetUrl("/Dashboard/Task", query);
            item.TaskLocalUrl = services.GetLocalUrl("/Dashboard/Task", query);
            return item;
        }
    }
}
