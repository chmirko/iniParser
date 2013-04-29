using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.IO;

namespace ConfigReader.Parsing
{
   /// <summary>
   /// Class representing config parser,
   /// </summary>
   internal sealed class ConfigParser
   {
      /// <summary>
      /// Configuration Parsing mode.
      /// </summary>
      private readonly ParsingMode _mode;

      /// <summary>
      /// Creates parser for configuration at given path.
      /// </summary>
      /// <param name="mode">Desired Configuration Parsing mode</param>
      private ConfigParser(ParsingMode mode)
      {
         _mode = mode;
      }

      /// <summary>
      /// Create parser which will be used only for writing default values.
      /// All values that should be written will be set via SetOption
      /// </summary>
      /// <returns>Parser to be used for writing default values</returns>
      static internal ConfigParser ForWritingOnly()
      {
         // For writer only, the mode is irrelevnat, chooseing Strict is less complex than creating new branch for writer
         return new ConfigParser(ParsingMode.Strict);
      }

      /// <summary>
      /// Create config parser, which parses given stream
      /// </summary>
      /// <param name="input">Stream to be parsed</param>
      /// <param name="mode">Desired Configuration Parsing Node</param>
      /// <returns>Parser which parses given stream</returns>
      static internal ConfigParser FromStream(StreamReader input, ParsingMode mode)
      {
         if (input == null)
         {
            throw new ParserException(
               userMsg: "Parser failed due to wrong usage", 
               developerMsg: "ConfigParser::FromStream called with null argument in place of input StreamReader", 
               inner: new ArgumentNullException("input"));
         }

         var parser = new ConfigParser(mode);
         parser.readConfig(input);

         return parser;
      }

      /// <summary>
      /// Create Config Parser, which parses given file
      /// </summary>
      /// <param name="configFilePath">Path to file which is to be parsed</param>
      /// <param name="mode"></param>
      /// <returns></returns>
      static internal ConfigParser FromFile(string configFilePath, ParsingMode mode)
      {
         if (configFilePath == null)
         {
            throw new ParserException(
               userMsg: "Parser failed due to wrong usage",
               developerMsg: "ConfigParser::FromFile called with null argument in place of input StreamReader",
               inner: new ArgumentNullException("input"));
         }

         StreamReader reader;

         try
         {
            reader = new StreamReader(configFilePath);
         }
         catch (Exception ex)
         {
            throw new ParserException(
               userMsg: "Parser failed when accessing fileSystem",
               developerMsg: "ConfigParser::FromFile problem occured when opening file",
               inner: ex);
         }

         ConfigParser parser = FromStream(reader, mode);

         reader.Close();

         return parser;

      }

      /// <summary>
      /// Read configuration data from given stream
      /// </summary>
      /// <param name="input">Stream from which data is to be read</param>
      private void readConfig(StreamReader input)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Write parsed and changed options into output file
      /// </summary>
      /// <param name="outputFile">Path to the oputput file</param>
      internal void Save(string outputFile)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Write parsed and changed options into output stream
      /// </summary>
      /// <param name="output">Stream into which to output config textual representation</param>
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

