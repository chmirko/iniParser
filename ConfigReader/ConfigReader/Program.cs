using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

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
            
            config.Save("changedConfig.cfg");


            var memStream=new MemoryStream();
            var writer = new StreamWriter(memStream);
            config.WriteTo(writer);

            Console.WriteLine(Encoding.Default.GetString(memStream.ToArray()));
               
        }
    }

    

    interface ConfigStructure:IConfiguration
    {
        [DefaultComment("First section of config")]
        Section1 Sec1 { get; }
        [DefaultComment("Second section of config")]
        Section2 Sec2 { get; }
    }

    /// <summary>
    /// Test for non-trivial type conversions
    /// </summary>
    interface SpecialTypeSection
    {
        IEnumerable<string> Enumerable { get; }

        List<string> List { get; }

        string[] Array { get; }

        EnumTest Enum { get; }

        HashSet<EnumTest> EnumSet { get; }
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

        [OptionInfo(ID="duplTest2")]
        int x2 { get; }
        [OptionInfo(ID="duplTest")]
        int x1 { get; }
    }

    enum EnumTest
    {
        Value1,
        Value2,
        ValueLast=134
    }
}
