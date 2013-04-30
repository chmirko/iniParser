using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConfigReader.Parsing
{

   /// <summary>
   /// Convertor for string
   /// </summary>
   class StringConverter : IValueConverter
   {
      /// <summary>
      /// Deserializes given string into string object
      /// </summary>
      /// <param name="data">String to be deserialized</param>
      /// <returns>Desired object</returns>
      public object Deserialize(string data)
      {
         return data;
      }

      /// <summary>
      /// Serializes Int object into string representation
      /// </summary>
      /// <param name="obj">Int object to be serialized</param>
      /// <returns>String representation of given object</returns>
      public string Serialize(object obj)
      {
         return obj.ToString();
      }
   }
}
