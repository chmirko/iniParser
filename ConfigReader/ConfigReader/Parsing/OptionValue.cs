using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.Parsing
{
    class OptionValue
    {
        public readonly QualifiedOptionName Name;
        public readonly object ConvertedValue;

        internal OptionValue(QualifiedOptionName name, object value)
        {
            Name = name;
            ConvertedValue = value;
        }
    }
}
