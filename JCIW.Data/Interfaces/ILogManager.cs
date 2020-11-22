using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCIW.Data.Interfaces
{
    /// <summary>
    /// This interface is used to expose server / repository logging functionality externally (for example in a server).
    /// </summary>
    public interface ILogManager
    {
        /// <summary>
        /// Save log message.
        /// </summary>
        /// <param name="serviceId">The service ID.</param>
        /// <param name="message">The log message.</param>
        void Add(long serviceId, string message);
    }
}
