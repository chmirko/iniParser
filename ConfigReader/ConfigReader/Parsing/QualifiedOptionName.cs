using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.Parsing
{
    sealed class QualifiedOptionName:QualifiedName
    {
        public readonly QualifiedSectionName Section;
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
