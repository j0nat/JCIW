using System;
using System.Windows.Forms;
using JCIW.Data.Interfaces;
using Microsoft.Xna.Framework;

namespace Client_Desktop
{
    /// <summary>
    /// This class implements <see cref="IPlatformFunctions"/>.
    /// </summary>
    class DesktopFunctions : IPlatformFunctions
    {
        public event EventHandler KeyPressEvent;
        public event EventHandler CameraEvent;

        public string Browse(string filter, bool saveFile = false, string fileName = "")
        {
            string returnPath = null;

            if (saveFile)
            {
                SaveFileDialog openFileDialog = new SaveFileDialog();
                openFileDialog.FileName = fileName;
                openFileDialog.Filter = filter;

                DialogResult result = openFileDialog.ShowDialog();

                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    string selectedFile = openFileDialog.FileName;

                    if (selectedFile.Length != 0)
                    {
                        returnPath = selectedFile;
                    }
                }
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.FileName = fileName;
                openFileDialog.Filter = filter;

                DialogResult result = openFileDialog.ShowDialog();

                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    string selectedFile = openFileDialog.FileName;

                    if (selectedFile.Length != 0)
                    {
                        returnPath = selectedFile;
                    }
                }
            }

            return returnPath;
        }

        public void CloseKeyboard()
        {
            throw new NotImplementedException("Not supported on desktop.");
        }

        public void Exit()
        {
            Environment.Exit(0);
        }

        public void OpenCamera()
        {
            throw new NotImplementedException("Not supported on desktop.");
        }

        public void OpenKeyboard()
        {
            throw new NotImplementedException("Not supported on desktop.");
        }

        public void RegisterDesktopKeyboardInput(object window)
        {
            Game game = (Game)window;

            game.Window.TextInput += (s, a) =>
            {
                if (a.Character == '\t') return;

                KeyPressEvent.Invoke(a.Character, EventArgs.Empty);
            };
        }

        public float ScreenDensity()
        {
            return 1;
        }
    }
}
