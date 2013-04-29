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

   public class ParserExceptionWithinConfig : ParserException
   {
      /// <summary>
      /// Line where exception ocurred
      /// </summary>
      public readonly uint Line;

      /// <summary>
      /// Section where exception ocurred (if aplicable)
      /// </summary>
      public readonly string Section;
      
      /// <summary>
      /// Option where exception ocurred (if aplicable)
      /// </summary>
      public readonly string Option;

      /// <summary>
      /// Exception constructor
      /// </summary>
      /// <param name="userMsg">Message designated for user</param>
      /// <param name="developerMsg">Generic exception message, desingated for application developer</param>
      /// <param name="logMsg">Message designated to be logged into log file, for future inspection (null if same as developerMsg)</param>
      /// <param name="line">Line in configuration file where exception ocurred</param>
      /// <param name="inner">Inner exception, for chained exceptions (null if no chained exception)</param>
      public ParserExceptionWithinConfig(string userMsg, string developerMsg, string logMsg = null, uint line = 0, string section = null, string option = null, Exception inner = null)
         : base(userMsg, developerMsg, logMsg, inner)
      {
         Line = line;
         Section = section;
         Option = option; 
      }
   }
}
