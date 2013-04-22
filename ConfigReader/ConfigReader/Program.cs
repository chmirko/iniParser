using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader
{

    delegate void IntSetter(int value);

    class Program
    {
        static void Main(string[] args)
        {
            var config = Configuration.CreateFromFile<ConfigStructure>("config.cfg",ConfigMode.Strict);

            var x = config.Sec1;

            config.Sec1.settableNumber = 4;
            config.Sec1.settableNumber = 42;
            
            config.Save("chagedConfig.cfg");            
        }
    }

    interface ConfigStructure:IConfig
    {
        [DefaultComment("First section of config")]
        Section1 Sec1 { get; }
        [DefaultComment("Second section of config")]
        Section2 Sec2 { get; }
    }

    interface Section1
    {        
        int number { get; }
        [DefaultComment("Value that can be set at runtime")]
        int settableNumber { get; set; }
    }

    interface Section2
    {
        [OptionInfo(ID="Name with space",IsOptional=true, DefaultValue="default value")]
        string Field_name { get; }
    }
}
