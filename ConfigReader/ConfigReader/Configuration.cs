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

    public static class Configuration
    {
        public static Structure CreateFromDefaults<Structure>()
        {
            throw new NotImplementedException();
        }

        public static Structure CreateFromFile<Structure>(string configFilePath, ConfigMode mode)
            where Structure : IConfig
        {
            //       var parser=ConfigParser.FromFile(configFilePath,mode);
            //       return createConfig<Structure>(parser);
            return createConfig<Structure>(null);
        }

        public static Structure CreateFromStream<Structure>(StreamReader input, ConfigMode mode)
            where Structure : IConfig
        {
            var parser = ConfigParser.FromStream(input, mode);

            return createConfig<Structure>(parser);
        }

        private static Structure createConfig<Structure>(ConfigParser parser)
            where Structure : IConfig
        {
            var structureType = typeof(Structure);

            //throws exception for invalid structureType
            throwOnInvalid(structureType);

            var structureInfo = InfoUtils.CreateStructureInfo(structureType);
            var optionValues = parser.GetOptionValues(structureInfo);

            var configRoot = ConfigUtils.CreateConfigRoot(structureType);
            //fill config with parsed values
            foreach (var value in optionValues)
            {
                configRoot.SetOption(value);
            }

            return (Structure)(object)configRoot;
        }


        private static void throwOnInvalid(Type structureType)
        {
            //throw new NotImplementedException();
        }
    }
}
