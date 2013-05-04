using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ConfigRW;

namespace UsageExamples
{
    class Program
    {
        private const string rawConfig = @"
         [ParseSect_A]
         uncommentedOpt = dummyVal
         optSingleA = AlphaBrawoCharlieDeltaEchoFoxtrot    ;I HAZ COMMENTS
         optSingleB=\ \ \ Alpha   Brawo   Charlie   Delta   Echo   Foxtrot\ \ \ 
         optMultiA = Alpha,Brawo,Charlie,Delta,Echo,Foxtrot    ;I HAZ COMMENTS TOO
         optMultiB=Alpha:Brawo:Charlie:Delta:Echo:Foxtrot

         [ParseSect_B]
         uncommentedOpt=dummyVal ; HA! HAS COMMENT HERE, just must be everywhere as some dummyDeveloper made it non-optional
         optSingleA=${ParseSect_A#optSingleA}    ;I HAZ COMMENTS
         optSingleB = ${ParseSect_A#optSingleB}
         optMultiA=${ParseSect_A#optSingleA}, Golf, Hotel, India, Juliet    ;I HAZ COMMENTS TOO
         optMultiB = ${ParseSect_A#optMultiB},        Golf        ,Hotel, India, Juliet

         [ParseSect_C]
         uncommentedOpt=dummyVal
         abundant = Hello, What the heck am I doing here, whell, screwing your parsing process; have a nice exception catching day
         optMultiA = Heading, ${ParseSect_B#optMultiA}, kilo,       Lima, \ \ \ Mike\ \ \   ;I HAZ COMMENTS TOO
         optMultiB=HeadingHereToo:${ParseSect_B#optMultiB}::somethingMissingBeforeMe:kilo:Lima";

        /// <summary>
        /// Testing scenarios used during parser development
        /// </summary>
        private static void runParserTests()
        {
            // Prepare raw config
            System.IO.StreamWriter outfile = new System.IO.StreamWriter("raw.cfg");
            outfile.Write(rawConfig);
            outfile.Close();

            // Test saving from directlyCreated struct
            var parseConf_cleanStore = Configuration.CreateFromDefaults<ParserConfigStruct>();
            parseConf_cleanStore.SaveTo("parseTest_directlyStored.cfg");

            // Test default values not being saved after not present in original file (and other featurettes as well)
            var parseConf_readStore = Configuration.CreateFromFile<ParserConfigStruct>("raw.cfg", ParsingMode.Relaxed);
            parseConf_readStore.SaveTo("parseTest_defaultSaveTest.cfg");
        }

        static void Main(string[] args)
        {
            // parser testing
            runParserTests();

            // some other testing
            var config = Configuration.CreateFromDefaults<ConfigStructure>();
            config.Special.List.Add("test added");
            config.Sec1.settableNumber = 98765;
            config.SetComment(QualifiedName.ForSection("Sec2"), "Comment on section 2");
            config.SaveTo("defaultValues.cfg");

            var config2 = Configuration.CreateFromFile<ConfigStructure>("defaultValues.cfg", ParsingMode.Strict);
            config2.SetComment(QualifiedName.ForSection("Sec2"), "Comment override");
            config2.SetComment(QualifiedName.ForSection("SpecialTest"), "Comment added");
            config2.Special.List.Add("test changed");

            config2.SaveTo("changedConfig.cfg");
        }
    }

    /// <summary>
    /// Root configuration structure.
    /// </summary>
    public interface ConfigStructure : IConfiguration
    {
        [SectionInfo(ID = "SpecialTest")]
        SpecialTypeSection Special { get; }

        [DefaultComment("Second section of config, default comment")]
        //Set special ID for section
        [SectionInfo(ID = "Special $ - name")]
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
        [Range(LowerBound = 20)]
        [OptionInfo(DefaultValue = 20)]
        int number { get; }
        [DefaultComment("Value that can be set at runtime")]
        int settableNumber { get; set; }
    }

    public interface Section2
    {        
        [OptionInfo(ID = "Name with space", IsOptional = true, DefaultValue = "default value")]
        string Field_name { get; }

        [OptionInfo(ID = "duplTest2", DefaultValue = 4)]
        int x2 { get; }
        [OptionInfo(ID = "duplTest", DefaultValue = 7)]
        int x1 { get; }
    }

    /// <summary>
    /// Configuration structure used for parser testing
    /// </summary>
    public interface ParserConfigStruct : IConfiguration
    {
        [DefaultComment("Section used as parser tester - ALPHA")]
        ParserTestSection ParseSect_A { get; }

        [DefaultComment("Section used as parser tester - BRAVO")]
        ParserTestSection ParseSect_B { get; }

        [DefaultComment("Section used as parser tester - CHARLIE")]
        ParserTestSection ParseSect_C { get; }
    }

    /// <summary>
    /// Section for pure parser & lexer testing
    /// </summary>
    public interface ParserTestSection
    {
        [OptionInfo(ID = "uncommentedOpt", IsOptional = false, DefaultValue = "dummyVal")]
        string uncommentedOpt { get; }

        [OptionInfo(ID = "defaultOpt", IsOptional = true, DefaultValue = "THIS SHOULD NOT BE SEEN IN THE CONFIG")]
        [DefaultComment("test for default option")]
        string defaultOpt { get; }

        [OptionInfo(ID = "optSingleA", IsOptional = true, DefaultValue = "default")]
        [DefaultComment("I HAZ COMMENTS")]
        string optSingleA { get; set; }

        [OptionInfo(ID = "optSingleB", IsOptional = true, DefaultValue = "default")]
        string optSingleB { get; set; }

        [OptionInfo(ID = "optMultiA", IsOptional = true, DefaultValue = new string[] { "default1", "default2", "default3" })]
        [DefaultComment("I HAZ COMMENTS TOO")]
        List<string> optMultiA { get; set; }

        [OptionInfo(ID = "optMultiB", IsOptional = true, DefaultValue = new string[] { "default1", "default2", "default3" })]
        List<string> optMultiB { get; set; }
    }

    public enum EnumTest
    {
        Value1,
        Value2,
        ValueLast = 134
    }
}
