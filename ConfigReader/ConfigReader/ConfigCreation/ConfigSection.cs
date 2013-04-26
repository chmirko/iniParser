using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using ConfigReader.Parsing;

namespace ConfigReader.ConfigCreation
{
    class ConfigSection : PropertyStorage
    {
        private SectionInfo _info;
        private Dictionary<QualifiedOptionName, string> _associatedProperties = new Dictionary<QualifiedOptionName, string>();
        private Dictionary<string,QualifiedOptionName> _associatedPropertiesRev = new Dictionary< string,QualifiedOptionName>();

        internal QualifiedSectionName Name
        {
            get
            {
                return _info.Name;
            }
        }

        public string AssociatedProperty
        {
            get
            {
                return _info.AssociatedProperty;
            }
        }

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

        internal void SetSectionInfo(SectionInfo info)
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

        internal OptionInfo GetOptionInfo(QualifiedOptionName name)
        {
            return _info.GetOptionInfo(name);
        }


        internal void ClearOptionChangeLog()
        {
            ClearChangeLog();
        }

 
        private string getOptionProperty(QualifiedOptionName optionName)
        {
            return _associatedProperties[optionName];
        }

        internal void SetOption(QualifiedOptionName optionName, object value)
        {
            var associatedProperty = getOptionProperty(optionName);
            this.DirectPropertySet(associatedProperty, value);
        }
    }
}
