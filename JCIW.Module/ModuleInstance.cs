using System;
using System.Reflection;

namespace JCIW.Module
{
    /// <summary>
    /// Class for running modules.
    /// </summary>
    public class ModuleInstance
    {
        private readonly object[] objects;
        private readonly string className;
        private readonly string modulePath;
        private bool isLoaded;

        private MethodInfo serviceCommandMethod;
        private MethodInfo initializeMethod;
        private MethodInfo unloadMethod;
        private MethodInfo drawMethod;
        private Assembly moduleAssembly;
        private object instance;
        private Type moduleType;

        /// <summary>
        /// Creates <see cref="ModuleInstance"/>.
        /// </summary>
        /// <param name="modulePath">The path to the module file.</param>
        /// <param name="className">The class name.</param>
        /// <param name="objects">The class parameters used to run initialize.</param>
        public ModuleInstance(string modulePath, string className, object[] objects)
        {
            this.objects = objects;
            this.className = className;
            this.modulePath = modulePath;

            isLoaded = false;
        }

        /// <summary>
        /// Start module.
        /// </summary>
        /// <returns>true</returns>
        public bool Load()
        {
            if (moduleAssembly == null)
            {
                moduleAssembly = Assembly.LoadFile(modulePath);
                moduleType = moduleAssembly.GetType(className);

                instance = Activator.CreateInstance(moduleType);
                unloadMethod = moduleType.GetMethod("Unload");
                initializeMethod = moduleType.GetMethod("Initialize");
            }

            if (!isLoaded)
            {
                initializeMethod.Invoke(instance, objects);

                isLoaded = true;
            }

            return isLoaded;
        }

        /// <summary>
        /// Executes service command on service modules.
        /// </summary>
        /// <param name="command">The command to run.</param>
        /// <returns>The result from the module.</returns>
        public string ExecuteServiceCommand(string command)
        {
            string commandResult = "";

            if (moduleAssembly != null)
            {
                if (serviceCommandMethod == null)
                {
                    serviceCommandMethod = moduleType.GetMethod("Command");
                }

                commandResult = serviceCommandMethod.Invoke(instance, new object[] { command }).ToString();
            }
            else
            {
                commandResult = "Service not loaded.";
            }

            return commandResult;
        }

        /// <summary>
        /// Calls the draw method on app modules.
        /// </summary>
        public void CallDraw()
        {
            if (drawMethod == null)
            {
                drawMethod = moduleType.GetMethod("Draw");
            }

            drawMethod.Invoke(instance, null);
        }

        /// <summary>
        /// Unload / free resources on running module.
        /// </summary>
        public void Unload()
        {
            if (isLoaded)
            {
                unloadMethod.Invoke(instance, null);
                isLoaded = false;
            }
        }
    }
}
