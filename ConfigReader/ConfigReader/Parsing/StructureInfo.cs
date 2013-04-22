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

        internal StructureInfo(IEnumerable<SectionInfo> sections)
        {
            Sections = new List<SectionInfo>(sections);
        }

    }
}
