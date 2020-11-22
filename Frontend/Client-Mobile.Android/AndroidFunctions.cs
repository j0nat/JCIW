using System;
using Android.Views;
using Android.Views.InputMethods;
using JCIW.Data.Interfaces;

namespace Client_Mobile.Android
{
    class AndroidFunctions : IPlatformFunctions
    {
        public static byte[] ByteArrayLastImageCaptured = new byte[0];
        private readonly InputMethodManager inputMethodManager;
        public View View;
        private readonly Activity1 mainActivity;

        public event EventHandler KeyPressEvent;
        public event EventHandler CameraEvent;
        private bool capitalizedLetters;

        /// <summary>
        /// Create a new implementation of <see cref="IPlatformFunctions"/> for Android.
        /// </summary>
        /// <param name="inputMethodManager"></param>
        /// <param name="mainActivity"></param>
        public AndroidFunctions(InputMethodManager inputMethodManager, Activity1 mainActivity)
        {
            this.inputMethodManager = inputMethodManager;
            this.mainActivity = mainActivity;
            this.capitalizedLetters = false;
        }

        public void CloseKeyboard()
        {
            View.KeyPress -= View_KeyPress;

            inputMethodManager.HideSoftInputFromWindow((View).WindowToken, HideSoftInputFlags.None);
        }

        public void OpenKeyboard()
        {
            View.KeyPress -= View_KeyPress;

            inputMethodManager.ShowSoftInput(View, ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);

            View.KeyPress += View_KeyPress;
        }

        public void InvokeCameraEvent(JCIW.Data.Drawing.ImageSource imageSource)
        {
            if (CameraEvent != null && ByteArrayLastImageCaptured != null)
            {
                imageSource.Image = ByteArrayLastImageCaptured;
                CameraEvent.Invoke(imageSource, EventArgs.Empty);
            }
        }
        private void View_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.KeyCode == Keycode.Back && e.Event.Action == KeyEventActions.Up)
            {
                View.KeyPress -= View_KeyPress;
            }

            if (e.KeyCode == Keycode.ShiftLeft || e.KeyCode == Keycode.ShiftRight)
            {
                if (capitalizedLetters)
                {
                    capitalizedLetters = false;
                }
                else
                {
                    capitalizedLetters = true;
                }
            }

            string input = GetInput(e.KeyCode, capitalizedLetters);

            if (input != "-1" && e.Event.Action == KeyEventActions.Up)
            {
                KeyPressEvent.Invoke(Convert.ToChar(input), EventArgs.Empty);
            }

            if (e.KeyCode == Keycode.Del && e.Event.Action == KeyEventActions.Up)
            {
                KeyPressEvent.Invoke((char)Keycode.Del, EventArgs.Empty);
            }

            if (e.KeyCode == Keycode.Space && e.Event.Action == KeyEventActions.Up)
            {
                KeyPressEvent.Invoke(' ', EventArgs.Empty);
            }
        }

        private string GetInput(Keycode key, bool capitalized)
        {
            switch (key)
            {
                case Keycode.A: return (capitalized ? "A" : "a");
                case Keycode.B: return (capitalized ? "B" : "b");
                case Keycode.C: return (capitalized ? "C" : "c");
                case Keycode.D: return (capitalized ? "D" : "d");
                case Keycode.E: return (capitalized ? "E" : "e");
                case Keycode.F: return (capitalized ? "F" : "f");
                case Keycode.G: return (capitalized ? "G" : "g");
                case Keycode.H: return (capitalized ? "H" : "h");
                case Keycode.I: return (capitalized ? "I" : "i");
                case Keycode.J: return (capitalized ? "J" : "j");
                case Keycode.K: return (capitalized ? "K" : "k");
                case Keycode.L: return (capitalized ? "L" : "l");
                case Keycode.M: return (capitalized ? "M" : "m");
                case Keycode.N: return (capitalized ? "N" : "n");
                case Keycode.O: return (capitalized ? "O" : "o");
                case Keycode.P: return (capitalized ? "P" : "p");
                case Keycode.Q: return (capitalized ? "Q" : "q");
                case Keycode.R: return (capitalized ? "R" : "r");
                case Keycode.S: return (capitalized ? "S" : "s");
                case Keycode.T: return (capitalized ? "T" : "t");
                case Keycode.U: return (capitalized ? "U" : "u");
                case Keycode.V: return (capitalized ? "V" : "v");
                case Keycode.W: return (capitalized ? "W" : "w");
                case Keycode.X: return (capitalized ? "X" : "x");
                case Keycode.Y: return (capitalized ? "Y" : "y");
                case Keycode.Z: return (capitalized ? "Z" : "z");
                case Keycode.Num0: return "0";
                case Keycode.Num1: return "1";
                case Keycode.Num2: return "2";
                case Keycode.Num3: return "3";
                case Keycode.Num4: return "4";
                case Keycode.Num5: return "5";
                case Keycode.Num6: return "6";
                case Keycode.Num7: return "7";
                case Keycode.Num8: return "8";
                case Keycode.Num9: return "9";
                case Keycode.Period: return ".";
                case Keycode.Comma: return ",";
                case Keycode.NumpadDot: return ".";
                default: return "-1";
            }
        }

        public void OpenCamera()
        {
            mainActivity.OpenCamera();
        }

        public float ScreenDensity()
        {
            return mainActivity.ApplicationContext.Resources.DisplayMetrics.Density;
        }

        public void RegisterDesktopKeyboardInput(object window)
        {
            throw new NotImplementedException("Not supported on Android.");
        }

        public void Exit()
        {
            mainActivity.FinishAndRemoveTask();
        }

        public string Browse(string filter, bool saveFile = false, string fileName = "")
        {
            throw new NotImplementedException("Not supported on Android.");
        }
    }
}