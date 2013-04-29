using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConfigReader.Parsing
{
   /// <summary>
   /// Interface of convertor between String and Value common to the convertor
   /// </summary>
   interface IValueConverter
   {
      /// <summary>
      /// Deserializes given string into desired object
      /// </summary>
      /// <param name="data">String to be deserialized</param>
      /// <returns>Desired object</returns>
      object Deserialize(string data);

      /// <summary>
      /// Serializes desired object into its string representation
      /// </summary>
      /// <param name="obj">Desired object to be serialized</param>
      /// <returns>String representation of given object</returns>
      string Serialize(object obj);
   }
}
