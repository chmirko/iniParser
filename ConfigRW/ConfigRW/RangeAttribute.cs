using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConfigRW
{

    /// <summary>
    /// Attribute for specifying range bounds on numerical values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RangeAttribute:Attribute
    {
        /// <summary>
        /// Upper bound of range.
        /// </summary>
        public object UpperBound { get; set; }
        /// <summary>
        /// Lower bound of range.
        /// </summary>
        public object LowerBound { get; set; }
    }
}
