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
      /// List of all values retrieved from config file
      /// </summary>
      private List<string> strValues = new List<string>();

      /// <summary>
      /// Option comment
      /// </summary>
      internal string Comment;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="name">Qualified name</param>
      /// <param name="comment">Option comment</param>
      internal InnerOption(QualifiedOptionName name, string comment = null)
      {
         Name = name;
         Comment = comment;
      }
   }
}
