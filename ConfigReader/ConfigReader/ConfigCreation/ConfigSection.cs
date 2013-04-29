using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using ConfigReader.Parsing;

namespace ConfigReader.ConfigCreation
{
    /// <summary>
    /// Section of configuration structure.
    /// </summary>
    class ConfigSection : PropertyStorage
    {
        /// <summary>
        /// Source info for this section.
        /// </summary>
        private SectionInfo _info;
        /// <summary>
        /// Lookup for associated properties according to options.
        /// </summary>
        private Dictionary<QualifiedOptionName, string> _associatedProperties = new Dictionary<QualifiedOptionName, string>();
        /// <summary>
        /// Lookup for options according to associated properties.
        /// </summary>
        private Dictionary<string,QualifiedOptionName> _associatedPropertiesRev = new Dictionary< string,QualifiedOptionName>();

        /// <summary>
        /// Name of section.
        /// </summary>
        internal QualifiedSectionName Name
        {
            get
            {
                return _info.Name;
            }
        }

        /// <summary>
        /// Property which is associated with this section.
        /// </summary>
        public string AssociatedProperty
        {
            get
            {
                return _info.AssociatedProperty;
            }
        }

        /// <summary>
        /// Enumeration of changed options.
        /// </summary>
        public IEnumerable<OptionValue> ChangedOptions
        {
            get
            {
                foreach (var changed in ChangedProperties)
                {
                    var value = new OptionValue(_associatedPropertiesRev[changed], ReadStoredProperty(changed));
                    yield return value;
                }
            }
        }

        /// <summary>
        /// Initialize source section info.
        /// NOTE: It's not passed through construtor, because this type is dynamicallly instantied.
        /// </summary>
        /// <param name="info"></param>
        internal void InitializeSectionInfo(SectionInfo info)
        {
            _info = info;
            foreach (var option in info.Options)
            {
                var propertyName=option.AssociatedProperty;
                var optionName=option.Name;
                _associatedProperties[optionName] =propertyName;
                _associatedPropertiesRev[propertyName] = optionName;
            }
        }

        /// <summary>
        /// Get option info for given name.
        /// </summary>
        /// <param name="name">Option name.</param>
        /// <returns>Option info.</returns>
        internal OptionInfo GetOptionInfo(QualifiedOptionName name)
        {
            return _info.GetOptionInfo(name);
        }

        /// <summary>
        /// Clear property change log.
        /// </summary>
        internal void ClearOptionChangeLog()
        {
            ClearChangeLog();
        }

        /// <summary>
        /// Set option with given value.
        /// </summary>
        /// <param name="name">Option name.</param>
        /// <param name="value">Option value.</param>
        internal void SetOption(QualifiedOptionName name, object value)
        {
            var associatedProperty = getOptionProperty(name);
            this.DirectPropertySet(associatedProperty, value);
        }
        
        /// <summary>
        /// Get name of property associated with option of given name.
        /// </summary>
        /// <param name="name">Option name.</param>
        /// <returns>Name of property associated with option.</returns>
        private string getOptionProperty(QualifiedOptionName name)
        {
            return _associatedProperties[name];
        }
    }
}
