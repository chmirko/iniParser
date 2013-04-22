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
            
        internal void InsertSection(ConfigSection section)
        {
            this.DirectPropertySet(section.AssociatedProperty,section);
            _sections.Add(section.Name,section);
        }

        internal void SetParser(ConfigParser parser)
        {
            _parser = parser;
        }

        internal void SetOption(OptionValue value)
        {
            var optionName = value.Name;
            var sectionName = optionName.Section;
            

            _sections[sectionName].SetOption(optionName, value.ConvertedValue);
        }

        public void Save(string outputFile)
        {
            foreach (var section in _sections.Values)
            {
                foreach (var changedOption in section.ChangedOptions)
                {
                    var optionInfo = section.GetOptionInfo(changedOption.Name);
                    _parser.SetOption(optionInfo,changedOption);
                }

                section.ClearOptionChangeLog();
            }
            _parser.WriteTo(outputFile);
        }


        public void SetComment(QualifiedName comment)
        {
            throw new NotImplementedException();
        }
    }
}
