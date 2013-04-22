using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace ConfigReader.Parsing
{
    class SectionInfo
    {
        internal readonly QualifiedSectionName Name;
        internal readonly Type SectionType;
        internal readonly string AssociatedProperty;
        internal readonly string DefaultComment;
        internal readonly IEnumerable<OptionInfo> Options;


        private readonly Dictionary<QualifiedOptionName, OptionInfo> _options = new Dictionary<QualifiedOptionName, OptionInfo>();

        internal SectionInfo(QualifiedSectionName name,PropertyInfo associatedProperty, List<OptionInfo> options,string defaultComment)
        {
            Name = name;

            AssociatedProperty = associatedProperty.Name;
            SectionType = associatedProperty.PropertyType;

            foreach (var option in options)
            {
                _options[option.Name] = option;
            }

            Options = _options.Values;

            DefaultComment = defaultComment;
        }

        internal OptionInfo GetOptionInfo(QualifiedOptionName name)
        {
            return _options[name];
        }
    }
}
