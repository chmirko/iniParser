using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Reflection;

namespace ConfigRW
{
    /// <summary>
    /// Attribute for specifying additional information on sections.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class SectionInfoAttribute:Attribute
    {
        /// <summary>
        /// ID of section. This ID overrides default ID from property name.
        /// </summary>
        public string ID { get; set; }
        
        /// <summary>
        /// Determine that section has to be present in configuration file.
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        /// Create section info attribute.
        /// </summary>
        public SectionInfoAttribute()
        {
        }
    }
}
