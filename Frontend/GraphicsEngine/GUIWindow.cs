using GraphicsEngine.ImGuiMonoGame;
using ImGuiNET;
using JCIW.Data;
using JCIW.Data.Drawing;
using JCIW.Data.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace GraphicsEngine
{
    /// <summary>
    /// The <see cref="Game"/> used by JCIW to draw graphics to screen.
    /// </summary>
    public class GUIWindow : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private readonly IFrame frame;
        private readonly IApp app; // this is mainApp
        private readonly Platform platform;
        private readonly IPlatformFunctions platformFunctions;
        private readonly List<ImageSource> imageLoadQueue;
        private readonly Fonts fonts;
        private ImGuiRenderer imGuiRenderer;
        private bool androidKeyboardOpen;
        private readonly GUIEngine guiEngine;
        private Color backgroundColor;

        /// <summary>
        /// Create <see cref="GUIWindow"/>.
        /// </summary>
        /// <param name="platform">The <see cref="Platform"/>.</param>
        /// <param name="platformFunctions">A <see cref="IPlatformFunctions"/> implementation.</param>
        /// <param name="frame">A <see cref="IFrame"/> implementaiton.</param>
        /// <param name="app">The <see cref="IApp"/> implementation.</param>
        /// <param name="guiEngine">The <see cref="GUIEngine"/>.</param>
        public GUIWindow(Platform platform, IPlatformFunctions platformFunctions, IFrame frame, IApp app, GUIEngine guiEngine)
        {
            Runtime.PLATFORM = platform;

            this.imageLoadQueue = new List<ImageSource>();
            this.frame = frame;
            this.app = app;
            this.platform = platform;
            this.androidKeyboardOpen = false;
            this.platformFunctions = platformFunctions;
            this.fonts = new Fonts();
            this.guiEngine = guiEngine;
            this.backgroundColor = new Color(36, 36, 36);

            TargetElapsedTime = TimeSpan.FromSeconds(1d / 30d); // 30 FPS cap limit

            graphics = new GraphicsDeviceManager(this)
            {
                SupportedOrientations = DisplayOrientation.Portrait,
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8,
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferMultiSampling = true,
                SynchronizeWithVerticalRetrace = false,
            };

            this.IsMouseVisible = true;
           

            Content.RootDirectory = "Content";

            if (platform == Platform.Android)
            {
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;

                graphics.IsFullScreen = true;

                graphics.ApplyChanges();
            }
            else
            {
                if (platform == Platform.Desktop)
                {
                    graphics.PreferredBackBufferWidth = 800;
                    graphics.PreferredBackBufferHeight = 600;
                }
            }
            
            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

            frame.SetScreenWidth(GraphicsDevice.Viewport.Width);
            frame.SetScreenHeight(GraphicsDevice.Viewport.Height);
        }

        /// <summary>
        /// Retrieve the loaded fonts.
        /// </summary>
        /// <returns>The <see cref="Fonts"/>.</returns>
        public Fonts GetFonts()
        {
            return fonts;
        }

        /// <summary>
        /// Sets a new backgrund color.
        /// </summary>
        /// <param name="color">The background <see cref="Color"/>.</param>
        public void SetBackgroundColor(Color color)
        {
            this.backgroundColor = color;
        }

        /// <summary>
        /// Load texture from byte array and add it to queue to be loaded by ImGui outside the draw.
        /// </summary>
        /// <param name="imageSource">The <see cref="ImageSource"/>.</param>
        public void LoadTexture(ImageSource imageSource)
        {
            Texture2D texture = LoadTexture(imageSource.Image);
            imageSource.ImageSize = new JCIW.Data.Drawing.Vector2(texture.Width, texture.Height);
            imageSource.MGTexture = texture;

            lock (imageLoadQueue)
            {
                imageLoadQueue.Add(imageSource);
            }
        }

        /// <summary>
        /// Sets a new render target and loads the texture. This seems to help with avoiding blinking or corruption from
        /// ImGui.
        /// </summary>
        /// <param name="imageData">Image data as byte array.</param>
        /// <returns>The <see cref="Texture2D"/>.</returns>
        private Texture2D LoadTexture(byte[] imageData)
        {
            GraphicsDevice.SetRenderTarget(new RenderTarget2D(GraphicsDevice, 1, 1));

            var imageStream = new MemoryStream(imageData);
            Texture2D texture = Texture2D.FromStream(GraphicsDevice, imageStream);

            imageStream.Close();
            imageStream.Dispose();

            GraphicsDevice.SetRenderTarget(null);
            return texture;
        }

        /// <summary>
        /// Unloads the texture from ImGui.
        /// </summary>
        /// <param name="texture">The texture <see cref="IntPtr"/>.</param>
        public void UnloadTexture(IntPtr texture)
        {
            imGuiRenderer.UnbindTexture(texture);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            imGuiRenderer = new ImGuiRenderer(this, platform, platformFunctions);

            if (platform == Platform.Android)
            {
                AndroidCopyToDisk("OpenSans-Regular.ttf");
            }

            fonts.NormalFont = ImGui.GetIO().Fonts.AddFontFromFileTTF(
                PlatformContentPath("OpenSans-Regular.ttf"), 32 * platformFunctions.ScreenDensity());

            fonts.LargeFont = ImGui.GetIO().Fonts.AddFontFromFileTTF(
                PlatformContentPath("OpenSans-Regular.ttf"), 48 * platformFunctions.ScreenDensity());

            fonts.SmallFont = ImGui.GetIO().Fonts.AddFontFromFileTTF(
                PlatformContentPath("OpenSans-Regular.ttf"), 24 * platformFunctions.ScreenDensity());

            imGuiRenderer.RebuildFontAtlas();

            frame.SetScreenWidth(GraphicsDevice.Viewport.Width);
            frame.SetScreenHeight(GraphicsDevice.Viewport.Height);

            base.Initialize();
        }

        /// <summary>
        /// Desktop and Android have different content folders. This is used to retrieve the content path.
        /// </summary>
        /// <param name="contentName">The name of the file to append to the content path.</param>
        /// <returns>The full path to the content folder + file name.</returns>
        private string PlatformContentPath(string contentName)
        {
            string path = "";

            if (platform == Platform.Android)
            {
                string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string contentFolder = Path.Combine(appFolder, "Content");
                path = Path.Combine(contentFolder, contentName);
            }
            else
            {
                if (platform == Platform.Desktop)
                {
                    string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                    string contentFolder = Path.Combine(appFolder, "Content");
                    path = Path.Combine(contentFolder, contentName);
                }
            }

            return path;
        }

        /// <summary>
        /// Used to Android to copy files to the content folder. Neccessary for loading fonts.
        /// </summary>
        /// <param name="fontName">The name of the embedded resource name.</param>
        private void AndroidCopyToDisk(string fontName)
        {
            string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string contentFolder = Path.Combine(appFolder, "Content");
            string filePath = Path.Combine(contentFolder, fontName);

            if (!Directory.Exists(contentFolder))
            {
                Directory.CreateDirectory(contentFolder);
            }

            if (!File.Exists(filePath))
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = "GraphicsEngine.Content." + fontName;

                Stream fontStream = assembly.GetManifestResourceStream(resourceName);


                var memoryStream = new MemoryStream();
                fontStream.CopyTo(memoryStream);

                File.WriteAllBytes(filePath, memoryStream.ToArray());
            }
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            guiEngine.UpdateGameComponent(gameTime.ElapsedGameTime);
            frame.Update();

            if (platform == Platform.Android)
            {
                if (ImGui.KeyboardWanted() == 1)
                {
                    if (!androidKeyboardOpen)
                    {
                        // Open keyboard
                        platformFunctions.OpenKeyboard();

                        androidKeyboardOpen = true;
                    }
                }
                else
                {
                    if (androidKeyboardOpen)
                    {
                        platformFunctions.CloseKeyboard();

                        androidKeyboardOpen = false;
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);
            guiEngine.DrawGameComponentBeforeGUI();

            // TODO: Add your drawing code here
            imGuiRenderer.BeforeLayout(gameTime);

            app.Draw();

            imGuiRenderer.AfterLayout();

            spriteBatch.Begin();
            if (imageLoadQueue.Count != 0)
            {
                List<ImageSource> removeFromQueue = new List<ImageSource>();

                lock (imageLoadQueue)
                {
                    foreach (ImageSource image in imageLoadQueue)
                    {
                        image.Texture = imGuiRenderer.BindTexture((Texture2D)image.MGTexture);

                        removeFromQueue.Add(image);
                    }

                    foreach (ImageSource image in removeFromQueue)
                    {
                        imageLoadQueue.Remove(image);
                    }
                }
            }
            spriteBatch.End();

            guiEngine.DrawGameComponentAfterGUI();

            base.Draw(gameTime);
        }
    }
}
