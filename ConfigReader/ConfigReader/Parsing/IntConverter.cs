using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConfigReader.Parsing
{
    class IntConverter:IValueConverter
    {
        public object Deserialize(string data)
        {
            return int.Parse(data);
        }

        public string Serialize(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
