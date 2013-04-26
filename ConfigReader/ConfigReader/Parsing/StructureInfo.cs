using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConfigReader.Parsing
{
    /// <summary>
    /// Info describing connfig structure.
    /// </summary>
    class StructureInfo
    {
        /// <summary>
        /// Sections that are present in config.
        /// </summary>
        internal readonly IEnumerable<SectionInfo> Sections;
        /// <summary>
        /// Type that describes config structure.
        /// </summary>
        internal readonly Type DescribingType;

        internal StructureInfo(Type describingType, IEnumerable<SectionInfo> sections)
        {
            DescribingType=describingType;
            Sections = new List<SectionInfo>(sections);
        }        
    }
}
