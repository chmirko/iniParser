using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.IO;

using ConfigReader.Parsing;

namespace ConfigReader.ConfigCreation
{
    
    class ConfigRoot:PropertyStorage,IConfiguration
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
            flushChanges();
            _parser.Save(outputFile);
        }

        public void WriteTo(StreamWriter output)
        {
            flushChanges();
            _parser.WriteTo(output);
        }

        internal OptionInfo GetOptionInfo(QualifiedOptionName name)
        {
            return _sections[name.Section].GetOptionInfo(name);
        }

        private void flushChanges()
        {
            foreach (var section in _sections.Values)
            {
                foreach (var changedOption in section.ChangedOptions)
                {
                    var optionInfo = GetOptionInfo(changedOption.Name);
                    _parser.SetOption(optionInfo, changedOption);
                }

                section.ClearOptionChangeLog();
            }
        }

        public void SetComment(QualifiedName name,string comment)
        {
            //TODO what if name doesn't exists ?
            _parser.SetComment(name, comment);
        }
    }
}
