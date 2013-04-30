using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigReader.Parsing.Converters
{
   /// <summary>
   /// Converter used for easy conversion of supported types
   /// </summary>
   internal class ConfigConverters
   {
      /// <summary>
      /// Convertors for given types
      /// </summary>
      internal static Dictionary<Type, IValueConverter> convertors = new Dictionary<Type, IValueConverter>(){
         {typeof(int),new IntConverter()},
         {typeof(Int32),new Int32Converter()},
         {typeof(Int64),new Int64Converter()},
         {typeof(UInt32),new UInt32Converter()},
         {typeof(UInt64),new UInt64Converter()},
         {typeof(double),new DoubleConverter()},
         {typeof(float),new FloatConverter()}
      };
   }
}
