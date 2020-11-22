using System;
using System.IO;

namespace JCIW.Module
{
    /// <summary>
    /// Class for reading app and service headers
    /// </summary>
    public static class ModuleHeaderReader
    {
        /// <summary>
        /// Read app header from module file.
        /// </summary>
        /// <param name="libDirectory">The full path directory for where the library dependencies for modules are.</param>
        /// <param name="moduleFile">The path to the module file.</param>
        /// <returns>If the module is found and a service is found in it, then it will return <see cref="ModuleHeader"/>, else null.</returns>
        public static ModuleHeader AppHeader(string libDirectory, string moduleFile)
        {
            return Header(libDirectory, moduleFile, "Module.App");
        }

        /// <summary>
        /// Read service header from module file.
        /// </summary>
        /// <param name="libDirectory">The full path directory for where the library dependencies for modules are.</param>
        /// <param name="moduleFile">The path to the module file.</param>
        /// <returns>If the module is found and a service is found in it, then it will return <see cref="ModuleHeader"/>, else null.</returns>
        public static ModuleHeader ServiceHeader(string libDirectory, string moduleFile)
        {
            return Header(libDirectory, moduleFile, "Module.Service");
        }

        /// <summary>
        /// Attempts to read from module. 
        /// </summary>
        /// <param name="libDirectory">The full path directory for where the library dependencies for modules are.</param>
        /// <param name="moduleFile">The path to the module file.</param>
        /// <param name="className">The namespace of the module class.</param>
        /// <returns>Returns <see cref="ModuleHeader"/> if header is found. Else null.</returns>
        private static ModuleHeader Header(string libDirectory, string moduleFile, string className)
        {
            ModuleHeader module = null;

            if (File.Exists(moduleFile))
            {
                AppDomainSetup setup = new AppDomainSetup();
                setup.ApplicationBase = libDirectory;
                AppDomain domain = AppDomain.CreateDomain("PluginDomain", null, setup);
                ProxyObject proxyObject = (ProxyObject)domain.CreateInstanceFromAndUnwrap(typeof(ProxyObject).Assembly.Location, "JCIW.Module.ProxyObject");

                try
                {
                    proxyObject.InstantiateObject(moduleFile, className, null);

                    string name = proxyObject.InvokeStringMethod("Name", null);
                    string version = proxyObject.InvokeStringMethod("Version", null);

                    module = new ModuleHeader(name, version);
                }
                catch
                {
                    // Do nothing...
                }
                finally
                {
                    AppDomain.Unload(domain);
                }
            }

            return module;
        }
    }
}
