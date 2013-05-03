using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigRW
{
    /// <summary>
    /// Base exception for exceptions thrown out of Config Reader and Writer.    
    /// </summary>
    public class ConfigRWException : Exception
    {
        /// <summary>
        /// Exception message designated for user to be seen in console, msgBox etc
        /// </summary>
        public readonly string UserMsg;

        /// <summary>
        /// Exception message designated to be logged into file
        /// </summary>
        public readonly string LogMsg;


        /// <summary>
        /// Exception constructor
        /// </summary>
        /// <param name="userMsg">Message designated for user</param>
        /// <param name="developerMsg">Generic exception message, desingated for application developer</param>
        /// <param name="logMsg">Message designated to be logged into log file, for future inspection (null if same as developerMsg)</param>
        /// <param name="inner">Inner exception, for chained exceptions (null if no chained exception)</param>      
        public ConfigRWException(string userMsg, string developerMsg, string logMsg = null, Exception inner = null):
            base(developerMsg, inner)
        {
            UserMsg = userMsg;
            LogMsg = (logMsg != null) ? logMsg : developerMsg;            
        }

    }
}
