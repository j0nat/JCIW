using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using GraphicsEngine;
using JCIW.Data;
using JCIW.Data.Drawing;
using JCIW.Data.Interfaces;

namespace JCIW
{
    /// <summary>
    /// This class handles the graphical window for the app.
    /// </summary>
    public class Frame : IFrame
    {
        private GUIEngine graphics;
        private List<Action> actionQueue;
        public float Width;
        public float Height;
        public Fonts Fonts { get; private set; }

        /// <summary>
        /// Create <see cref="Frame"/>.
        /// </summary>
        /// <param name="platform">The <see cref="Platform"/> type.</param>
        /// <param name="platformFunctions">The <see cref="IPlatformFunctions"/> implementation.</param>
        /// <param name="app">The <see cref="IApp"/> implementation.</param>
        public Frame(Platform platform, IPlatformFunctions platformFunctions, IApp app)
        {
            this.graphics = new GUIEngine(platform, platformFunctions, this, app);
            this.actionQueue = new List<Action>();
            this.Fonts = graphics.GetFonts(); 
        }

        /// <summary>
        /// Add <see cref="Action"/> to run on UI thread.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Invoke(Action action)
        {
            lock (actionQueue)
            {
                actionQueue.Add(action);
            }
        }

        /// <summary>
        /// Set the frames background color.
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        public void SetBackgroundColor(int r, int g, int b)
        {
            graphics.SetBackgroundColor(r, g, b);
        }

        public void AddGameComponent(IJCIWGameComponent gameComponent, DrawOrder drawOrder)
        {
            graphics.AddGameComponent(gameComponent, drawOrder);
        }

        public void RemoveGameComponent(IJCIWGameComponent gameComponent)
        {
            graphics.RemoveGameComponent(gameComponent);
        }

        public void ClearGameComponents()
        {
            graphics.ClearGameComponents();
        }

        /// <summary>
        /// Returns the graphical window.
        /// </summary>
        /// <returns>The graphical window object.</returns>
        public object Window()
        {
            return (object)graphics.Window;
        }

        /// <summary>
        /// Called when the screen size changes.
        /// </summary>
        /// <param name="height">The screen height.</param>
        public void SetScreenHeight(float height)
        {
            this.Height = height;
        }

        /// <summary>
        /// Called when the screen size changes.
        /// </summary>
        /// <param name="width">The screen width.</param>
        public void SetScreenWidth(float width)
        {
            this.Width = width;
        }

        /// <summary>
        /// Load image for drawing.
        /// </summary>
        /// <param name="ImageSource">Image data.</param>
        public void LoadTexture(ImageSource imageSource)
        {
            graphics.LoadTexture(imageSource);
        }

        /// <summary>
        /// Unload loaded image.
        /// </summary>
        /// <param name="IntPtr">Image data.</param>
        public void UnloadTexture(IntPtr texture)
        {
            graphics.UnloadTexture(texture);
        }

        /// <summary>
        /// Called when the frame updates.
        /// </summary>
        public void Update()
        {
            lock (actionQueue)
            {
                if (actionQueue.Count > 0)
                {
                    Action action = actionQueue[0];

                    action.Invoke();

                    actionQueue.RemoveAt(0);
                }
            }
        }
    }
}
