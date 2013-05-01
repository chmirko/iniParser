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
        private Dictionary<string, QualifiedOptionName> _associatedPropertiesRev = new Dictionary<string, QualifiedOptionName>();

        /// <summary>
        /// Contains backup of all elements in container. Is used for equality comparison.
        /// </summary>
        private Dictionary<QualifiedOptionName, List<object>> _containerBackups = new Dictionary<QualifiedOptionName, List<object>>();

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
        internal string AssociatedProperty
        {
            get
            {
                return _info.AssociatedProperty;
            }
        }


        /// <summary>
        /// Enumeration of changed options.
        /// </summary>
        internal IEnumerable<OptionValue> ChangedOptions
        {
            get
            {
                var changes = new HashSet<string>(ChangedProperties);

                foreach (var backup in _containerBackups)
                {
                    var associatedProperty = _associatedProperties[backup.Key];
                    if (changes.Contains(associatedProperty))
                    {
                        //this container is already marked as changed.
                        continue;
                    }

                    var currContainer = ReadStoredProperty(associatedProperty);
                    if(!isEqual(backup.Value,StructureFactory.GetContainerElements(currContainer))){
                        changes.Add(associatedProperty);
                    }
                }

                foreach (var change in changes)
                {
                    var value = new OptionValue(_associatedPropertiesRev[change], ReadStoredProperty(change));
                    yield return value;
                }
            }
        }

        /// <summary>
        /// Initialize source section info.
        /// NOTE: It's not passed through construtor, because this type is dynamicallly instantied.
        /// </summary>
        /// <param name="info">Section info for this config section.</param>
        internal void InitializeSectionInfo(SectionInfo info)
        {
            _info = info;
            foreach (var option in info.Options)
            {
                var propertyName = option.AssociatedProperty;
                var optionName = option.Name;
                _associatedProperties[optionName] = propertyName;
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

            var info = GetOptionInfo(name);
            if (info.IsContainer)
            {
                registerContainer(info, value);
            }
        }


        /// <summary>
        /// Determine that old value is equal (has same elements) as currValue.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="currValue">Current value.</param>
        /// <returns>True if enumerations contains same elements, false otherwise.</returns>
        private bool isEqual(List<object> oldValue, IEnumerable<object> currValue)
        {
            if (oldValue == null)
            {
                return currValue == null;
            }

            if (currValue == null)
            {
                return false;
            }


            if (oldValue.Count != currValue.Count())
            {
                return false;
            }

            int i = 0;
            foreach (var el in currValue)
            {
                if (oldValue[i] != el)
                {
                    return false;
                }
                ++i;
            }

            return true;
        }

        /// <summary>
        /// Register container option, because of equality comparison.
        /// </summary>
        /// <param name="optionInfo">Option info for container option.</param>
        /// <param name="container">Container to be registered.</param>
        private void registerContainer(OptionInfo optionInfo, object container)
        {
            var containerElements = StructureFactory.GetContainerElements(container);
            var backup = new List<object>(containerElements);
            _containerBackups[optionInfo.Name] = backup;
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
