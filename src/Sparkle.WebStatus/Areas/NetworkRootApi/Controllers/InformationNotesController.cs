
namespace Sparkle.WebStatus.Areas.NetworkRootApi.Controllers
{
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Services.Networks.Models;
    using Sparkle.WebBase.WebApi;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Filters;

    [RoutePrefix(NetworkRootApiAreaRegistration.BasePath)]
    public class InformationNotesController : ApiController
    {
        [Route("InformationNotes/List"), HttpGet, HttpPost]
        [CheckNetworkApiKeyAttribute(Roles = "ReadInformationNotes")]
        public BaseResponse<PagedListModel<InformationNoteModel>> List(
            int Offset = 0, int Count = 20,
            InformationNotesKnownFilter KnownFilter = InformationNotesKnownFilter.All)
        {
            var filter = new InformationNotesFilter();
            filter.Filter = KnownFilter;
            var options = new InformationNoteOptions();
            var result = this.Services.InformationNotes.GetList(Offset, Count, filter, options);
            return new BaseResponse<PagedListModel<InformationNoteModel>>(result);
        }

        [Route("InformationNotes/ById/{id}"), HttpGet]
        [CheckNetworkApiKeyAttribute(Roles = "ReadInformationNotes")]
        public BaseResponse<InformationNoteModel> GetById(int id)
        {
            var result = this.Services.InformationNotes.GetById(id);
            return new BaseResponse<InformationNoteModel>(result);
        }

        [Route("InformationNotes/PrepareEdit"), HttpGet]
        [CheckNetworkApiKeyAttribute(Roles = "ReadInformationNotes,WriteInformationNotes")]
        public BaseResponse<EditInformationNoteRequest> GetPrepareEdit()
        {
            var result = this.Services.InformationNotes.GetEditModel(null, null);
            return new BaseResponse<EditInformationNoteRequest>(result);
        }

        [Route("InformationNotes/PrepareEdit/{id}"), HttpGet]
        [CheckNetworkApiKeyAttribute(Roles = "ReadInformationNotes,WriteInformationNotes")]
        public BaseResponse<EditInformationNoteRequest> GetPrepareEdit(int id)
        {
            var result = this.Services.InformationNotes.GetEditModel(id, null);
            return new BaseResponse<EditInformationNoteRequest>(result);
        }

        [Route("InformationNotes/PrepareEdit"), HttpPost]
        [CheckNetworkApiKeyAttribute(Roles = "ReadInformationNotes,WriteInformationNotes")]
        public BaseResponse<EditInformationNoteRequest> PostPrepareEdit(EditInformationNoteRequest request)
        {
            var result = this.Services.InformationNotes.GetEditModel(request.Id, request);
            return new BaseResponse<EditInformationNoteRequest>(result);
        }

        [Route("InformationNotes/Edit"), HttpPost]
        [CheckNetworkApiKeyAttribute(Roles = "WriteInformationNotes")]
        public BaseResponse<EditInformationNoteResult> Edit(EditInformationNoteRequest request)
        {
            this.ValidateRequestWrapped(request);
            var result = this.Services.InformationNotes.Edit(request);
            return this.DomainResultWrapped(result);
        }
    }
}
