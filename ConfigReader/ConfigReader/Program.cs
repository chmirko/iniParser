using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader
{

    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigCreator.FromFile<ConfigStructure>("config.cfg",ConfigMode.Strict);

            var x = config.Sec1;
            

            config.Sec1.settableNumber = 4;
            config.Save("chagedConfig.cfg");
        }
    }

    interface ConfigStructure:IConfig
    {
        Section1 Sec1 { get; }
        Section2 Sec2 { get; }
    }

    interface Section1
    {
        int number { get; }
        int settableNumber { get; set; }
    }

    interface Section2
    {
    }
}
