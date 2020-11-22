using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCIW.Data.Interfaces
{
    /// <summary>
    /// Handles the graphical window for the app.
    /// </summary>
    public interface IFrame
    {
        /// <summary>
        /// Called when the screen size changes.
        /// </summary>
        /// <param name="height">The screen height.</param>
        void SetScreenHeight(float height);

        /// <summary>
        /// Called when the screen size changes.
        /// </summary>
        /// <param name="width">The screen width.</param>
        void SetScreenWidth(float width);

        /// <summary>
        /// Called when the frame updates.
        /// </summary>
        void Update();
    }
}
