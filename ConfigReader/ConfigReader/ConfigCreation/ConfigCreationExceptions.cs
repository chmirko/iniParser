using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigReader
{
    public class ConfigValidationException : ConfigRWException
    {
        /// <summary>
        /// Exception constructor
        /// </summary>
        /// <param name="userMsg">Message designated for user</param>
        /// <param name="developerMsg">Generic exception message, desingated for application developer</param>
        /// <param name="logMsg">Message designated to be logged into log file, for future inspection (null if same as developerMsg)</param>
        /// <param name="inner">Inner exception, for chained exceptions (null if no chained exception)</param>
        public ConfigValidationException(string userMsg, string developerMsg, string logMsg = null, Exception inner = null)
            : base(userMsg, developerMsg, logMsg, inner)
        {
        }
    }

    public class ContainerBuildException : ConfigRWException
    {
        public readonly Type ContainerType;

        public ContainerBuildException(string userMsg,string developerMsg,Type containerType):
            base(userMsg,developerMsg)
        {
            ContainerType = containerType;
        }
    }

    public class CreateInstanceException: ConfigRWException
    {
        

        public CreateInstanceException(string userMsg, string developerMsg, Exception inner) :
            base(userMsg, developerMsg, inner:inner)
        {            
        }
    }
}
