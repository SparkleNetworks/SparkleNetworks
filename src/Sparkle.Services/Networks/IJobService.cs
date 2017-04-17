
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Users;

    public interface IJobService
    {
        Job AddJob(Job item);
        System.Collections.Generic.IList<Job> Search(string request);
        System.Collections.Generic.IList<Job> SelectAll();
        Job SelectWithAlias(string alias);
        Job SelectWithId(int? id);

        int Count();

        IList<JobModel> GetAll();
        IList<JobModel> GetAllWithCounts();

        void AddManyJob(string[] jobs);

        string MakeAlias(string name);

        JobModel GetById(int id);

        DeleteJobRequest GetDeleteRequest(int? id, DeleteJobRequest request);
        DeleteJobResult Delete(DeleteJobRequest request);

        EditJobRequest GetEditRequest(int? id, EditJobRequest request);
        EditJobResult Edit(EditJobRequest request);
    }
}
