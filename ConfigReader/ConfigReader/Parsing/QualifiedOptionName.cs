using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.Parsing
{
    struct QualifiedOptionName
    {
        public readonly string Option;
        public readonly string Section;

        public QualifiedOptionName(string section,string option) {
            Option = option;
            Section = section;
        }
        
        public override bool Equals(object obj)
        {            
            return ToString()==obj.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}::{1}", Section, Option);
        }
    }
}
