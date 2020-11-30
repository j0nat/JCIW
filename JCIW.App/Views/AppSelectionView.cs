using ImGuiNET;
using JCIW.Data;
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
        private float buttonRealWidth;

        /// <summary>
        /// Creates <see cref="AppSelectionView"/>.
        /// </summary>
        /// <param name="main"><see cref="MainApp"/></param>
        public AppSelectionView(MainApp main)
        {
            this.main = main;
            this.buttonRealWidth = 0;
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
        public void Draw(float overlayHeight)
        {
            if (main.Platform == Platform.Android)
            {
                ImGui.GetStyle().ScrollbarSize = 60;
            }

            float windowHeight = 0;
            float windowWidth = 0;
            ImGui.Begin("AppList", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize);

            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 1, 1));
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1, 1, 1, 1));
            ImGui.PushFont((ImFontPtr)main.Frame.Fonts.SmallFont);

            if (apps != null)
            {
                ImGui.TextUnformatted("Available apps:");

                float headerSizeWidth = ImGui.CalcTextSize("Available apps:").X;

                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0, 0, 0, 1));
                ImGui.SetCursorPosY(ImGui.CalcTextSize("UNUSED").Y * 2);

                windowHeight += ImGui.CalcTextSize("Available apps:").Y * 3;

                windowWidth = headerSizeWidth;

                lock (this.apps)
                {
                    foreach (ModuleInfo app in apps)
                    {
                        string text = app.Name + " (v" + app.Version + ")";
                        Vector2 buttonSize = ImGui.CalcTextSize(text) * 1.2f;
                        Vector2 buttonPadding = new Vector2(16, 20 + ImGui.CalcTextSize("H").Y * 1);

                        windowHeight += buttonSize.Y + buttonPadding.Y;

                        float buttonWidth = buttonSize.X + buttonPadding.X;

                        if (buttonRealWidth < buttonWidth)
                        {
                            buttonRealWidth = buttonWidth;
                        }

                        if (buttonWidth > windowWidth)
                        {
                            windowWidth = buttonWidth;
                        }

                        if (ImGui.Button(text, new Vector2(buttonRealWidth, buttonSize.Y + buttonPadding.Y))) main.AppClicked(app);
                    }
                }
            }
            else
            {
                ImGui.TextUnformatted("No applications available.");

                windowWidth = ImGui.CalcTextSize("No applications available.").X;
            }

            Vector2 windowSize = new Vector2(windowWidth, windowHeight);
            windowWidth += ImGui.GetStyle().ScrollbarSize * 2;


            Vector2 screenSize = new Vector2(main.Frame.Width, main.Frame.Height - overlayHeight);

            if (windowHeight >= (main.Frame.Height - (overlayHeight)))
            {
                ImGui.SetWindowPos(new Vector2(
                    (screenSize.X / 2) - (windowSize.X / 2),
                    overlayHeight));

                ImGui.SetWindowSize(new Vector2(windowWidth, (main.Frame.Height - (overlayHeight * 1.2f))));
            }
            else
            {
                ImGui.SetWindowSize(new Vector2(windowWidth, windowHeight * 1.2f));

                float newWindowPos = (screenSize.Y / 2) - (windowSize.Y / 2);

                if (newWindowPos <= overlayHeight)
                {
                    ImGui.SetWindowPos(new Vector2(
                        (screenSize.X / 2) - (windowSize.X / 2),
                        overlayHeight));
                }
                else
                {
                    ImGui.SetWindowPos(new Vector2(
                        (screenSize.X / 2) - (windowSize.X / 2),
                        (screenSize.Y / 2) - (windowSize.Y / 2)));
                }
            }

            ImGui.PopStyleColor();
            ImGui.PopFont();
            ImGui.End();
        }
    }
}
