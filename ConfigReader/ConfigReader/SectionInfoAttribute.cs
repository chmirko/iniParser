using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Reflection;

namespace ConfigReader
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class SectionInfoAttribute:Attribute
    {
        public string ID { get; set; }
        public object DefaultValue { get; set; }
        public bool IsOptional { get; set; }

        public SectionInfoAttribute()
        {
        }
    }
}
