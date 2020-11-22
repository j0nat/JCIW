using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCIW.Data.Interfaces
{
    /// <summary>
    /// To be inherited when creating new apps.
    /// </summary>
    public interface IApp
    {
        /// <summary>
        ///  Called when graphics engine draws to screen.
        /// </summary>
        void Draw();
    }
}
