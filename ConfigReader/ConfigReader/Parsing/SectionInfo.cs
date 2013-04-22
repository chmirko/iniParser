using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.Parsing
{
    class SectionInfo
    {
        public readonly string Name;
        public readonly IEnumerable<OptionInfo> Options;
    }
}
