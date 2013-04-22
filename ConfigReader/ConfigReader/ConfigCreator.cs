using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConfigReader.Parsing;
using System.Reflection;
using System.Reflection.Emit;

using System.IO;

namespace ConfigReader
{

    public enum ConfigMode { Strict, Relaxed }

    public static class ConfigCreator
    {

        public static ConfigStructure FromFile<ConfigStructure>(string configFilePath, ConfigMode mode)
            where ConfigStructure : IConfig
        {

            var parser=ConfigParser.FromFile(configFilePath,mode);

            return createConfig<ConfigStructure>(parser);
        }

        public static ConfigStructure FromStream<ConfigStructure>(StreamReader input,ConfigMode mode)
        {
            var parser=ConfigParser.FromStream(input,mode);

            return createConfig<ConfigStructure>(parser);
        }

        private static ConfigStructure createConfig<ConfigStructure>(ConfigParser parser)
        {
            var structureType = typeof(ConfigStructure);

            //throws exception for invalid structureType
            throwOnInvalid(structureType);

            


            var configRoot = createConfigRoot<ConfigStructure>(structureType);
            var internalConfigRoot = configRoot as ConfigRoot;
            foreach (var section in getSections(structureType))
            {
                internalConfigRoot.InsertSection(section);
            }

            throw new NotImplementedException();


            return configRoot;
        }

        private static void fillFromFile(string configFilePath, ConfigMode mode, ConfigRoot internalConfigRoot)
        {
            var parser = ConfigParser.FromFile(configFilePath,mode);
            internalConfigRoot.SetParser(parser);

            foreach (var section in internalConfigRoot.Sections)
            {
                fillFromParser(parser, section);
            }

        }

        private static void fillFromParser(ConfigParser parser, ConfigSection section)
        {
            foreach (var storedProperty in section.StoredProperties)
            {

            }
        }

        private static IEnumerable<SectionHandler> getSections(Type structureType)
        {
            foreach (var sectionProperty in structureType.GetProperties())
            {
                var sectionData = createConfigSection(sectionProperty.PropertyType);

                yield return new SectionHandler(sectionProperty, sectionData);
            }
        }



        private static void throwOnInvalid(Type structureType)
        {
            //throw new NotImplementedException();
        }

        private static ConfigStructure createConfigRoot<ConfigStructure>(Type structureType)
        {
            var structureBuilder = new ClassBuilder<ConfigRoot>(structureType);
            foreach (var prop in structureType.GetProperties())
            {
                structureBuilder.AddProperty(prop.Name, prop.PropertyType);
            }
            var type = structureBuilder.Build();

            var configStructure = (ConfigStructure)Activator.CreateInstance(type);
            return configStructure;
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
