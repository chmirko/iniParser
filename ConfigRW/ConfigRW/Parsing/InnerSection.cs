using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigRW.Parsing
{
   /// <summary>
   /// Class representing config section,
   /// designated for parser internal use
   /// </summary>
   internal class InnerSection
   {
      /// <summary>
      /// Section name
      /// </summary>
      internal readonly QualifiedSectionName Name;

      /// <summary>
      /// Collection of options from this section
      /// </summary>
      internal Dictionary<QualifiedOptionName, InnerOption> Options = new Dictionary<QualifiedOptionName, InnerOption>();

      /// <summary>
      /// Section comment
      /// </summary>
      internal string Comment;

      /// <summary>
      /// Mark, for strict mode
      /// </summary>
      internal bool Seen;

      /// <summary>
      /// Name of section as seen in config
      /// </summary>
      /// <param name="name">Qualified name</param>
      /// <param name="comment">Section comment</param>
      internal InnerSection(QualifiedSectionName name, string comment)
      {
         Name = name;
         Comment = comment;
      }
   }
}
