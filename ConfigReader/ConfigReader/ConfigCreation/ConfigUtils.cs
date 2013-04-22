using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.ConfigCreation
{
    static class ConfigUtils
    {

        internal static ConfigRoot CreateConfigRoot(Type structureType)
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
            foreach (var sectionProperty in InfoUtils.GetSectionProperties(structureType))
            {
                var sectionData = createConfigSection(sectionProperty.PropertyType);
                yield return new SectionHandler(sectionProperty, sectionData);
            }
        }

        private static ConfigSection createConfigSection(Type sectionType)
        {
            var sectionBuilder = new ClassBuilder<ConfigSection>(sectionType);
            foreach (var optionProperty in InfoUtils.GetOptionProperties(sectionType))
            {
                sectionBuilder.AddProperty(optionProperty.Name, optionProperty.PropertyType);
            }

            var type = sectionBuilder.Build();

            return (ConfigSection)Activator.CreateInstance(type);
        }
    }
}
