using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConfigReader.Parsing;

namespace ConfigReader
{
    public abstract class QualifiedName
    {
        internal abstract string toString();

        public static QualifiedName ForSection(string sectionID)
        {
            return new QualifiedSectionName(sectionID);
        }

        public static QualifiedName ForOption(string sectionID, string optionID)
        {
            var sectionName = new QualifiedSectionName(sectionID);
            return new QualifiedOptionName(sectionName, optionID);
        }

        public override bool Equals(object obj)
        {
            return ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return toString();
        }
    }
}
