using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConfigReader.Parsing
{
   /// <summary>
   /// Exceptions thrown out of parser
   /// Except really extraordynary conditions, this should be the only one exception thorwon out of the parser
   /// </summary>
   public class ParserException : Exception
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
      public ParserException(string userMsg, string developerMsg, string logMsg = null, Exception inner = null)
         : base(developerMsg, inner)
      {
         UserMsg = userMsg;
         LogMsg = (logMsg != null) ? logMsg : developerMsg;
      }
   }
}
