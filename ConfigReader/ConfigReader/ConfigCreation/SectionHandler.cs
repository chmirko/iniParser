using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

using ConfigReader.Parsing;

namespace ConfigReader.ConfigCreation
{
    internal class SectionHandler
    {
        public readonly QualifiedSectionName Name;
        public readonly ConfigSection Storage;
        public readonly Type SectionType;
        

        public SectionHandler(PropertyInfo sectionProperty,ConfigSection sectionData)
        {
            SectionType = sectionProperty.PropertyType;
            Name = new QualifiedSectionName(sectionProperty.Name);
            Storage = sectionData;
        }
    }
}
