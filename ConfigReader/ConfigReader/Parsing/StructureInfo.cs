using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.Parsing
{
    class StructureInfo
    {
        internal readonly IEnumerable<SectionInfo> Sections;
        internal readonly Type StructureType;

        internal StructureInfo(Type structureType, IEnumerable<SectionInfo> sections)
        {
            StructureType=structureType;
            Sections = new List<SectionInfo>(sections);
        }        
    }
}
