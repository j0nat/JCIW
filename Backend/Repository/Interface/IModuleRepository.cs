using Repository.Entities;
using System.Collections.Generic;

namespace Repository.Interface
{
    /// <summary>
    /// The interface is used to read and write module data.
    /// </summary>
    public interface IModuleRepository
    {
        /// <summary>
        /// Create a new app entry.
        /// </summary>
        /// <param name="module">The <see cref="Module"/> app data.</param>
        void Add(Module module);

        /// <summary>
        /// Retrieve <see cref="Module"/> based on id.
        /// </summary>
        /// <param name="id">Module identifier.</param>
        /// <returns>module</returns>
        Module Get(long id);

        /// <summary>
        /// Enable app by module ID.
        /// </summary>
        /// <param name="id">Module identifier.</param>
        void EnableService(long id);

        /// <summary>
        /// Disable app by app ID.
        /// </summary>
        /// <param name="id">Module identifier.</param>
        void DisableService(long id);

        /// <summary>
        /// Delete app by app ID.
        /// </summary>
        /// <param name="id">Module identifier.</param>
        void DeleteService(long id);

        /// <summary>
        /// Retrieve <see cref="Module"/> list of all services.
        /// </summary>
        /// <returns><see cref="Module"/> list.</returns>
        List<Module> RetrieveServices();

        /// <summary>
        /// Retrieves <see cref="Module"/> list of all enabled apps.
        /// </summary>
        /// <returns><see cref="Module"/> list.</returns>
        List<Module> RetrieveEnabledApps();

        /// <summary>
        /// Retrieves <see cref="Module"/> list of all apps.
        /// </summary>
        /// <returns><see cref="Module"/> list.</returns>
        List<Module> RetrieveApps();
    }
}
