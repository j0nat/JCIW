using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;

using JCIW.Data;
using JCIW;
using System.IO;
using System.Reflection;
using System;

using Client_Networking.Android;
using Android.Views.InputMethods;
using Android.Content;
using Android.Runtime;
using Android;
using Android.Support.V13.App;

namespace Client_Mobile.Android
{
    /// <summary>
    /// This is the main activity for the android implementation.
    /// </summary>
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@mipmap/ic_launcher",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    public class Activity1 : AndroidGameActivity
    {
        private Game game;
        private View view;
        private AndroidFunctions androidFunctions;
        private readonly int camera_id = 10;

        /// <summary>
        /// Create MonoGame instance and set the content view.
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            var requiredPermissions = new String[] { 
                Manifest.Permission.ReadExternalStorage,
                Manifest.Permission.WriteExternalStorage,
                Manifest.Permission.Camera};

            ActivityCompat.RequestPermissions(this, requiredPermissions, 123);

            InputMethodManager inputMethodManager = GetSystemService(InputMethodService) as InputMethodManager;
            androidFunctions = new AndroidFunctions(inputMethodManager, this);

            JCIW.App.MainApp main = new JCIW.App.MainApp(new AndroidNetwork(), androidFunctions, Platform.Android);

            game = (Game)main.Window();

            view = game.Services.GetService(typeof(View)) as View;
            androidFunctions.View = view;

            SetContentView(view);

            game.Run();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == 123)
            {
                bool permissionsNotGiven = false;

                if (permissions.Length == 3)
                {
                    if (grantResults[0] == Permission.Granted && grantResults[1] == Permission.Granted
                        && grantResults[2] == Permission.Granted)
                    {
                        CopyLibraries();
                    }
                    else
                    {
                        permissionsNotGiven = true;
                    }
                }
                else
                {
                    permissionsNotGiven = true;
                }

                if (permissionsNotGiven)
                {
                    Finish();
                }
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <summary>
        /// Copy dependent JCIW module libraries from embedded resource to android file system.
        /// </summary>
        private void CopyLibraries()
        {
            string path = CreateDirectory();

            try
            {
                CopyModuleDependencyFromEmbeddedResource(path, "ImGui.NET.dll");
                CopyModuleDependencyFromEmbeddedResource(path, "JCIW.Data.dll");
                CopyModuleDependencyFromEmbeddedResource(path, "JCIW.dll");
                CopyModuleDependencyFromEmbeddedResource(path, "JCIW.Module.dll");
                CopyModuleDependencyFromEmbeddedResource(path, "Networking.Data.dll");
                CopyModuleDependencyFromEmbeddedResource(path, "Newtonsoft.Json.dll");
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine("Error copying JCIW dependency files: " + err.ToString());
            }
        }

        /// <summary>
        /// Check if Android/Data/..../ override folder exists. If not then create it.
        /// </summary>
        /// <returns>Path to the override folder.</returns>
        private string CreateDirectory()
        {
            bool directoryCreated = false;

            string parentDirectory = "/storage/emulated/0/Android/data/" + ApplicationContext.PackageName;
            if (!Directory.Exists(parentDirectory))
            {
                Directory.CreateDirectory(parentDirectory);
                directoryCreated = true;
            }

            if (!Directory.Exists(parentDirectory + "/files"))
            {
                Directory.CreateDirectory(parentDirectory + "/files");
                directoryCreated = true;
            }

            if (!Directory.Exists(parentDirectory + "/files/.__override__"))
            {
                Directory.CreateDirectory(parentDirectory + "/files/.__override__");
                directoryCreated = true;
            }

            if (directoryCreated)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetMessage("First time initialization complete. Please re-start. Closing.")
                       .SetCancelable(false);

                builder.SetPositiveButton("Ok", (senderAlert, args) =>
                {
                    System.Environment.Exit(0);
                });

                builder.Create();
                builder.Show();
            }

            return parentDirectory + "/files/.__override__";
        }

        /// <summary>
        /// Load file from embedded resource and copy it to directory.
        /// </summary>
        /// <param name="directory">Directory to save file to.</param>
        /// <param name="fileName">File name to create.</param>
        private void CopyModuleDependencyFromEmbeddedResource(string directory, string fileName)
        {
            string fullPath = Path.Combine(directory, fileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Client_Mobile.Android.ModuleLibs." + fileName;
            Stream fontStream = assembly.GetManifestResourceStream(resourceName);

            var memoryStream = new MemoryStream();
            fontStream.CopyTo(memoryStream);

            File.WriteAllBytes(fullPath, memoryStream.ToArray());
        }

        /// <summary>
        /// Open CameraActivity.
        /// </summary>
        public void OpenCamera()
        {
            Intent intent = new Intent(this, typeof(CameraActivity));
            StartActivityForResult(intent, camera_id);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == camera_id)
            {
                if (resultCode == Result.Ok)
                {
                    // Picture has been taken...
                    int orientation = data.GetIntExtra("orientation", 0);
                    int width = data.GetIntExtra("width", 0);
                    int height = data.GetIntExtra("height", 0);

                    Action scrollAction = new Action(() =>
                    {
                        androidFunctions.InvokeCameraEvent(new JCIW.Data.Drawing.ImageSource());

                    });

                    view.PostDelayed(scrollAction, 100);

                }
                else
                {
                    androidFunctions.InvokeCameraEvent(null);
                }
            }
        }

        /// <summary>
        /// Convert DP to PX to retrieve screen pixel density.
        /// </summary>
        /// <param name="context">Activity context</param>
        /// <param name="dp">DP</param>
        /// <returns>Pixels.</returns>
        public static int dpToPx(Context context, int dp)
        {
            float density = context.Resources.DisplayMetrics.Density;

            return (int)Math.Round((float)dp * density);
        }

        protected override void OnResume()
        {
            androidFunctions.CloseKeyboard();

            base.OnResume();
        }

        protected override void OnDestroy()
        {
            androidFunctions.CloseKeyboard();

            base.OnDestroy();
        }
    }
}
