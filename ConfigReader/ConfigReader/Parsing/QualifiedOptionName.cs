using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConfigReader.Parsing
{
    /// <summary>
    /// Represents config scope unique name of option.
    /// NOTE: Can be used in natural way as key in hash containers.
    /// </summary>
    sealed class QualifiedOptionName:QualifiedName
    {
        /// <summary>
        /// Qualified name of section, where this option is defined.
        /// </summary>
        public readonly QualifiedSectionName Section;
        /// <summary>
        /// ID of option. This ID is used in input/output file.
        /// </summary>
        public readonly string ID;

        public QualifiedOptionName(QualifiedSectionName section,string optionID) {
            ID= optionID;
            Section = section;
        }
        
        internal override string toString()
        {
            return string.Format("{0}::{1}", Section.ID, ID);
        }
    }
}
