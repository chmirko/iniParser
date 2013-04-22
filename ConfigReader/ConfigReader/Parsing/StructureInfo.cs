using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.Parsing
{
    class StructureInfo
    {
        private readonly List<SectionInfo> _sections;

        internal readonly ConfigMode Mode;

        internal IEnumerable<SectionInfo> Sections
        {
            get
            {
                return _sections;
            }
        }

        internal StructureInfo(IEnumerable<SectionInfo> sections, ConfigMode mode)
        {
            Mode = mode;
            _sections.AddRange(sections);
        }

    }
}
