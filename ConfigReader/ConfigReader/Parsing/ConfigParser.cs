using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.IO;

namespace ConfigReader.Parsing
{
    internal class ConfigParser
    {
        /// <summary>
        /// Configuration mode.
        /// </summary>
        private readonly ParsingMode _mode;

        /// <summary>
        /// Creates parser for configuration at given path.
        /// </summary>
        /// <param name="configFilePath"></param>
        private ConfigParser(ParsingMode mode)
        {            
            _mode = mode;
        }

        /// <summary>
        /// Create parser which will be used only for writing default values.
        /// All values that should be written will be set via SetOption
        /// </summary>
        /// <returns></returns>
        static internal ConfigParser ForWritingOnly()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create config parser from given stream.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        static internal ConfigParser FromStream(StreamReader input, ParsingMode mode)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            var parser = new ConfigParser(mode);
            parser.readConfig(input);

            return parser;
        }

        static internal ConfigParser FromFile(string configFilePath,ParsingMode mode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stream can be read only here.
        /// </summary>
        /// <param name="input"></param>
        private void readConfig(StreamReader input)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write parsed and changed options into output file
        /// </summary>
        /// <param name="outputFile"></param>
        internal void Save(string outputFile)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write parsed and changed options into output stream
        /// </summary>
        /// <param name="output"></param>
        internal void WriteTo(StreamWriter output)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Get current option values. Options validity is checked here. Returns only options which were present in file or options that were passed through SetOption.
        /// </summary>
        /// <param name="structure"></param>        
        /// <returns></returns>
        internal IEnumerable<OptionValue> GetOptionValues(StructureInfo structure)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set option info and it's value.
        /// NOTE: Only options which are set via this call can be ADDED into output (readed options are included automatically).
        /// </summary>
        /// <param name="info"></param>
        /// <param name="value"></param>
        internal void SetOption(OptionInfo info, OptionValue value)
        {
            throw new NotImplementedException();
        }

        Dictionary<QualifiedName, string> _comments = new Dictionary<QualifiedName, string>();

        /// <summary>
        /// Overrides default and parsed comment.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="comment"></param>
        internal void SetComment(QualifiedName name, string comment)
        {
            _comments[name] = comment;
            throw new NotImplementedException();
        }

        
    }
}
