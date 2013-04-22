using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.Parsing
{
    sealed class QualifiedSectionName:QualifiedName
    {
        public readonly string ID;

        public QualifiedSectionName(string name)
        {
            ID = name;
        }
        internal override string toString()
        {
            return ID;
        }
    }
}
