using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace ConfigReader
{
    interface InfoAttribute
    {
        string GetOptionID(PropertyInfo property);
    }
}
