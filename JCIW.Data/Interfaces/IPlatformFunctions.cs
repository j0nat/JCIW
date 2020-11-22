using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCIW.Data.Interfaces
{
    /// <summary>
    /// This interface is implemented on the platform (android, windows) where the app is launched from.
    /// </summary>
    public interface IPlatformFunctions
    {
        /// <summary>
        /// Open a browser dialog.
        /// </summary>
        /// <param name="filter">The file filter.</param>
        /// <returns>The file selected by the user.</returns>
        string Browse(string filter, bool saveFile = false, string fileName = "");

        /// <summary>
        /// Get the screen's pixel density.
        /// </summary>
        /// <returns>Screen pixel density</returns>
        float ScreenDensity();

        /// <summary>
        /// Open mobile keyboard.
        /// </summary>
        void OpenKeyboard();

        /// <summary>
        /// Close mobile keyboard.
        /// </summary>
        void CloseKeyboard();

        /// <summary>
        /// Register keyboard input to KeyPressEvent event. 
        /// </summary>
        /// <param name="window">The graphical window object.</param>
        void RegisterDesktopKeyboardInput(object window);

        /// <summary>
        /// Open mobile keyboard.
        /// </summary>
        void OpenCamera();

        /// <summary>
        /// Exit the program.
        /// </summary>
        void Exit();

        /// <summary>
        /// Raised when key is pressed.
        /// </summary>
        event EventHandler KeyPressEvent;

        /// <summary>
        /// Raised when the phone has finished caputing a photo.
        /// </summary>
        event EventHandler CameraEvent;
    }
}