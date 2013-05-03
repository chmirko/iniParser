using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ConfigRW;

namespace UsageExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = Configuration.CreateFromDefaults<ConfigStructure>();
            config.Special.List.Add("test added");
            config.Sec1.settableNumber = 98765;
            config.Save("defaultValues.cfg");



            var config2 = Configuration.CreateFromFile<ConfigStructure>("defaultValues.cfg", ParsingMode.Strict);
            config2.SetComment(QualifiedName.ForSection("Sec1"), "Comment override");
            config2.SetComment(QualifiedName.ForSection("Special"), "Comment added");
            config2.Special.List.Add("test changed");

            config2.Save("changedConfig.cfg");
        }
    }


    /// <summary>
    /// Root configuration structure.
    /// </summary>
    public interface ConfigStructure : IConfiguration
    {
        SpecialTypeSection Special { get; }

        [DefaultComment("First section of config, default comment")]
        Section1 Sec1 { get; }
        [DefaultComment("Second section of config")]
        Section2 Sec2 { get; }
    }

    /// <summary>
    /// Test for non-trivial type conversions
    /// </summary>
    public interface SpecialTypeSection
    {
        [OptionInfo(DefaultValue = new string[] { "test1", "test2" })]
        string[] Array { get; }
        [OptionInfo(DefaultValue = new string[] { "test2", "test3" })]
        IEnumerable<string> Enumerable { get; }

        [OptionInfo(DefaultValue = new string[] { "value1", "value2", "valu3" })]
        List<string> List { get; }


        [OptionInfo(DefaultValue = EnumTest.ValueLast)]
        EnumTest Enum { get; }

        [OptionInfo(IsOptional = true, DefaultValue = new EnumTest[] { EnumTest.Value1, EnumTest.Value2 })]
        List<EnumTest> EnumSet { get; }
    }

    public interface Section1
    {
        [Range(LowerBound = 0)]
        int number { get; }
        [DefaultComment("Value that can be set at runtime")]
        int settableNumber { get; set; }
    }

    public interface Section2
    {
        [OptionInfo(ID = "Name with space", IsOptional = true, DefaultValue = "default value")]
        string Field_name { get; }

        [OptionInfo(ID = "duplTest2")]
        int x2 { get; }
        [OptionInfo(ID = "duplTest")]
        int x1 { get; }
    }

    public enum EnumTest
    {
        Value1,
        Value2,
        ValueLast = 134
    }
}
