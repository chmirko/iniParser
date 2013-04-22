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

        public static Structure CreateFromFile<Structure>(string configFilePath, ConfigMode mode)
            where Structure : IConfig
        {

            var parser=ConfigParser.FromFile(configFilePath,mode);

            return createConfig<Structure>(parser);
        }

        public static Structure CreateFromStream<Structure>(StreamReader input,ConfigMode mode)
            where Structure : IConfig
        {
            var parser=ConfigParser.FromStream(input,mode);

            return createConfig<Structure>(parser);
        }

        private static Structure createConfig<Structure>(ConfigParser parser)
            where Structure : IConfig
        {
            var structureType = typeof(Structure);

            //throws exception for invalid structureType
            throwOnInvalid(structureType);

            var structureInfo = createStructureInfo(structureType);
            var optionValues = parser.GetOptionValues(structureInfo);
            var configRoot = createConfigRoot(structureType);

            //fill config with parsed values
            foreach (var value in optionValues)
            {
                configRoot.SetOption(value);
            }

            return (Structure)(object)configRoot;
        }

        private static StructureInfo createStructureInfo(Type structureType)
        {
            var sections = new List<SectionInfo>();
            foreach (var sectionProperty in getSectionProperties(structureType))
            {
                var sectionInfo = createSectionInfo(sectionProperty);
                sections.Add(sectionInfo);
            }

            return new StructureInfo(sections);
        }

        private static SectionInfo createSectionInfo(PropertyInfo sectionProperty)
        {
            var options = new List<OptionInfo>();
            var sectionName = sectionProperty.Name;
            foreach (var optionProperty in getOptionProperties(sectionProperty.PropertyType))
            {
                var optionInfo = createOptionInfo(sectionName,optionProperty);
                options.Add(optionInfo);
            }

            return new SectionInfo(sectionName,options,null);
        }

        private static OptionInfo createOptionInfo(string sectionName, PropertyInfo optionProperty)
        {
            var name = new QualifiedOptionName(sectionName, optionProperty.Name);
            var expectedType = optionProperty.PropertyType;
            return new OptionInfo(name, expectedType, null, null);
        }
            


        private static IEnumerable<PropertyInfo> getSectionProperties(Type structureType)
        {
            return structureType.GetProperties();
        }

        private static IEnumerable<PropertyInfo> getOptionProperties(Type sectionType)
        {
            return sectionType.GetProperties();
        }

        private static void throwOnInvalid(Type structureType)
        {
            //throw new NotImplementedException();
        }

        private static ConfigRoot createConfigRoot(Type structureType)
        {
            var configRoot = createConfigRootRaw(structureType);
            
            foreach (var section in createSectionHandlers(structureType))
            {
                configRoot.InsertSection(section);
            }
            return configRoot;
        }

        private static ConfigRoot createConfigRootRaw(Type structureType)
        {
            var rootBuilder = new ClassBuilder<ConfigRoot>(structureType);
            foreach (var prop in structureType.GetProperties())
            {
                rootBuilder.AddProperty(prop.Name, prop.PropertyType);
            }
            var type = rootBuilder.Build();

            var configRoot = (ConfigRoot)Activator.CreateInstance(type);
            return configRoot;
        }

        private static IEnumerable<SectionHandler> createSectionHandlers(Type structureType)
        {
            foreach (var sectionProperty in getSectionProperties(structureType))
            {
                var sectionData = createConfigSection(sectionProperty.PropertyType);
                yield return new SectionHandler(sectionProperty, sectionData);
            }
        }

        private static ConfigSection createConfigSection(Type sectionType)
        {
            var sectionBuilder = new ClassBuilder<ConfigSection>(sectionType);
            foreach (var prop in sectionType.GetProperties())
            {
                sectionBuilder.AddProperty(prop.Name, prop.PropertyType);
            }

            var type = sectionBuilder.Build();

            return (ConfigSection)Activator.CreateInstance(type);
        }

    }
}
