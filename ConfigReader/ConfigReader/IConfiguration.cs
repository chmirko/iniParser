using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.IO;

using ConfigReader.Parsing;

namespace ConfigReader
{
    public interface IConfiguration
    {
        void Save(string outputFile);
        void WriteTo(StreamWriter output);
        void SetComment(QualifiedName name,string comment);
    }
}
