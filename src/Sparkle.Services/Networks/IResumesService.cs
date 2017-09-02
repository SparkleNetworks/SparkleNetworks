using System.Collections.Generic;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface IResumesService
    {
        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        Resume Insert(Resume item);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        Resume Update(Resume item);

        /// <summary>
        /// Selects the approved.
        /// </summary>
        /// <returns></returns>
        IList<Resume> SelectApproved(bool allNetworks = false);

        /// <summary>
        /// Selects the unapproved.
        /// </summary>
        /// <returns></returns>
        IList<Resume> SelectUnapproved(bool allNetworks = false);

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        IList<Resume> SelectAll(bool allNetworks = false);

        /// <summary>
        /// Gets the resume by id.
        /// </summary>
        /// <param name="resumeId">The resume id.</param>
        /// <returns></returns>
        Resume GetById(int resumeId);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Delete(Resume item);

        /// <summary>
        /// Gets the by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        Resume GetByEmail(string email);

        /// <summary>
        /// Counts the approved resumes.
        /// </summary>
        /// <param name="allNetworks">if set to <c>true</c> [all networks].</param>
        /// <returns></returns>
        int CountApproved(bool allNetworks = false);

        /// <summary>
        /// Counts the unapproved resumes.
        /// </summary>
        /// <param name="allNetworks">if set to <c>true</c> [all networks].</param>
        /// <returns></returns>
        int CountUnapproved(bool allNetworks = false);

        int CountPending(bool allNetworks = false);
    }
}
