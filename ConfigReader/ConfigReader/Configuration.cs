using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Reflection;
using System.Reflection.Emit;

using System.IO;

using ConfigReader.Parsing;
using ConfigReader.ConfigCreation;

namespace ConfigReader
{
    /// <summary>
    /// Mode of configuration file parsing.
    /// </summary>
    public enum ParsingMode { 
        /// <summary>
        /// Requires strict match with structure specification.
        /// </summary>
        Strict, 
        /// <summary>
        /// Allows additional elements to be present in file.
        /// </summary>
        Relaxed 
    }

    /// <summary>
    /// Factory class for configuration objects.
    /// </summary>
    public static class Configuration
    {

        /// <summary>
        /// Create configuration object, which implements given Structure.
        /// Structure properties are initialized according to default values.
        /// </summary>
        /// <typeparam name="Structure">Interface which describes structure of configuration file.</typeparam>
        /// <returns>Configuration object.</returns>
        public static Structure CreateFromDefaults<Structure>()
            where Structure : IConfiguration
        {
            var parser = ConfigParser.ForWritingOnly();
            return createConfig<Structure>(parser, false);           
        }


        /// <summary>
        /// Create configuration object, which implements given Structure.
        /// Structure properties are parsed from file specified by configFilePath.
        /// </summary>
        /// <param name="configFilePath">Input file path from where configuration is parsed.</param>
        /// <param name="mode">Mode which is used for parsing input data.</param>
        /// <typeparam name="Structure">Interface which describes structure of configuration file.</typeparam>
        /// <returns>Configuration object.</returns>
        public static Structure CreateFromFile<Structure>(string configFilePath, ParsingMode mode)
            where Structure : IConfiguration
        {
            var parser=ConfigParser.FromFile(configFilePath,mode);
            return createConfig<Structure>(parser);            
        }


        /// <summary>
        /// Create configuration object, which implements given Structure.
        /// Structure properties are parsed from input stream.
        /// </summary>
        /// <param name="input">Input stream from where configuration is parsed.</param>
        /// <param name="mode">Mode which is used for parsing input data.</param>
        /// <typeparam name="Structure">Interface which describes structure of configuration file.</typeparam>
        /// <returns>Configuration object.</returns>
        public static Structure CreateFromStream<Structure>(StreamReader input, ParsingMode mode)
            where Structure : IConfiguration
        {
            var parser = ConfigParser.FromStream(input, mode);
            return createConfig<Structure>(parser);
        }


        /// <summary>
        /// Create configuration object, which implements given Structure.
        /// Structure properties are parsed from input stream.
        /// </summary>
        /// <param name="parser">Parser which is used for input/output.</param>
        /// <param name="readFromParser">Determine that values can be read from parser.</param>
        /// <typeparam name="Structure">Interface which describes structure of configuration file.</typeparam>
        /// <returns>Configuration object.</returns>
        private static Structure createConfig<Structure>(ConfigParser parser,bool readFromParser=true)
            where Structure : IConfiguration
        {
            var structureType = typeof(Structure);

            //throws exception for invalid structureType
            StructureValidation.ThrowOnInvalid(structureType);

            var structureInfo = StructureFactory.CreateStructureInfo(structureType);
            var optionValues =  readFromParser?parser.GetOptionValues(structureInfo):getDefaults(structureInfo);

        
            var configRoot = ConfigFactory.CreateConfigRoot(structureInfo);
            configRoot.AssociateParser(parser);
            //fill config with option values
            foreach (var value in optionValues)
            {
                configRoot.SetOption(value);
                if (!readFromParser)
                {
                    //parser doesn't know about default values - notify him
                    var info = configRoot.GetOptionInfo(value.Name);
                    parser.SetOption(info, value);
                }
            }

            return (Structure)(object)configRoot;
        }

        /// <summary>
        /// Collect all default values from given structure info.
        /// </summary>
        /// <param name="structureInfo">Structure info, which default values we want to obtain.</param>
        /// <returns>All option values with default value.</returns>
        private static IEnumerable<OptionValue> getDefaults(StructureInfo structureInfo)
        {
            foreach (var section in structureInfo.Sections)
            {
                foreach (var option in section.Options)
                {
                    if (option.DefaultValue==null)
                    {
                        continue;
                    }
                    yield return new OptionValue(option.Name, option.DefaultValue);
                }
            }
        }
    }
}
