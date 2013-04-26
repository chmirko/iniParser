using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConfigReader
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RangeAttribute:Attribute
    {
        public object UpperBound { get; set; }
        public object LowerBound { get; set; }
    }
}
