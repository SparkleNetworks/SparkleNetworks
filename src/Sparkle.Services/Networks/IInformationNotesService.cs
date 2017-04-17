
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;

    public interface IInformationNotesService
    {
        List<InformationNote> GetAll();
        InformationNote Get(int noteId);
        int Insert(InformationNote item);
        InformationNote Update(InformationNote item);
        void Delete(InformationNote item);

        List<InformationNote> GetActive(DateTime dateTime);
        IList<InformationNoteModel> GetCurrent(DateTime utcNow);

        IList<InformationNoteModel> GetAllModels();

        /// <summary>
        /// Prepare to create or edit an information note.
        /// </summary>
        EditInformationNoteRequest GetEditModel(int? id, EditInformationNoteRequest model);

        /// <summary>
        /// Create or edit an information note.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        EditInformationNoteResult Edit(EditInformationNoteRequest request);

        InformationNoteModel GetById(int id);

        PagedListModel<InformationNoteModel> GetList(int offset, int count, InformationNotesFilter filter, InformationNoteOptions options);
    }
}
