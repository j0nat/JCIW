using ImGuiNET;
using JCIW.Data.Drawing;
using Networking.Data;
using Networking.Data.Packets;
using Networking.Data.ResponseCodes;
using System;
using System.Net;

namespace JCIW.App.Views
{
    /// <summary>
    /// This class is used to draw and handle login.
    /// </summary>
    class LoginView
    {
        private readonly MainApp main;
        private string hostBuffer = "";
        private string usernameBuffer = "";
        private string passwordBuffer = "";
        private string infoText = "Please log in.";

        /// <summary>
        /// Creates <see cref="LoginView"/>.
        /// </summary>
        /// <param name="main"><see cref="MainApp"/></param>
        public LoginView(MainApp main)
        {
            this.main = main;

            string host;
            string username;

            LocalData.RetrieveLogin(out host, out username);

            if (username != null)
            {
                usernameBuffer = username;
            }


            if (host != null)
            {
                hostBuffer = host;

                IPAddress address = ParseHost(host);

                if (address != null)
                {
                    string localSessionId = LocalData.RetrieveSession();
                    if (localSessionId != null && localSessionId.Length != 0)
                    {
                        main.IgnoreDisconnect = true;

                        main.Networking.Connect(address);
                        if (main.Networking.ReConnect())
                        {
                            main.Networking.SessionVerificationResult += Networking_SessionVerificationResult;
                            main.Networking.Send(PacketName.RequestSessionVerification.ToString(), localSessionId);
                        }

                        main.IgnoreDisconnect = false;
                    }
                    else
                    {
                        // Unable to connect to server...
                    }
                }
            }
        }

        /// <summary>
        /// Try to parse host.
        /// </summary>
        /// <param name="ipAddrInput">Host</param>
        /// <returns><see cref="IPAddress"/></returns>
        private IPAddress ParseHost(string ipAddrInput)
        {
            IPAddress ipAddrOutput = null;

            if (!IPAddress.TryParse(ipAddrInput, out ipAddrOutput))
            {
                try
                {
                    IPHostEntry dnsInfo = Dns.GetHostEntry(hostBuffer);
                    ipAddrOutput = dnsInfo.AddressList[0];
                }
                catch
                {
                    // Neither IP parsing or DNS parsing worked.
                }
            }

            return ipAddrOutput;
        }

        private void Networking_SessionVerificationResult(object sender, EventArgs e)
        {
            main.Networking.SessionVerificationResult -= Networking_SessionVerificationResult;

            GenericResponse response = (GenericResponse)sender;

            if (response == GenericResponse.Success)
            {
                main.LoginSuccess();
            }
            else
            {
                LocalData.SaveSession("");
            }
        }

        private void LoginResultReceived(object sender, EventArgs e)
        {
            main.Networking.LoginEvent -= LoginResultReceived;

            LoginResponse loginResponse = (LoginResponse)sender;

            if (loginResponse == LoginResponse.Success)
            {
                LocalData.SaveLogin(hostBuffer, usernameBuffer);

                passwordBuffer = "";
                

                main.LoginSuccess();
            }
            else
            {
                infoText = "Wrong username / password.";
            }
        }

        /// <summary>
        /// Draw login view.
        /// </summary>
        public void Draw()
        {
            ImGui.Begin("Login", ImGuiWindowFlags.NoMove |
                 ImGuiWindowFlags.NoCollapse
                 | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.AlwaysAutoResize);

            if (main.Platform == Data.Platform.Android)
            {
                ImGui.PushFont((ImFontPtr)main.Frame.Fonts.SmallFont);
            }
            else
            if (main.Platform == Data.Platform.Desktop)
            {
                ImGui.PushFont((ImFontPtr)main.Frame.Fonts.NormalFont);
            }

                ImGui.Spacing();

            ImGui.TextUnformatted("Host: ");
            ImGui.PushItemWidth(200 * main.PlatformFunctions.ScreenDensity());


            ImGui.InputText("##host", ref hostBuffer, 100);


            ImGui.TextUnformatted("Username: ");
            ImGui.PushItemWidth(200 * main.PlatformFunctions.ScreenDensity());

            ImGui.InputText("##username", ref usernameBuffer, 100);


            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.1f, 0.1f, 0.1f, 1));

            ImGui.TextUnformatted("Password: ");


            ImGui.InputText("##password", ref passwordBuffer, 100, ImGuiInputTextFlags.Password);


            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.1f, 0.1f, 0.1f, 1));

            ImGui.SetCursorPosX(ImGui.GetWindowContentRegionWidth() - ImGui.CalcTextSize("Login ").X);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);

            int intColor = (36 << 16) | (36 << 8) | (36);
            ImGui.PushStyleColor(ImGuiCol.Button, (uint)intColor);

            if (ImGui.Button("Login")) Login();

            Vector2 windowSize = ImGui.GetWindowSize();
            Vector2 screenSize = new Vector2(main.Frame.Width, main.Frame.Height);
            Vector2 loginWindowPosition = new Vector2(
                (screenSize.X / 2) - (windowSize.X / 2),
                (screenSize.Y / 2) - (windowSize.Y / 2));

            if (main.Platform == Data.Platform.Android)
            {
                 loginWindowPosition = new Vector2(
                    (screenSize.X / 2) - (windowSize.X / 2),
                    (screenSize.Y / 3) - (windowSize.Y / 2));
            }

            ImGui.SetWindowPos(loginWindowPosition);

          //  ImGui.PopFont();
            ImGui.End();

            ImGui.Begin("Info", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar
                | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.AlwaysAutoResize);

         //   ImGui.PushFont((ImFontPtr)main.Frame.Fonts.NormalFont);
            ImGui.TextUnformatted(infoText);
            windowSize = ImGui.GetWindowSize();
            screenSize = new Vector2(main.Frame.Width, main.Frame.Height);

            ImGui.SetWindowPos(new Vector2((screenSize.X / 2) - (windowSize.X / 2),
                loginWindowPosition.Y - (int)windowSize.Y - 10));
            ImGui.PopFont();
            ImGui.PopStyleColor();
            ImGui.End();
        }

        /// <summary>
        /// Send login request to server.
        /// </summary>
        public void Login()
        {
            IPAddress host = ParseHost(hostBuffer);

            if (host != null)
            {
                main.IgnoreDisconnect = true;
                main.Networking.Connect(host);

                if (main.Networking.ReConnect())
                {
                    if (usernameBuffer.Length > 1 && passwordBuffer.Length > 1)
                    {
                        main.Networking.SessionReceived += Networking_SessionReceived;
                        main.Networking.LoginEvent += LoginResultReceived;

                        main.Networking.Send(PacketName.RequestLogin.ToString(),
                            new LoginRequest(usernameBuffer, passwordBuffer, false));
                    }
                    else
                    {
                        infoText = "Login too short.";
                    }
                }
                else
                {
                    infoText = "Unable to connect.";
                }

                main.IgnoreDisconnect = false;
            }
            else
            {
                infoText = "Invalid host.";
            }
        }

        private void Networking_SessionReceived(object sender, EventArgs e)
        {
            main.Networking.SessionReceived -= Networking_SessionReceived;

            string sessionId = sender.ToString();

            LocalData.SaveSession(sessionId);
        }
    }
}
