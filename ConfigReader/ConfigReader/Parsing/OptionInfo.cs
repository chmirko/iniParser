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
        internal readonly Type ExpectedType;    
        internal readonly object DefaultValue;
        internal readonly string DefaultComment;


        internal OptionInfo(QualifiedOptionName name, Type expectedType, object defaultValue, string defaultComment)
        {
            Name = name;
            ExpectedType = expectedType;
            DefaultValue = defaultValue;
            DefaultComment = defaultComment;
        }
    }
}
