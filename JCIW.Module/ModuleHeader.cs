namespace JCIW.Module
{
    /// <summary>
    /// This class contains the header data from modules
    /// </summary>
    public class ModuleHeader
    {
        public string Name { get; private set; }
        public string Version { get; private set; }

        /// <summary>
        /// Creates <see cref="ModuleHeader"/>.
        /// </summary>
        /// <param name="name">The name of the module</param>
        /// <param name="version">The version of the module</param>
        public ModuleHeader(string name, string version)
        {
            this.Name = name;
            this.Version = version;
        }
    }
}
