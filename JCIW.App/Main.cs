using JCIW.Data;
using JCIW.Data.Interfaces;
using JCIW.Module;
using Networking.Data;
using Networking.Data.Packets;
using System;
using System.Collections.Generic;
using System.IO;

namespace JCIW.App
{
    /// <summary>
    /// This class is used to draw the login and app selection views and overlays.
    /// </summary>
    public class Main : IApp
    {
        public List<string> UserGroups { get; set; }
        private Dictionary<long, ModuleInstance> apps { get; set; }

        public readonly IAppNetworking Networking;
        public readonly IPlatformFunctions PlatformFunctions;
        public readonly Platform Platform;
        public readonly Frame Frame;
        private ModuleInstance currentApp;

        public string CurrentlyRunningAppName { get; set; }

        /// <summary>
        /// Creates implementation of <see cref="IApp"/>.
        /// </summary>
        /// <param name="networking">A <see cref="IAppNetworking"/> implementation.</param>
        /// <param name="platformFunctions">A <see cref="IPlatformFunctions"/> implementation.</param>
        /// <param name="platform">The <see cref="Platform"/>.</param>
        public Main(IAppNetworking networking, IPlatformFunctions platformFunctions, Platform platform)
        {
            this.CurrentlyRunningAppName = "";
            this.currentApp = null;
            this.UserGroups = new List<string>();
            this.Networking = networking;
            this.PlatformFunctions = platformFunctions;
            this.Platform = platform;
            this.apps = new Dictionary<long, ModuleInstance>();
            this.Frame = new Frame(platform, platformFunctions, this);

            if (platform == Platform.Desktop)
            {
                Local.Folder = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                if (platform == Platform.Android)
                {
                    Local.Folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                }
            }
        }

        /// <summary>
        /// Get the MonoGame active game.
        /// </summary>
        /// <returns>MonoGame Game object.</returns>
        public object Window()
        {
            return Frame.Window();
        }

        /// <summary>
        /// Check if app is valid and load.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="filePath">Path on file system.</param>
        private void LoadApp(long appId, string filePath)
        {
            FileInfo file = new FileInfo(Path.Combine(Local.Folder, filePath));
            ModuleHeader moduleHeader = ModuleHeaderReader.AppHeader(Local.Folder, file.FullName);

            if (moduleHeader != null)
            {
                if (apps.ContainsKey(appId))
                {
                    ModuleInstance app = apps[appId];

                    app.Load();

                    currentApp = app;
                }
                else
                {
                    ModuleInstance app = new ModuleInstance(file.FullName, "Module.App",
                        new object[] { Frame, Networking, PlatformFunctions, Platform, UserGroups.ToArray() });

                    bool result = app.Load();

                    apps.Add(appId, app);

                    currentApp = app;
                }

                CurrentlyRunningAppName = moduleHeader.Name;
            }
        }

        /// <summary>
        /// Prepare app from file system. If not present then request download from server.
        /// </summary>
        /// <param name="appInfo"></param>
        public void PrepareAppFile(ModuleInfo appInfo)
        {
            string folder = Path.Combine(Local.Folder, "plugins");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            if (File.Exists(Path.Combine(Local.Folder, appInfo.Path)))
            {
                // File already exists!

                // Load!
                LoadApp(appInfo.Id, appInfo.Path);

            }
            else
            {
                // File needs to be downloaded
                Networking.Send(PacketName.RequestAppFile.ToString(), appInfo.Id);

                Networking.AppFileEvent += Networking_AppFileEvent;
            }
        }

        private void Networking_AppFileEvent(object sender, EventArgs e)
        {
            AppFileData fileData = (AppFileData)sender;

            File.WriteAllBytes(Path.Combine(Local.Folder, fileData.path), fileData.data);

            LoadApp(fileData.id, fileData.path);
        }

        /// <summary>
        /// Draw to screen.
        /// </summary>
        public virtual void Draw()
        {

        }

        /// <summary>
        /// Close and unload app.
        /// </summary>
        public void CloseApp()
        {
            if (currentApp != null)
            {
                currentApp.Unload();
                currentApp = null;
            }
        }

        /// <summary>
        /// Call draw code from current app.
        /// </summary>
        public void DrawApp()
        {
            if (currentApp != null)
            {
                currentApp.CallDraw();
            }
        }
    }
}
