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
               developerMsg: "ConfigParser::FromFile called with null argument in place of config file",
               inner: new ArgumentNullException("configFilePath"));
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
      /// Delimiter used in config values
      /// </summary>
      static internal char delimiter = ',';

      /// <summary>
      /// Internal dictionary of all known sections
      /// </summary>
      private Dictionary<QualifiedSectionName, InnerSection> knownSections = new Dictionary<QualifiedSectionName, InnerSection>();

      /// <summary>
      /// Read configuration data from given stream
      /// </summary>
      /// <param name="input">Stream from which data is to be read</param>
      private void readConfig(StreamReader input)
      {
         // Reset known sections and options
         knownSections = new Dictionary<QualifiedSectionName, InnerSection>();

         // State variables
         QualifiedSectionName curSection = null;
         uint curLine = 0;

         // Crawl full stream
         try
         {
            string oneLine;
            while ((oneLine = input.ReadLine()) != null)
            {
               // Trim & update state variables
               oneLine = oneLine.Trim();
               ++curLine;

               // blank line
               if (oneLine == "")
                  continue;

               // switching logic
               switch (oneLine[0])
               {
                  case '[':
                     curSection = processSingleLineAsSectionStart(oneLine, curLine, knownSections);
                     break;

                  case ';':
                     /*fulline comment*/
                     break;

                  default:
                     processSingleLineAsOption(oneLine, curLine, curSection, knownSections);
                     break;
               }
            }
         }
         catch (ParserException)
         {
            throw;
         }
         catch (Exception ex)
         {
            throw new ParserExceptionWithinConfig(
               userMsg: "Parsing of file failed on line " + curLine,
               developerMsg: "Unexpected exception ocurred on line " + curLine + ", see inner exception for more details",
               line: curLine,
               section: curSection.ID,
               inner: ex);
         }
      }

      /// <summary>
      /// Parsing process, processing single trimmed FULL-line as new section start
      /// </summary>
      /// <param name="oneLine">Line to be processed</param>
      /// <param name="curLine">Current line number, for when exception occurs</param>
      /// <param name="knownSections">Dictionary of all pre-parsed sections</param>
      /// <returns>Newly entered section</returns>
      private static QualifiedSectionName processSingleLineAsSectionStart(string oneLine, uint curLine, Dictionary<QualifiedSectionName, InnerSection> knownSections)
      {
         //{ a-z, A-Z, 0-9, _, ~, -, ., :, $, mezera } začínající znakem z množiny { a-z, A-Z, . , $, : }
         bool matchesSection = System.Text.RegularExpressions.Regex.IsMatch(oneLine, "\\[[a-zA-Z,\\.$:][a-zA-Z0-9_~\\.:$ -]*\\]");

         // Mismatching format
         if (!matchesSection)
            throw new ParserExceptionWithinConfig(
               userMsg: "Error when parsing file on line " + curLine + " unexpected section start",
               developerMsg: "Unexpected start of the file on line " + curLine + ", section start does not match desired format",
               line: curLine);

         // extract values
         int rightBracket = oneLine.IndexOf(']');
         string sectionName = oneLine.Substring(1, rightBracket - 1);
         QualifiedSectionName qName = new QualifiedSectionName(sectionName);

         // Section redefinition
         if (knownSections.ContainsKey(qName))
            throw new ParserExceptionWithinConfig(
               userMsg: "Error when parsing file on line " + curLine + " redefinition of section " + sectionName,
               developerMsg: "Error when parsing file on line " + curLine + " redefinition of section " + sectionName,
               line: curLine,
               section: sectionName);

         // Create new section & return it
         knownSections.Add(qName, new InnerSection(qName));
         return qName;
      }

      /// <summary>
      /// Parsing process, processing single trimmed FULL-line as option
      /// </summary>
      /// <param name="oneLine">Line to be processed</param>
      /// <param name="curLine">Current line number, for when exception occurs</param>
      /// <param name="curSection">Currently processed section</param>
      /// <param name="knownSections">Dictionary of all pre-parsed sections</param>
      private static void processSingleLineAsOption(string oneLine, uint curLine, QualifiedSectionName curSection, Dictionary<QualifiedSectionName, InnerSection> knownSections)
      {
         // no section started yet
         if (curSection == null)
            throw new ParserExceptionWithinConfig(
               userMsg: "Error when parsing file on line " + curLine + " unexpected element outside of section",
               developerMsg: "Unexpected start of the file on line " + curLine + ", option with no section",
               line: curLine,
               section: curSection.ID);

         ////// TODO CALL LEXER

      }

      /// <summary>
      /// Simulates trim function with respect to escaped spaces
      /// </summary>
      /// <param name="value">Untrimmed value with possiblity of escaped spaces</param>
      /// <returns>Final trimmed value, with unescaped spaces</returns>
      static string TrimEscaped(string value)
      {
         string lTrim = value.TrimStart();
         string toOut = lTrim.TrimEnd();

         // push back trimmed last space
         if (!toOut.Equals(lTrim) && toOut[toOut.Length - 1] == '\\')
         {
            toOut = toOut + " ";
         }

         // Unescape other spaces
         toOut = toOut.Replace("\\ "," ");

         return toOut;
      }

      /// <summary>
      /// Write parsed and changed options into output file
      /// </summary>
      /// <param name="outputFile">Path to the oputput file</param>
      internal void Save(string outputFile)
      {
         if (outputFile == null)
         {
            throw new ParserException(
               userMsg: "Parser failed due to wrong usage",
               developerMsg: "ConfigParser::Save called with null argument in place of output file",
               inner: new ArgumentNullException("outputFile"));
         }

         StreamWriter writer;
         try
         {
            writer = new StreamWriter(outputFile);
         }
         catch (Exception ex)
         {
            throw new ParserException(
               userMsg: "Parser failed when accessing fileSystem",
               developerMsg: "ConfigParser::Save problem occured when opening file",
               inner: ex);
         }

         WriteTo(writer);

         writer.Close();
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