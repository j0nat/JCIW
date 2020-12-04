using JCIW.Data.Drawing;
using Networking.Data;
using Networking.Data.Packets;
using Networking.Data.ResponseCodes;
using System;
using System.Threading.Tasks;
using JCIW.Data.Interfaces;
using JCIW.Data;
using ImGuiNET;
using JCIW.App.Views;

namespace JCIW.App
{
    /// <summary>
    /// This class is used to draw the login and app selection views and overlays.
    /// </summary>
    public class MainApp : Main
    {
        public bool IgnoreDisconnect { get; set; }
        private bool drawLogin;
        private bool drawAppSelectionView;
        private bool drawApp;
        private bool drawNetworkDisconnected;
        private bool waitingForAuthorization;
        private bool runningConnectCheck;
        private readonly LoginView loginView;
        private readonly AppSelectionView appSelectionView;
        private float downScaleNetworkDisconnectedWindow;

        /// <summary>
        /// Create <see cref="MainApp"/>.
        /// </summary>
        /// <param name="networking">A <see cref="IAppNetworking"/> implementation.</param>
        /// <param name="platformFunctions">A <see cref="IPlatformFunctions"/> implementation.</param>
        /// <param name="platform">The <see cref="Platform"/>.</param>
        public MainApp(IAppNetworking networking, IPlatformFunctions platformFunctions, Platform platform) : 
            base(networking, platformFunctions, platform)
        {
            this.drawLogin = true;
            this.drawAppSelectionView = false;
            this.waitingForAuthorization = false;
            this.appSelectionView = new AppSelectionView(this);
            this.loginView = new LoginView(this);
            this.runningConnectCheck = false;
            this.IgnoreDisconnect = false;

            this.downScaleNetworkDisconnectedWindow = 0;

            this.Networking.GroupList += Networking_GroupList;
            this.Networking.NetworkDisconnect += Networking_NetworkDisconnect;
            this.Networking.UnAuthorized += Networking_UnAuthorized;
        }

        private void Networking_UnAuthorized(object sender, EventArgs e)
        {
            if (!waitingForAuthorization && !drawLogin)
            {
                string localSessionId = LocalData.RetrieveSession();
                if (localSessionId != null && localSessionId.Length != 0)
                {
                    waitingForAuthorization = true;

                    Networking.SessionVerificationResult += Networking_SessionVerificationResult;
                    Networking.Send(PacketName.RequestSessionVerification.ToString(), localSessionId);
                }
                else
                {
                    // User has no session.
                    LocalData.SaveSession("");
                    this.drawLogin = true;
                    this.drawAppSelectionView = false;
                    this.drawApp = false;

                    waitingForAuthorization = false;
                    Frame.SetBackgroundColor(36, 36, 36);
                }
            }
        }

        private void Networking_NetworkDisconnect(object sender, EventArgs e)
        {
            if (!IgnoreDisconnect)
            {
                if (!drawNetworkDisconnected)
                {
                    this.drawNetworkDisconnected = true;
                }
            }
        }

        /// <summary>
        /// Called when user has logged in.
        /// Changes the view.
        /// </summary>
        public void LoginSuccess()
        {
            Networking.Send(PacketName.RequestUserAppList.ToString(), -1);

            drawApp = false;
            drawLogin = false;
            drawAppSelectionView = true;

            appSelectionView.RequestApps();
        }


        /// <summary>
        /// App in AppSelectionView clicked.
        /// </summary>
        /// <param name="app">App <see cref="ModuleInfo"/>.</param>
        public void AppClicked(ModuleInfo app)
        {
            drawLogin = false;
            drawAppSelectionView = false;

            PrepareAppFile(app);

            drawApp = true;
        }

        private void Networking_GroupList(object sender, EventArgs e)
        {
            UserGroups.Clear();

            Group[] groups = (Group[])sender;

            if (groups != null)
            {
                foreach (Group group in groups)
                {
                    UserGroups.Add(group.name);
                }
            }
        }

        /// <summary>
        ///  This method is used to draw the GUI.
        /// </summary>
        public override void Draw()
        {
            ImGui.GetStyle().WindowBorderSize = 0;

            if (drawNetworkDisconnected)
            {
                DrawConnectionErrorOverlay();
            }
            
            if (drawLogin)
            {
                loginView.Draw();
                DrawLoginOverlay();
            }

            if (drawAppSelectionView)
            {
                float overlayHeight = DrawMainOverlay();

                appSelectionView.Draw(overlayHeight);
            }

            if (drawApp)
            {
                float overlayHeight = DrawpAppOverlay();

                ImGui.SetNextWindowPos(new Vector2(ImGui.GetWindowPos().X, overlayHeight + 10 * PlatformFunctions.ScreenDensity()), ImGuiCond.Once);

                DrawApp();
            }
        }

        /// <summary>
        /// Called when logout is clicked.
        /// </summary>
        private void LogOut()
        {
            LocalData.SaveSession("");
            drawApp = false;
            drawLogin = true;
            drawAppSelectionView = false;
        }

        /// <summary>
        /// Called when app is closed by the user.
        /// </summary>
        private void ClickCloseApp()
        {
            drawApp = false;
            drawLogin = false;
            drawAppSelectionView = true;

            CloseApp();

            Frame.SetBackgroundColor(36, 36, 36);
        }

        /// <summary>
        /// Draw connection error overlay. Retry connection and close.
        /// </summary>
        private void DrawConnectionErrorOverlay()
        {
            bool unused_open = true;
            ImGui.OpenPopup("NetworkDisconnected");
            if (ImGui.BeginPopupModal("NetworkDisconnected", ref unused_open, ImGuiWindowFlags.NoMove |
                 ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.PushFont((ImFontPtr)Frame.Fonts.NormalFont);

                if (Platform == Platform.Android)
                {
                    ImGui.SetWindowFontScale(1 - downScaleNetworkDisconnectedWindow);
                }

                ImGui.TextUnformatted("Lost connection to server.");

                float buttonPositionX = ImGui.CalcTextSize("Lost connection to server.").X;
                ImGui.SetCursorPosX(buttonPositionX);
                ImGui.SetCursorPosY(ImGui.CalcTextSize("UNUSED").Y * 2);

                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 1, 1));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0, 0, 0, 1));

                Vector2 buttonPadding = new Vector2(14, 20);
                if (Platform == Platform.Android)
                {
                    buttonPadding = new Vector2(14, 60);
                }

                if (runningConnectCheck)
                {
                    ImGui.Button("Retrying Connection... ", ImGui.CalcTextSize("Retrying Connection... ") + buttonPadding);
                }
                else
                {
                    if (ImGui.Button("Retry Connection", ImGui.CalcTextSize("Retrying Connection... ") + buttonPadding))
                    {
                        Task.Factory.StartNew(() =>
                        {
                            runningConnectCheck = true;

                            if (Networking.ReConnect())
                            {
                                string localSessionId = LocalData.RetrieveSession();
                                if (localSessionId != null && localSessionId.Length != 0)
                                {
                                    Networking.SessionVerificationResult += Networking_SessionVerificationResult;
                                    Networking.Send(PacketName.RequestSessionVerification.ToString(), localSessionId);
                                }
                                else
                                {
                                    // User is not logged in.
                                    drawNetworkDisconnected = false;
                                }
                            }

                            runningConnectCheck = false;
                        });
                    }
                }

                ImGui.SetCursorPosX(buttonPositionX);
                
                if (ImGui.Button("Close app", ImGui.CalcTextSize("Retrying Connection... ") + buttonPadding))
                {
                    PlatformFunctions.Exit();
                }

                Vector2 windowSize = ImGui.GetWindowSize();
                Vector2 screenSize = new Vector2(Frame.Width, Frame.Height);
                Vector2 loginWindowPosition = new Vector2((screenSize.X / 2) - (windowSize.X / 2),
                    (screenSize.Y / 2) - (windowSize.Y / 2));
                
                if ((ImGui.GetWindowWidth() + (15 * PlatformFunctions.ScreenDensity())) > (screenSize.X))
                {
                    downScaleNetworkDisconnectedWindow += 0.05f;
                }

                ImGui.SetWindowPos(loginWindowPosition);

                ImGui.PopStyleColor();
                ImGui.PopFont();
                ImGui.EndPopup();
            }
        }

        private void Networking_SessionVerificationResult(object sender, EventArgs e)
        {
            Networking.SessionVerificationResult -= Networking_SessionVerificationResult;

            GenericResponse response = (GenericResponse)sender;
            if (response != GenericResponse.Success)
            {
                CloseApp();

                LocalData.SaveSession("");
                this.drawLogin = true;
                this.drawAppSelectionView = false;
                this.drawApp = false;
                Frame.SetBackgroundColor(36, 36, 36);
            }

            drawNetworkDisconnected = false;
        }

        /// <summary>
        /// Draw main overlay buttons.
        /// </summary>
        private float DrawMainOverlay()
        {
            float overlayHeight = 0;

            ImGui.Begin("MainOverlay", ImGuiWindowFlags.NoMove |
                 ImGuiWindowFlags.NoCollapse
                 | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoResize);

            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.1f, 0.1f, 0.1f, 1));

            int intColor = (36 << 16) | (36 << 8) | (36);
            ImGui.PushStyleColor(ImGuiCol.Button, (uint)intColor);
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1, 1, 1, 1));

            ImGui.PushFont((ImFontPtr)Frame.Fonts.NormalFont);
            Vector2 CloseButtonSize = ImGui.CalcTextSize("X");

            if (Platform == Platform.Android)
            {
                ImGui.SetCursorPosX(30);
            }

            if (ImGui.Button("Log Out")) LogOut();

            ImGui.PopFont();
            ImGui.PopStyleColor();

            ImGui.SameLine();

            ImGui.SetWindowSize(new Vector2(Frame.Width, (CloseButtonSize.Y) + ImGui.GetFrameHeightWithSpacing()));
            ImGui.SetWindowPos(new Vector2(0, 0));

            overlayHeight = ImGui.GetWindowHeight();

            ImGui.End();

            return overlayHeight;
        }

        /// <summary>
        /// Draw app overlay buttons.
        /// </summary>
        private float DrawpAppOverlay()
        {
            float overlayHeight = 0;

            ImGui.Begin("AppOverlay", ImGuiWindowFlags.NoMove |
                 ImGuiWindowFlags.NoCollapse
                 | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoResize);

            int intColor = (36 << 16) | (36 << 8) | (36);
            ImGui.PushStyleColor(ImGuiCol.Button, (uint)intColor);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, 
                new Vector4(
                    0.07f,
                    0.07f,
                    0.07f, 0.7f));
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1, 1, 1, 1));
            ImGui.PushFont((ImFontPtr)Frame.Fonts.NormalFont);
            Vector2 CloseButtonSize = ImGui.CalcTextSize("X");

            if (Platform == Platform.Android)
            {
                ImGui.SetCursorPosX(30);
            }

            if (ImGui.Button("Close")) ClickCloseApp();

            ImGui.PopFont();

            ImGui.SameLine();

            ImGui.SetWindowSize(new Vector2(Frame.Width, (CloseButtonSize.Y) + ImGui.GetFrameHeightWithSpacing()));
            ImGui.SetWindowPos(new Vector2(0, 0));

            ImGui.PushFont((ImFontPtr)Frame.Fonts.SmallFont);
            Vector2 headerSize = ImGui.CalcTextSize(CurrentlyRunningAppName);
            ImGui.SetCursorPosX((Frame.Width / 2) - (headerSize.X / 2));

            ImGui.TextUnformatted(CurrentlyRunningAppName);

            ImGui.PopStyleColor();
            ImGui.PopFont();

            overlayHeight = ImGui.GetWindowHeight();

            ImGui.End();

            return overlayHeight;
        }

        /// <summary>
        /// Draw login overlay buttons.
        /// </summary>
        private void DrawLoginOverlay()
        {
            ImGui.Begin("LoginOverlay", ImGuiWindowFlags.NoMove |
                 ImGuiWindowFlags.NoCollapse
                 | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoResize);

            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.1f, 0.1f, 0.1f, 1));
            int intColor = (36 << 16) | (36 << 8) | (36);
            ImGui.PushStyleColor(ImGuiCol.Button, (uint)intColor);
            ImGui.PushFont((ImFontPtr)Frame.Fonts.NormalFont);
            Vector2 CloseButtonSize = ImGui.CalcTextSize("X");

            ImGui.PopFont();

            ImGui.SameLine();

            ImGui.SetWindowSize(new Vector2(Frame.Width, (CloseButtonSize.Y) + ImGui.GetFrameHeightWithSpacing()));
            ImGui.SetWindowPos(new Vector2(0, 0));

            ImGui.PushFont((ImFontPtr)Frame.Fonts.SmallFont);
            Vector2 headerSize = ImGui.CalcTextSize("JCIW Login");
            ImGui.SetCursorPosX((Frame.Width / 2) - (headerSize.X / 2));

            ImGui.TextUnformatted("JCIW Login");

            ImGui.PopStyleColor();
            ImGui.PopFont();

            ImGui.End();
        }
    }
}
