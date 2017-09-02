
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Data.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class InformationNotesService : ServiceBase, IInformationNotesService
    {
        [DebuggerStepThrough]
        internal InformationNotesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IInformationNotesRepository informationNotesRepository
        {
            get { return this.Repo.InformationNotes; }
        }

        public List<InformationNote> GetAll()
        {
            return informationNotesRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .OrderByDescending( i => i.StartDateUtc)
                .ToList();
        }

        public InformationNote Get(int noteId)
        {
            return informationNotesRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(n => n.Id == noteId)
                .FirstOrDefault();
        }

        public int Insert(InformationNote item)
        {
            this.SetNetwork(item);

            return this.informationNotesRepository.Insert(item).Id;
        }

        public InformationNote Update(InformationNote item)
        {
            this.VerifyNetwork(item);

            return this.informationNotesRepository.Update(item);
        }

        public void Delete(InformationNote item)
        {
            this.VerifyNetwork(item);

            this.informationNotesRepository.Delete(item);
        }

        public List<InformationNote> GetActive(DateTime dateTime)
        {
            return this.Repo.InformationNotes
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(note => note.StartDateUtc.ToUniversalTime() <= dateTime && note.EndDateUtc.ToUniversalTime() > dateTime)
                .ToList();
        }

        public IList<InformationNoteModel> GetAllModels()
        {
            var list = this.GetAll();
            return list.Select(i => new InformationNoteModel(i)).ToList();
        }

        public IList<InformationNoteModel> GetCurrent(DateTime utcNow)
        {
            var items = informationNotesRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(i => i.StartDateUtc <= utcNow && utcNow <= i.EndDateUtc)
                .OrderBy(i => i.StartDateUtc)
                .ToArray();
            return items.Select(i => new InformationNoteModel(i)).ToList();
        }

        public EditInformationNoteRequest GetEditModel(int? id, EditInformationNoteRequest model)
        {
            if (model == null)
            {
                model = new EditInformationNoteRequest();
            }

            if (id != null)
            {
                var item = this.informationNotesRepository.GetById(id.Value);
                if (item == null)
                    return null;

                var tmpModel = new InformationNoteModel(item);
                if (!tmpModel.CanEdit)
                    return null;

                model.RefreshFrom(item, this.Services.Context.Timezone);
            }

            return model;
        }

        public EditInformationNoteResult Edit(EditInformationNoteRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new EditInformationNoteResult(request);

            if (!request.IsValid)
            {
                result.Errors.Add(EditInformationNoteError.InvalidRequest, NetworksEnumMessages.ResourceManager);
                return result;
            }

            // check existing item
            InformationNote item = null;
            bool create = true;
            if (request.Id > 0)
            {
                create = false;
                item = this.informationNotesRepository.GetById(request.Id);
                if (item == null)
                {
                    result.Errors.Add(EditInformationNoteError.NoSuchItem, NetworksEnumMessages.ResourceManager);
                    return this.LogResult(result, "InformationNotesService.Edit");
                }
                else
                {
                    var tmpModel = new InformationNoteModel(item);
                    if (!tmpModel.CanEdit)
                    {
                        result.Errors.Add(EditInformationNoteError.CannotEdit, NetworksEnumMessages.ResourceManager);
                    }
                }

                if (item.NetworkId != this.Services.NetworkId)
                {
                    result.Errors.Add(EditInformationNoteError.NoSuchItem, NetworksEnumMessages.ResourceManager);
                    return this.LogResult(result, "InformationNotesService.Edit", "Id:" + request.Id + " Item.NetworkId:" + item.NetworkId + " CurrentNetwork.Id:" + this.Services.NetworkId);
                }
            }
            else
            {
                item = new InformationNote();
                item.NetworkId = this.Services.NetworkId;
                // TODO: item.DateCreatedUtc = now;
            }

            // check user authorization
            var requiredNetworkAccess = new NetworkAccessLevel[]
            {
                NetworkAccessLevel.ContentManager,
                NetworkAccessLevel.ManageInformationNotes,
                NetworkAccessLevel.NetworkAdmin,
                NetworkAccessLevel.SparkleStaff,
            };
            var actingUser = this.Services.People.GetById(request.ActingUserId ?? 0, PersonOptions.Company);
            if (actingUser != null)
            {
                if (actingUser.IsActive && actingUser.NetworkAccessLevel != null && actingUser.NetworkAccessLevel.Value.HasAnyFlag(requiredNetworkAccess))
                {
                    // authorized
                    if (create)
                    {
                        item.UserId = actingUser.Id;
                    }
                }
                else
                {
                    result.Errors.Add(EditInformationNoteError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                }
            }
            else
            {
                result.Errors.Add(EditInformationNoteError.NoSuchActingUser, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return this.LogResult(result, "InformationNotesService.Edit");
            }

            // proceed with the DB change
            var tz = this.Services.Context.Timezone;
            var now = DateTime.UtcNow;
            item.Name = request.Name;
            item.Description = request.Description;
            item.StartDateUtc = tz.ConvertToUtc(request.UserStartDate);
            item.EndDateUtc = tz.ConvertToUtc(request.UserEndDate);

            if (create)
            {
                this.Repo.InformationNotes.Insert(item);
            }
            else
            {
                this.Repo.InformationNotes.Update(item);
            }

            result.Succeed = true;
            result.Item = new InformationNoteModel(item);

            return this.LogResult(result, "InformationNotesService.Edit", "Edited InformationNote " + item.Id);
        }

        public InformationNoteModel GetById(int id)
        {
            var note = this.Get(id);
            return note != null ? new InformationNoteModel(note) : null;
        }

        public PagedListModel<InformationNoteModel> GetList(int offset, int count, InformationNotesFilter filter, InformationNoteOptions options)
        {
            var items = this.Repo.InformationNotes.GetList(offset, count, filter, options);
            var models = items.Select(x => new InformationNoteModel(x)).ToList();
            var total = this.Repo.InformationNotes.CountList(filter);
            return new PagedListModel<InformationNoteModel>(models, total, offset, count);
        }
    }
}
