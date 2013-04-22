using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace ConfigReader.ConfigCreation
{
    class PropertyStorage
    {
        HashSet<string> _changedProperties = new HashSet<string>();

        internal IEnumerable<string> ChangedProperties
        {
            get
            {
                return _changedProperties;
            }
        }

        public IEnumerable<string> StoredProperties
        {
            get
            {
                foreach (var field in this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (field.Name.StartsWith("property_"))
                    {
                        yield return field.Name.Substring("property_".Length);
                    }
                }
            }
        }
        internal T Getter<T>(string propertyName, ref T propertyField)
        {
            return propertyField;
        }

        internal void Setter<T>(string propertyName, ref T propertyField, T value)
        {
            _changedProperties.Add(propertyName);
            propertyField = value;
        }

        internal void InitializeStoredProperty(string propertyName, object value)
        {
            getStoredField(propertyName).SetValue(this, value);
        }

        internal object ReadStoredProperty(string propertyName)
        {
            return getStoredField(propertyName).GetValue(this);
        }


        private FieldInfo getStoredField(string fieldName)
        {
            var result= this.GetType().GetField("property_" + fieldName,BindingFlags.NonPublic|BindingFlags.Instance);
            return result;
        }


    }
}
