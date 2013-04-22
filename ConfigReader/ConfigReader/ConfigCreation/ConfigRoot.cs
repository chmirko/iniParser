using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConfigReader.Parsing;

namespace ConfigReader.ConfigCreation
{
    
    class ConfigRoot:PropertyStorage,IConfig
    {
        ConfigParser _parser;
        Dictionary<QualifiedSectionName,ConfigSection> _sections = new Dictionary<QualifiedSectionName,ConfigSection>();

        public IEnumerable<ConfigSection> Sections
        {
            get
            {
                return _sections.Values;
            }
        }
            
        internal void InsertSection(SectionHandler section)
        {
            this.InitializeStoredProperty(section.Name.ID, section.Storage);
            _sections.Add(section.Name,section.Storage);
        }

        internal void SetParser(ConfigParser parser)
        {
            _parser = parser;
        }

        internal void SetOption(OptionValue value)
        {
            var sectionName = value.Name.Section;
            var optionName = value.Name.ID;

            _sections[sectionName].InitializeStoredProperty(optionName, value.ConvertedValue);
        }

        public void Save(string outputFile)
        {
            throw new NotImplementedException();
        }


        public void SetComment(QualifiedName comment)
        {
            throw new NotImplementedException();
        }
    }
}
