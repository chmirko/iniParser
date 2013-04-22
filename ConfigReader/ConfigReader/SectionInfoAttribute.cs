using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace ConfigReader
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SectionInfoAttribute:Attribute
    {
        public string ID { get; set; }
        public object DefaultValue { get; set; }
        public bool IsOptional { get; set; }

        public SectionInfoAttribute()
        {
        }

        internal string GetSectionID(PropertyInfo sectionProperty)
        {
            if (ID == null)
            {
                return sectionProperty.Name;
            }
            else
            {
                return ID;
            }
        }
    }
}
