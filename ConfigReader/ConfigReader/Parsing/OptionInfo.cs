using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.Parsing
{
    class OptionInfo
    {
        internal readonly QualifiedOptionName Name;
        internal readonly object DefaultValue;
        internal readonly Type ExpectedType;        
    }
}
