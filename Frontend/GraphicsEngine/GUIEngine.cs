using System;
using System.Collections.Generic;
using JCIW.Data;
using JCIW.Data.Drawing;
using JCIW.Data.Interfaces;
using Microsoft.Xna.Framework;

namespace GraphicsEngine
{
    /// <summary>
    /// This class is used to load the main <see cref="Game"/> and make changes to it.
    /// </summary>
    public class GUIEngine
    {
        public GUIWindow Window { get; private set; }

        private readonly List<IJCIWGameComponent> gameComponentsBefore;
        private readonly List<IJCIWGameComponent> gameComponentsAfter;

        /// <summary>
        /// Creates  <see cref="GUIEngine"/>.
        /// </summary>
        /// <param name="platform">The <see cref="Platform"/>.</param>
        /// <param name="platformFunctions">A <see cref="IPlatformFunctions"/> implementation.</param>
        /// <param name="frame">A <see cref="IFrame"/> implementaiton.</param>
        /// <param name="app">The <see cref="IApp"/> implementation.</param>
        public GUIEngine(Platform platform, IPlatformFunctions platformFunctions, IFrame frame, IApp app)
        {
            Window = new GUIWindow(platform, platformFunctions, frame, app, this);

            gameComponentsBefore = new List<IJCIWGameComponent>();
            gameComponentsAfter = new List<IJCIWGameComponent>();
        }

        /// <summary>
        /// Run the <see cref="GUIWindow"/>.
        /// </summary>
        public void Run()
        {
            Window.Run();
        }

        /// <summary>
        /// Set the background color.
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        public void SetBackgroundColor(int r, int g, int b)
        {
            Window.SetBackgroundColor(new Color(r, g, b));
        }

        /// <summary>
        /// Draw <see cref="IJCIWGameComponent"/> from gameComponentsBefore.
        /// </summary>
        public void DrawGameComponentBeforeGUI()
        { 
            lock (gameComponentsBefore)
            {
                foreach (IJCIWGameComponent gameComponent in gameComponentsBefore)
                {
                    gameComponent.Draw();
                }
            }
        }

        /// <summary>
        /// Draw <see cref="IJCIWGameComponent"/> from gameComponentsAfter.
        /// </summary>
        public void DrawGameComponentAfterGUI()
        {
            lock (gameComponentsAfter)
            {
                foreach (IJCIWGameComponent gameComponent in gameComponentsAfter)
                {
                    gameComponent.Draw();
                }
            }
        }

        /// <summary>
        /// Call <see cref="IJCIWGameComponent"/> updates.
        /// </summary>
        /// <param name="timeSpan">The <see cref="TimeSpan"/>.</param>
        public void UpdateGameComponent(TimeSpan timeSpan)
        {
            lock (gameComponentsAfter)
            {
                foreach (IJCIWGameComponent gameComponent in gameComponentsAfter)
                {
                    gameComponent.Update(timeSpan);
                }
            }

            lock (gameComponentsBefore)
            {
                foreach (IJCIWGameComponent gameComponent in gameComponentsBefore)
                {
                    gameComponent.Update(timeSpan);
                }
            }
        }

        /// <summary>
        /// Add new <see cref="IJCIWGameComponent"/>.
        /// </summary>
        /// <param name="gameComponent">The <see cref="IJCIWGameComponent"/>.</param>
        /// <param name="drawOrder">The <see cref="DrawOrder"/>.</param>
        public void AddGameComponent(IJCIWGameComponent gameComponent, DrawOrder drawOrder)
        {
            if (drawOrder == DrawOrder.AfterGUI)
            {
                lock (gameComponentsAfter)
                {
                    gameComponentsAfter.Add(gameComponent);
                }
            }
            else
            {
                if (drawOrder == DrawOrder.BeforeGUI)
                {
                    lock (gameComponentsBefore)
                    {
                        gameComponentsBefore.Add(gameComponent);
                    }
                }
            }

            gameComponent.Initialize(Window.GraphicsDevice);
        }

        /// <summary>
        /// Remove <see cref="IJCIWGameComponent"/>.
        /// </summary>
        /// <param name="gameComponent">The <see cref="IJCIWGameComponent"/>.</param>
        public void RemoveGameComponent(IJCIWGameComponent gameComponent)
        {
            lock (gameComponentsAfter)
            {
                if (gameComponentsAfter.Contains(gameComponent))
                {
                    gameComponentsAfter.Remove(gameComponent);
                }
            }

            lock (gameComponentsBefore)
            {
                if (gameComponentsBefore.Contains(gameComponent))
                {
                    gameComponentsBefore.Remove(gameComponent);
                }
            }
        }

        /// <summary>
        /// Remove all <see cref="IJCIWGameComponent"/>.
        /// </summary>
        public void ClearGameComponents()
        {
            lock (gameComponentsAfter)
            {
                gameComponentsAfter.Clear();
            }

            lock (gameComponentsBefore)
            {
                gameComponentsBefore.Clear();
            }
        }

        /// <summary>
        /// Load a new texture from <see cref="ImageSource"/>.
        /// </summary>
        /// <param name="imageSource">The <see cref="ImageSource"/>.</param>
        public void LoadTexture(ImageSource imageSource)
        {
            Window.LoadTexture(imageSource);
        }

        /// <summary>
        /// Unload texture.
        /// </summary>
        /// <param name="texture">The texture <see cref="IntPtr"/>.</param>
        public void UnloadTexture(IntPtr texture)
        {
            Window.UnloadTexture(texture);
        }

        /// <summary>
        /// Retrieve the loaded fonts.
        /// </summary>
        /// <returns>The <see cref="Fonts"/>.</returns>
        public Fonts GetFonts()
        {
            return Window.GetFonts();
        }
    }
}
