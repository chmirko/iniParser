using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace ConfigReader
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionInfoAttribute : Attribute
    {
        public string ID { get; set; }
        public object DefaultValue { get; set; }
        public bool IsOptional { get; set; }

        public OptionInfoAttribute()
        {
        }

        internal string GetOptionID(PropertyInfo optionProperty)
        {
            if (ID == null)
            {
                return optionProperty.Name;
            }
            else
            {
                return ID;
            }
        }
    }
}
