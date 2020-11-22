using ImGuiNET;
using JCIW.Data.Drawing;
using Networking.Data;
using Networking.Data.Packets;
using System;

namespace JCIW.App.Views
{
    /// <summary>
    /// This class is used to draw and handle app selection.
    /// </summary>
    class AppSelectionView
    {
        private readonly MainApp main;
        private ModuleInfo[] apps;

        /// <summary>
        /// Creates <see cref="AppSelectionView"/>.
        /// </summary>
        /// <param name="main"><see cref="MainApp"/></param>
        public AppSelectionView(MainApp main)
        {
            this.main = main;
        }

        /// <summary>
        /// Request app list request from server.
        /// </summary>
        public void RequestApps()
        {
            main.Networking.ModuleListEvent += ReceivedModuleList;

            main.Networking.Send(PacketName.RequestUserGroupList.ToString(), -1);
        }

        private void ReceivedModuleList(object sender, EventArgs e)
        {
            main.Networking.ModuleListEvent -= ReceivedModuleList;

            ModuleInfo[] apps = (ModuleInfo[])sender;

            if (apps != null)
            {
                this.apps = apps;
            }
        }

        /// <summary>
        /// Draw the app list view.
        /// </summary>
        public void Draw()
        {
            ImGui.Begin("AppList", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize);

            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 1, 1));
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1, 1, 1, 1));
            ImGui.PushFont((ImFontPtr)main.Frame.Fonts.NormalFont);

            if (apps != null)
            {
                ImGui.TextUnformatted("Available apps:");

                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0, 0, 0, 1));
                ImGui.SetCursorPosY(ImGui.CalcTextSize("UNUSED").Y * 2);

                lock (this.apps)
                {
                    foreach (ModuleInfo app in apps)
                    {
                        string text = app.Name + " (" + app.Version + ")";
                        Vector2 buttonSize = ImGui.CalcTextSize(text);
                        Vector2 buttonPadding = new Vector2(16, 20 + ImGui.CalcTextSize("H").Y * 1);

                        if (ImGui.Button(text, buttonSize + buttonPadding)) main.AppClicked(app);
                    }
                }
            }
            else
            {
                ImGui.TextUnformatted("No applications available.");
            }

            Vector2 windowSize = ImGui.GetWindowSize();
            Vector2 screenSize = new Vector2(main.Frame.Width, main.Frame.Height);

            ImGui.SetWindowPos(new Vector2(
                (screenSize.X / 2) - (windowSize.X / 2),
                (screenSize.Y / 3) - (windowSize.Y / 2)));

            ImGui.PopStyleColor();
            ImGui.PopFont();
            ImGui.End();
        }
    }
}
