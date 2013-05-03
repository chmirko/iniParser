using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Reflection;

namespace ConfigRW
{
    /// <summary>
    /// Attribute for specifying additional information on options.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OptionInfoAttribute : Attribute
    {
        /// <summary>
        /// ID of option. This ID overrides default ID from property name.         
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Default value for option. Containers default value has to be writen in array.
        /// </summary>
        public object DefaultValue { get; set; }
        /// <summary>
        /// Determine that option has to be present in configuration file.
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        /// Create option info attribute.
        /// </summary>
        public OptionInfoAttribute()
        {
        }
    }
}
