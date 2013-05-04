using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConfigRW.Parsing
{
    /// <summary>
    /// Represents config scope unique name of section.
    /// NOTE: Can be used in natural way as key in hash containers.
    /// </summary>
    sealed class QualifiedSectionName:QualifiedName
    {
        /// <summary>
        /// ID of section. This ID is used in input/output file.
        /// </summary>
        internal readonly string ID;

        internal QualifiedSectionName(string name)
        {
            ID = name;
        }
        internal override string toString()
        {
            return ID;
        }
    }
}
