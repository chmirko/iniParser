using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.IO;

using ConfigRW.Parsing;

namespace ConfigRW.ConfigCreation
{
    /// <summary>
    /// Root for configuration structure.
    /// </summary>
    class ConfigRoot : PropertyStorage, IConfiguration
    {
        /// <summary>
        /// Parser associated with configuration root.
        /// </summary>
        ConfigParser _parser;
        /// <summary>
        /// Sections that are present in configuration root.
        /// </summary>
        Dictionary<QualifiedSectionName, ConfigSection> _sections = new Dictionary<QualifiedSectionName, ConfigSection>();


        #region Public API implementation

        /// <summary>
        /// Save configuration into given file.
        /// </summary>
        /// <param name="outputFile">File where configuration will be saved.</param>
        public void Save(string outputFile)
        {
            flushChanges();
            _parser.Save(outputFile);
        }

        /// <summary>
        /// Write configuration into given output.
        /// </summary>
        /// <param name="output">Output where configuration will be written.</param>
        public void WriteTo(StreamWriter output)
        {
            flushChanges();
            _parser.WriteTo(output);
        }

        /// <summary>
        /// Set comment to element with specified name.
        /// </summary>
        /// <param name="name">Name of element.</param>
        /// <param name="comment">Comment that will be set to element.</param>
        public void SetComment(QualifiedName name, string comment)
        {            
            _parser.SetComment(name, comment);
        }

        #endregion


        /// <summary>
        /// Insert section into configuration structure.
        /// </summary>
        /// <param name="section">Section that will be inserted.</param>
        internal void InsertSection(ConfigSection section)
        {
            this.DirectPropertySet(section.AssociatedProperty, section);
            _sections.Add(section.Name, section);
        }

        /// <summary>
        /// Associate parser with configuration structure.
        /// NOTE: It's not passed through constructor, because this type is dynamically instantied.
        /// </summary>
        /// <param name="parser">Parser that will be associated.</param>
        internal void AssociateParser(ConfigParser parser)
        {
            _parser = parser;
        }
     
        /// <summary>
        /// Get option info for given name.
        /// </summary>
        /// <param name="name">Option name.</param>
        /// <returns>Option info.</returns>
        internal OptionInfo GetOptionInfo(QualifiedOptionName name) 
        {
            return _sections[name.Section].GetOptionInfo(name);
        }

        /// <summary>
        /// Set option value.
        /// </summary>
        /// <param name="value">Value to be set.</param>
        internal void SetOption(OptionValue value)
        {
            var optionName = value.Name;
            var sectionName = optionName.Section;

            _sections[sectionName].SetOption(optionName, value.ConvertedValue);
        }

  
        /// <summary>
        /// Flush changes into associated parser.
        /// </summary>
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
    }
}
