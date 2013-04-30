using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigReader.Parsing.Converters
{
    class EnumConverter:IValueConverter
    {
        /// <summary>
        /// Type of enum which can be converted by this converter.
        /// </summary>
        internal readonly Type EnumType;

        private EnumConverter(OptionInfo optionInfo)
        {
        }

        /// <summary>
        /// Create enum converter for optionInfo. 
        /// NOTE: Enum can be created according to ElementType and not ExpectedType.
        /// </summary>
        /// <param name="optionInfo">Option which converter will be created.</param>
        /// <param name="converter">Result converter if it was created, null otherwise.</param>
        /// <returns>True if converter was successfuly created, false otherwise.</returns>
        internal static bool TryCreate(OptionInfo optionInfo, out EnumConverter converter)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(string data)
        {
            throw new NotImplementedException();
        }

        public string Serialize(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
