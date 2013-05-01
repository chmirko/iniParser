using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Reflection;

namespace ConfigReader.ConfigCreation
{
    class PropertyStorage
    {
        /// <summary>
        /// Names of changed properties are stored here.
        /// </summary>
        HashSet<string> _changeLog = new HashSet<string>();

        /// <summary>
        /// Setter provider. Is used for registering setter calls.
        /// WARNING: Don't change signature. Is called from IL emited code.
        /// </summary>
        /// <typeparam name="T">Type of setted property.</typeparam>
        /// <param name="propertyName">Name of setted property.</param>
        /// <param name="propertyField">Storage field for property. (Is used for getter)</param>
        /// <param name="value">Value that is passed to setter.</param>
        internal void Setter<T>(string propertyName, ref T propertyField, T value)
        {
            _changeLog.Add(propertyName);
            propertyField = value;
        }

        /// <summary>
        /// Enumeration of changed properties.
        /// </summary>
        internal IEnumerable<string> ChangedProperties
        {
            get
            {
                return _changeLog;
            }
        }

        /// <summary>
        /// Clear log of changed properties.
        /// </summary>
        protected void ClearChangeLog()
        {
            _changeLog.Clear();
        }



        /// <summary>
        /// Directly set value of given property (without adding it to change log).
        /// </summary>
        /// <param name="propertyName">Name of property to be set.</param>
        /// <param name="value">Value that will be set.</param>
        protected void DirectPropertySet(string propertyName, object value)
        {
            getPropertyField(propertyName).SetValue(this, value);
        }

        /// <summary>
        /// Read value from stored property.
        /// </summary>
        /// <param name="propertyName">Name of property.</param>
        /// <returns>Value from property.</returns>
        protected object ReadStoredProperty(string propertyName)
        {
            return getPropertyField(propertyName).GetValue(this);
        }

        /// <summary>
        /// Get field used as storage for given property.
        /// </summary>
        /// <param name="propertyName">Name of property.</param>
        /// <returns>Field info of storage for given property.</returns>
        private FieldInfo getPropertyField(string propertyName)
        {
            var fieldName = ReflectionUtils.DynamicField_Prefix + propertyName;
            var field = this.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return field;
        }
    }
}
