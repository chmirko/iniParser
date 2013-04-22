using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Reflection.Emit;

using System.IO;

using ConfigReader.Parsing;
using ConfigReader.ConfigCreation;

namespace ConfigReader
{

    public enum ConfigMode { Strict, Relaxed }


    internal delegate string IDResolver(PropertyInfo property);

    public static class Configuration
    {
        public static Structure CreateFromDefaults<Structure>()
        {
            throw new NotImplementedException();
        }

        public static Structure CreateFromFile<Structure>(string configFilePath, ConfigMode mode)
            where Structure : IConfiguration
        {
            //       var parser=ConfigParser.FromFile(configFilePath,mode);
            //       return createConfig<Structure>(parser);
            return createConfig<Structure>(null);
        }

        public static Structure CreateFromStream<Structure>(StreamReader input, ConfigMode mode)
            where Structure : IConfiguration
        {
            var parser = ConfigParser.FromStream(input, mode);

            return createConfig<Structure>(parser);
        }

        private static Structure createConfig<Structure>(ConfigParser parser)
            where Structure : IConfiguration
        {
            var structureType = typeof(Structure);

            //throws exception for invalid structureType
            StructureValidationUtils.ThrowOnInvalid(structureType);

            var structureInfo = InfoUtils.CreateStructureInfo(structureType);
            // var optionValues = parser.GetOptionValues(structureInfo);
            var optionValues = getTestValues();

            var configRoot = ConfigUtils.CreateConfigRoot(structureInfo);
            //fill config with parsed values
            foreach (var value in optionValues)
            {
                configRoot.SetOption(value);
            }

            return (Structure)(object)configRoot;
        }


        #region DEBUG ONLY
        private static IEnumerable<OptionValue> getTestValues()
        {
            var values = new List<OptionValue>(){
                createValue("Sec1","number",12345),
                createValue("Sec1","settableNumber",4389),
                createValue("Sec2","Name with space","setted value by config")
            };

            return values;
        }

        private static OptionValue createValue(string section, string option, object value)
        {
            return new OptionValue(new QualifiedOptionName(new QualifiedSectionName(section), option), value);
        }
        #endregion


      
    }
}
