using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConfigReader.Parsing
{

   /// <summary>
   /// Convertor for boolean
   /// </summary>
   class BoolConverter : IValueConverter
   {
      /// <summary>
      /// Deserializes given string into Bool object
      /// </summary>
      /// <param name="data">String to be deserialized</param>
      /// <returns>Desired object</returns>
      public object Deserialize(string data)
      {
         if (data.Equals("1") || data.Equals("t")|| data.Equals("y")|| data.Equals("on")|| data.Equals("yes")|| data.Equals("enabled"))
         {
            return true;
         }
         else if (data.Equals("0") || data.Equals("f")|| data.Equals("n")|| data.Equals("off")|| data.Equals("no")|| data.Equals("disabled"))
         {
            return false;
         }
         else
         {
            throw new ParserException(
               userMsg: "Parser failed due to wrong usage",
               developerMsg: "BoolConverter::Serialize given object can not be parsed into desired type");
         }
      }

      /// <summary>
      /// Serializes Bool object into string representation
      /// </summary>
      /// <param name="obj">Bool object to be serialized</param>
      /// <returns>String representation of given object</returns>
      public string Serialize(object obj)
      {
         if (obj == null)
         {
            throw new ParserException(
               userMsg: "Parser failed due to wrong usage",
               developerMsg: "BoolConverter::Serialize called with null argument in place of object to b eparsed",
               inner: new ArgumentNullException("obj"));
         }

         if ((bool)obj)
         {
            return "enabled";
         }
         else
         {
            return "disabled";
         }
      }
   }
}
