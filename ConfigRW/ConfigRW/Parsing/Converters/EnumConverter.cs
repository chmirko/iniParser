using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigRW.Parsing.Converters
{
    class EnumConverter:IValueConverter
    {
        /// <summary>
        /// Type of enum which can be converted by this converter.
        /// </summary>
        internal readonly Type EnumType;

        private EnumConverter(Type enumType)
        {
            EnumType = enumType;
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
            Type enumType;
            if (optionInfo.IsContainer)
            {
                enumType = optionInfo.ElementType;
            }
            else
            {
                enumType = optionInfo.ExpectedType;
            }

            if (!enumType.IsEnum)
            {
                converter = null;
                return false;
            }

            converter = new EnumConverter(enumType);
            return true;
        }

        /// <summary>
        /// Deserializes given string into EnumType object
        /// </summary>
        /// <param name="data">String to be deserialized</param>
        /// <returns>Desired object</returns>
        public object Deserialize(string data)
        {
            try
            {
                return Enum.Parse(EnumType, data);
            }
            catch (Exception ex)
            {
                throw new ParserException(
                   userMsg: "Parser failed due to wrong syntax",
                   developerMsg: "EnumConverter::Serialize given object can not be parsed into desired type",
                   inner: ex);
            }
        }

        /// <summary>
        /// Serializes EnumType object into string representation
        /// </summary>
        /// <param name="obj">EnumType object to be serialized</param>
        /// <returns>String representation of given object</returns>
        public string Serialize(object obj)
        {
            if (obj == null)
            {
                throw new ParserException(
                   userMsg: "Parser failed due to wrong usage",
                   developerMsg: "EnumConverter::Serialize called with null argument in place of object to be parsed",
                   inner: new ArgumentNullException("obj"));
            }

            return obj.ToString();
        }
    }
}
