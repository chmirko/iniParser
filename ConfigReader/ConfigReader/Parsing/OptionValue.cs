using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConfigReader.Parsing
{
    /// <summary>
    /// Option value of given name.
    /// </summary>
    class OptionValue
    {
        /// <summary>
        /// Config scope unique name of option.
        /// </summary>
        public readonly QualifiedOptionName Name;
        /// <summary>
        /// .NET representation of option value.
        /// </summary>
        public readonly object ConvertedValue;

        internal OptionValue(QualifiedOptionName name, object value)
        {
            Name = name;
            ConvertedValue = value;
        }
    }
}
