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
         {typeof(Int32),new Int32Converter()},
         {typeof(Int64),new Int64Converter()},
         {typeof(UInt32),new UInt32Converter()},
         {typeof(UInt64),new UInt64Converter()},
         {typeof(double),new DoubleConverter()},
         {typeof(float),new FloatConverter()},
         {typeof(bool),new BoolConverter()},
         {typeof(string),new StringConverter()},
      };

      /// <summary>
      /// Provides appropriate convertor for given type
      /// </summary>
      /// <param name="info">Option info with needed type information</param>
      /// <returns></returns>
      internal static IValueConverter getConverter(OptionInfo info)
      {
         Type tp;
         if (info.IsContainer)
            tp = info.ElementType;
         else
            tp = info.ExpectedType;
         
         if (!convertors.ContainsKey(tp))
         {
            EnumConverter conv;
            if (EnumConverter.TryCreate(info, out conv))
            {
               convertors.Add(tp, conv);
            }
            else
            {
               throw new ParserException(userMsg: "Error with convertors", developerMsg: "Unable to create convertor for given type Converters::getConverter");
            }
         }

         return convertors[tp];
      }
   }
}
