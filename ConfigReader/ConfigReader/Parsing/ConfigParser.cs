using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace ConfigReader.Parsing
{
    internal class ConfigParser
    {
        /// <summary>
        /// Configuration mode.
        /// </summary>
        private readonly ConfigMode _mode;

        /// <summary>
        /// Creates parser for configuration at given path.
        /// NOTE: Is good to copy available converters (they can be changed, but it should'nt affect created instance)
        /// </summary>
        /// <param name="configFilePath"></param>
        private ConfigParser(ConfigMode mode)
        {            
            _mode = mode;
        }

        /// <summary>
        /// Set value converter for given type. (Can override default value converter)
        /// </summary>
        /// <param name="converter"></param>
        /// <param name="type"></param>
        static internal void SetConverter(Type type,IValueConverter converter){
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove converter associted with given type. (Can remove default value converter)
        /// </summary>
        /// <param name="type"></param>
        static internal void RemoveConverter(Type type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create config parser from given stream.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        static internal ConfigParser FromStream(StreamReader input, ConfigMode mode)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            var parser = new ConfigParser(mode);
            parser.readConfig(input);

            return parser;
        }

        static internal ConfigParser FromFile(string configFilePath,ConfigMode mode)
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
        internal void WriteTo(string outputFile)
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
        /// Get current option values. Options validity is checked here.
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


        /// <summary>
        /// Overrides default and parsed comment.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="comment"></param>
        internal void SetComment(QualifiedName name, string comment)
        {
            throw new NotImplementedException();
        }
    }
}
