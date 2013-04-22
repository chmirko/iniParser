using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace ConfigReader.ConfigCreation
{
    internal class SectionHandler
    {
        public readonly string Name;
        public readonly ConfigSection Storage;
        public readonly Type SectionType;
        

        public SectionHandler(PropertyInfo sectionProperty,ConfigSection sectionData)
        {
            SectionType = sectionProperty.PropertyType;
            Name = sectionProperty.Name;
            Storage = sectionData;
        }
    }
}
