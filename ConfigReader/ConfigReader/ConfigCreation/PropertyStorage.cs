using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Reflection;

namespace ConfigReader.ConfigCreation
{
    class PropertyStorage
    {
        
        public static readonly MethodInfo SetterProvider = typeof(PropertyStorage).GetMethod("Setter", BindingFlags.Instance | BindingFlags.NonPublic);

        
        HashSet<string> _changeLog = new HashSet<string>();

        internal IEnumerable<string> ChangedProperties
        {
            get
            {
                return _changeLog;
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

        protected void ClearChangeLog()
        {
            _changeLog.Clear();
        }


        internal void Setter<T>(string propertyName,ref T propertyField, T value)
        {
            _changeLog.Add(propertyName);
            propertyField = value;
        }

        protected void DirectPropertySet(string propertyName, object value)
        {
            getPropertyField(propertyName).SetValue(this, value);
        }

        protected object ReadStoredProperty(string propertyName)
        {
            return getPropertyField(propertyName).GetValue(this);
        }

        private FieldInfo getPropertyField(string fieldName)
        {
            var result= this.GetType().GetField("property_" + fieldName,BindingFlags.NonPublic|BindingFlags.Instance);
            return result;
        }
    }
}
