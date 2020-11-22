using System.IO;

namespace JCIW.App
{
    /// <summary>
    /// This class is used to save and store data to disk.
    /// </summary>
    static class LocalData
    {
        private readonly static string sessionFile = "session.data";
        private readonly static string loginFile = "login.data";

        /// <summary>
        /// Read and return the session string from disk.
        /// </summary>
        /// <returns>Session string from disk or null if file doesn't exist.</returns>
        public static string RetrieveSession()
        {
            string returnValue = null;

            string filePath = Path.Combine(Local.Folder, sessionFile);

            if (File.Exists(filePath))
            {
                returnValue = File.ReadAllText(filePath);
            }

            return returnValue;
        }

        /// <summary>
        /// Store session ID to disk.
        /// </summary>
        /// <param name="sessionId">The session ID to store.</param>
        public static void SaveSession(string sessionId)
        {
            string filePath = Path.Combine(Local.Folder, sessionFile);

            File.WriteAllText(filePath, sessionId);
        }

        /// <summary>
        /// Read host and username from file system.
        /// </summary>
        /// <param name="host">Host IP address or domain.</param>
        /// <param name="username">Login username.</param>
        public static void RetrieveLogin(out string host, out string username)
        {
            host = null;
            username = null;

            string filePath = Path.Combine(Local.Folder, loginFile);

            if (File.Exists(filePath))
            {
                string output = File.ReadAllText(filePath);

                if (output.Contains("\n"))
                {
                    host = output.Split('\n')[0];
                    username = output.Split('\n')[1];
                }
            }
        }

        /// <summary>
        /// Save login data to disk.
        /// </summary>
        /// <param name="host">Host IP address or domain.</param>
        /// <param name="username">Login username.</param>
        public static void SaveLogin(string host, string username)
        {
            string filePath = Path.Combine(Local.Folder, loginFile);

            File.WriteAllText(filePath, host + "\n" + username);
        }
    }
}
