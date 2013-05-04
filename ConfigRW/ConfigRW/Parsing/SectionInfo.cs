using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Reflection;

namespace ConfigRW.Parsing
{
    /// <summary>
    /// Info describing section structure.
    /// </summary>
    class SectionInfo
    {
        /// <summary>
        /// Config scope unique name for section.
        /// </summary>
        internal readonly QualifiedSectionName Name;
        /// <summary>
        /// Type which describes section structure.
        /// </summary>
        internal readonly Type DescribingType;     
        /// <summary>
        /// Default comment associated with section.
        /// </summary>
        internal readonly string DefaultComment;
        /// <summary>
        /// Options that are present in section.
        /// </summary>
        internal readonly IEnumerable<OptionInfo> Options;
        /// <summary>
        /// Name of property that is associated with section.
        /// </summary>
        internal readonly string AssociatedProperty;

        /// <summary>
        /// Determine that section is optional.
        /// </summary>
        internal readonly bool IsOptional;

        /// <summary>
        /// Here are stored options in section.
        /// </summary>
        private readonly Dictionary<QualifiedOptionName, OptionInfo> _options = new Dictionary<QualifiedOptionName, OptionInfo>();

        internal SectionInfo(QualifiedSectionName name,bool isOptional,PropertyInfo associatedProperty, List<OptionInfo> options,string defaultComment)
        {
            Name = name;

            AssociatedProperty = associatedProperty.Name;
            DescribingType = associatedProperty.PropertyType;

            foreach (var option in options)
            {
                _options[option.Name] = option;
            }

            Options = _options.Values;

            DefaultComment = defaultComment;
        }

        /// <summary>
        /// Get option info for option with given name.
        /// </summary>
        /// <param name="name">Name of wanted option.</param>
        /// <returns>Option info for given name.</returns>
        internal OptionInfo GetOptionInfo(QualifiedOptionName name)
        {
            return _options[name];
        }

        
    }
}
