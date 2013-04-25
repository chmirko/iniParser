using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.Parsing
{
    interface IValueConverter
    {
        object Deserialize(string data);
        string Serialize(object obj);
    }
}
