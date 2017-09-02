
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class DeleteJobRequest : LocalBaseRequest
    {
        public DeleteJobRequest()
        {
        }

        /// <summary>
        /// The job to delete.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The job in which to put users that have the job being delete.
        /// </summary>
        [Display(Name = "DeleteJobTarget", ResourceType = typeof(NetworksLabels))]
        public int? TargetJobId { get; set; }

        public IList<JobModel> Jobs { get; set; }
    }

    public class DeleteJobResult : BaseResult<DeleteJobRequest, DeleteJobError>
    {
        public DeleteJobResult(DeleteJobRequest request)
            : base(request)
        {
        }
    }

    public enum DeleteJobError
    {
        NotAuthorized,
        NoSuchActingUser,
        NoSuchItem,
        SameTarget
    }
}
