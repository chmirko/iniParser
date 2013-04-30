using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigReader.Parsing
{
   /// <summary>
   /// Class representing sinlge option, not designated for user
   /// </summary>
   class InnerOption
   {
      /// <summary>
      /// Option name
      /// </summary>
      internal readonly QualifiedOptionName Name;

      /// <summary>
      /// Line in configuration file (if available)
      /// </summary>
      internal readonly uint? Line;

      /// <summary>
      /// List of all values retrieved from config file
      /// </summary>
      internal List<string> strValues = new List<string>();

      /// <summary>
      /// Option comment
      /// </summary>
      internal string Comment = null;

      /// <summary>
      /// Mark, for strict mode
      /// </summary>
      internal bool Seen;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="name">Qualified name</param>
      /// <param name="line">Line in configuration file</param>
      internal InnerOption(QualifiedOptionName name, uint? line)
      {
         Name = name;
         Line = line;
      }
   }
}
