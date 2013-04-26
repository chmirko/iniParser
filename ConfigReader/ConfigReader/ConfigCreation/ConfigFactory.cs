using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



using ConfigReader.Parsing;

namespace ConfigReader.ConfigCreation
{
    static class ConfigFactory
    {

        internal static ConfigRoot CreateConfigRoot(StructureInfo structure)
        {
            var configRoot = createConfigRootRaw(structure);

            foreach (var section in structure.Sections)
            {
                var configSection = createConfigSection(section);
                configRoot.InsertSection(configSection);
            }
            return configRoot;
        }

        private static ConfigRoot createConfigRootRaw(StructureInfo structure)
        {
            var rootBuilder = new ClassBuilder<ConfigRoot>(structure.DescribingType);
            foreach (var section in structure.Sections)
            {
                rootBuilder.AddProperty(section.AssociatedProperty, section.DescribingType);
            }
            var type = rootBuilder.Build();

            var configRoot = (ConfigRoot)Activator.CreateInstance(type);
            return configRoot;
        }

        private static ConfigSection createConfigSection(SectionInfo info)
        {
            var sectionBuilder = new ClassBuilder<ConfigSection>(info.DescribingType);
            foreach (var option in info.Options)
            {
                sectionBuilder.AddProperty(option.AssociatedProperty, option.ExpectedType);
            }

            var type = sectionBuilder.Build();
            var section= (ConfigSection)Activator.CreateInstance(type);
            section.SetSectionInfo(info);

            return section;
        }
    }
}
