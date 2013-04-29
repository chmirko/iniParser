using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ConfigReader.Parsing;

namespace ConfigReader.ConfigCreation
{
    /// <summary>
    /// Factory for config.
    /// </summary>
    static class ConfigFactory
    {
        /// <summary>
        /// Create root of configuration structure.
        /// </summary>
        /// <param name="structure">Info about configuration structure.</param>
        /// <returns>Configuration structure root.</returns>
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

        /// <summary>
        /// Create raw (without filled sections) ConfigRoot object.
        /// </summary>
        /// <param name="structure">Info about configuration structure.</param>
        /// <returns>Configuration structure root without filled sections.</returns>
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
        /// <summary>
        /// Create configuration section object.
        /// </summary>
        /// <param name="info">Info about section structure.</param>
        /// <returns>Configuration section object.</returns>
        private static ConfigSection createConfigSection(SectionInfo info)
        {
            var sectionBuilder = new ClassBuilder<ConfigSection>(info.DescribingType);
            foreach (var option in info.Options)
            {
                sectionBuilder.AddProperty(option.AssociatedProperty, option.ExpectedType);
            }

            var type = sectionBuilder.Build();
            var section = (ConfigSection)Activator.CreateInstance(type);
            section.InitializeSectionInfo(info);

            return section;
        }
    }
}
