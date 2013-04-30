using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConfigReader.Parsing
{

   /// <summary>
   /// Convertor for integer
   /// </summary>
   class DoubleConverter : IValueConverter
   {
      /// <summary>
      /// Deserializes given string into Int object
      /// </summary>
      /// <param name="data">String to be deserialized</param>
      /// <returns>Desired object</returns>
      public object Deserialize(string data)
      {
         try
         {
            return double.Parse(data);
         }
         catch (Exception ex)
         {
            throw new ParserException(
               userMsg: "Parser failed due to wrong usage",
               developerMsg: "IntConverter::Serialize given object can not be parsed into desired type",
               inner: ex);
         }
      }

      /// <summary>
      /// Serializes Int object into string representation
      /// </summary>
      /// <param name="obj">Int object to be serialized</param>
      /// <returns>String representation of given object</returns>
      public string Serialize(object obj)
      {
         if (obj == null)
         {
            throw new ParserException(
               userMsg: "Parser failed due to wrong usage",
               developerMsg: "IntConverter::Serialize called with null argument in place of object to b eparsed",
               inner: new ArgumentNullException("obj"));
         }

         return obj.ToString();
      }
   }
}
