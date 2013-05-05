using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Reflection;
using System.Reflection.Emit;

using System.IO;

using ConfigRW.Parsing;
using ConfigRW.ConfigCreation;

namespace ConfigRW
{
    /// <summary>
    /// Mode of configuration file parsing.
    /// </summary>
    public enum ParsingMode
    {
        /// <summary>
        /// Requires strict match with structure description.
        /// </summary>
        Strict,
        /// <summary>
        /// Allows additional elements (those that wasn't specified in structure description) to be present in file.
        /// </summary>
        Relaxed
    }

    /// <summary>
    /// CONFIGURATION CLASS 
    ///     provides possibility of creating configuration objects. These objects let you
    ///     manipulate with configuration file content. Configuration object is specified by structure description
    ///     in form of .NET public interface. Configuration object than implements this interface and is mapped on content
    ///     of configuration file.
    ///  
    /// Configuration class provides three ways how to instantiate configuration object.
    /// * Firstly we can create configuration object from default values that are available in structure description.
    ///     NOTE: when you don't specify default value for non-optional option, you can create configuration file, 
    ///     with prepared required option lines, without any values. It's especially usefull when you want force config file user to fill some required values.
    /// 
    /// * Next way how you can create configuration object is from configuration file.
    ///     Library provides you Strict and Relaxed mode of parsing. They differ in need to exactly met structure description in configuration file.
    ///     
    /// * Last way how you can create configuration object is from specified StreamReader.
    ///     This is usefull when you need to work with configuration "file" in memory.
    ///     
    /// STRUCTURE DESCRIPTION
    ///     Structure of configuration files and their matching configuration objects is
    ///     described by .NET interfaces. Those interfaces has to be public, because our library
    ///     needs to implement them at runtime. Structure description allows natural constructs how
    ///     to specify format of configuration file. For getting started with structure descriptions
    ///     we recommend to see usage documentation of our library with examples.
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
        public static Structure CreateFromFile<Structure>(string configFilePath, ParsingMode mode = ParsingMode.Strict)
            where Structure : IConfiguration
        {
            var parser = ConfigParser.FromFile(configFilePath, mode);
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
        public static Structure CreateFromStream<Structure>(StreamReader input, ParsingMode mode = ParsingMode.Strict)
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
        private static Structure createConfig<Structure>(ConfigParser parser, bool readFromParser = true)
            where Structure : IConfiguration
        {
            var structureType = typeof(Structure);

            //throws exception for invalid structureType
            StructureValidation.ThrowOnInvalid(structureType);

            var structureInfo = StructureFactory.CreateStructureInfo(structureType);
            var optionValues = readFromParser ? parser.GetOptionValues(structureInfo) : getDefaults(structureInfo);
            if (!readFromParser)
            {
                //parser has to now about structure 
                parser.RegisterStructure(structureInfo);
            }


            var configRoot = ConfigFactory.CreateConfigRoot(structureInfo);
            configRoot.AssociateParser(parser);
            //fill config with option values
            foreach (var value in optionValues)
            {
                configRoot.SetOption(value);
                if (!readFromParser)
                {
                    //parser doesn't know about default values - notify it
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
                    if (option.DefaultValue == null)
                    {
                        continue;
                    }
                    yield return new OptionValue(option.Name, option.DefaultValue);
                }
            }
        }
    }
}
