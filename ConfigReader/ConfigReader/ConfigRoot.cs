using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




using ConfigReader.Parsing;

namespace ConfigReader
{
    
    class ConfigRoot:PropertyStorage,IConfig
    {
        ConfigParser _parser;
        List<ConfigSection> _sections = new List<ConfigSection>();

        public IEnumerable<ConfigSection> Sections
        {
            get
            {
                return _sections;
            }
        }

        public void Save(string outputFile)
        {
            throw new NotImplementedException();
        }

        internal void InsertSection(SectionHandler section)
        {
            this.InitializeStoredProperty(section.Name, section.Storage);
            _sections.Add(section.Storage);
        }

        internal void SetParser(ConfigParser parser)
        {
            _parser = parser;
        }
    }
}
