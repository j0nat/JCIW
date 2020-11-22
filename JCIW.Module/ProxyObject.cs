using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JCIW.Module
{
    /// <summary>
    /// Class to access objects accross domain. 
    /// </summary>
    public class ProxyObject : MarshalByRefObject
    {
        private Type type;
        private Object instance;

        /// <summary>
        /// Create instance of class.
        /// </summary>
        /// <param name="modulePath">Path to module file.</param>
        /// <param name="typeName">Namespace of class to create instance of.</param>
        /// <param name="args">The class arguments.</param>
        public void InstantiateObject(string modulePath, string typeName, object[] args)
        {
            Assembly assembly = Assembly.LoadFile(modulePath);

            this.type = assembly.GetType(typeName);
            this.instance = Activator.CreateInstance(this.type, args);
        }

        /// <summary>
        /// Call a string method.
        /// </summary>
        /// <param name="methodName">The name of the method in the class to invoke.</param>
        /// <param name="args">The arguments of the method.</param>
        /// <returns>The string returned cross domain.</returns>
        public string InvokeStringMethod(string methodName, object[] args)
        {
            var methodinfo = type.GetMethod(methodName);
            return methodinfo.Invoke(instance, args).ToString();
        }
    }
}
