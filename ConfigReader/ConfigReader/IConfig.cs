using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConfigReader.Parsing;

namespace ConfigReader
{
    public interface IConfig
    {
        void Save(string outputFile);        
    }
}
