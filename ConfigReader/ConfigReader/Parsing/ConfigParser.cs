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
            string fullLineComment = null;
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
                     curSection = Parser.processSingleLineAsSectionStart(oneLine, fullLineComment, curLine, knownSections);
                     fullLineComment = null;
                     break;

                  case ';':
                     fullLineComment = oneLine.Substring(1);
                     break;

                  default:
                     Parser.processSingleLineAsOption(oneLine, curLine, curSection, knownSections);
                     fullLineComment = null;
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
         const string defaultDelimiter = ", ";
         try
         {
            foreach (var sect in knownSections)
            {
               output.WriteLine(";" + sect.Value.Comment);
               output.WriteLine("[" + sect.Key.ID + "]");

               foreach (var opt in sect.Value.Options)
               {
                  WriteCoreTo(output, opt, defaultDelimiter);
               }
            }
         }
         catch (Exception ex)
         {
            throw new ParserException(userMsg: "Error ocurred when writing config file", developerMsg: "Unexpected error ocurred whe writing into stream", inner: ex);
         }
      }

      /// <summary>
      /// Guts of WriteTo method
      /// </summary>
      private static void WriteCoreTo(StreamWriter output, KeyValuePair<QualifiedOptionName,InnerOption> opt, string defaultDelimiter)
      {
         output.Write(opt.Key.ID);
         output.Write('=');

         bool firstStep = true;
         foreach (var optVal in opt.Value.strValues)
         {
            if (firstStep)
            {
               firstStep = false;
            }
            else
            {
               output.Write(defaultDelimiter);
            }

            output.Write(optVal);
         }

         if (opt.Value.Comment != null)
         {
            output.Write(';');
            output.Write(opt.Value.Comment);
         }

         output.WriteLine();
      }

      /// <summary>
      /// Get current option values. Options validity is checked here. Returns only options which were present in file or options that were passed through SetOption.
      /// </summary>
      /// <param name="structure"></param>        
      /// <returns></returns>
      internal IEnumerable<OptionValue> GetOptionValues(StructureInfo structure)
      {
         if (_mode == ParsingMode.Strict)
            markInternalAsUnseen();

         // crawl all sections
         foreach (var section in structure.Sections)
         {
            // crawl all options
            foreach (var option in section.Options)
            {
               object value = extractValue(option);
               yield return new OptionValue(option.Name, value);
            }
         }

         if (_mode == ParsingMode.Strict)
            checkInternalAsSeen();
      }

      /// <summary>
      /// Extracts value from internal representation into final form
      /// </summary>
      /// <param name="info">Option for which to extract the value</param>
      /// <returns>Value in expected format</returns>
      private object extractValue(OptionInfo info)
      {
         // Match inner option to desired one
         InnerOption opt;
         try
         {
            opt = knownSections[info.Name.Section].Options[info.Name];
         }
         catch (Exception ex)
         {
            throw new ParserException(
               userMsg: "Error when retrieving expected value", 
               developerMsg: "Error when finding internal representation of desired option, possibility of it being missing", 
               inner: ex);
         }

         // Check container/singleUnit consistenci
         if (!info.IsContainer && opt.strValues.Count > 1)
            throw new ParserExceptionWithinConfig(
               userMsg: "Error when retrieving expected value",
               developerMsg: "internal representation contains more values, while desired is only one",
               line: opt.Line.Value);

         // Obtain converter
         IValueConverter convertor;
         try
         {
            convertor = Converters.ConfigConverters.getConverter(info);
         }
         catch (Exception ex)
         {
            throw new ParserExceptionWithinConfig(userMsg: "Cannot find convertor", developerMsg: "Convertor terieving method thrown an exception", line: opt.Line, inner: ex);
         }

         // Value extraction
         try
         {
            // Default to be returned
            if (opt.strValues.Count == 0)
            {
               return info.DefaultValue;
            }
            // Container
            if (info.IsContainer)
            {
               List<object> outElements = new List<object>();
               foreach (string value in opt.strValues)
               {
                  outElements.Add(convertor.Deserialize(value));
               }

               return ConfigReader.ConfigCreation.StructureFactory.CreateContainer(info, outElements);
            }
            // Single value
            else
            {
               return convertor.Deserialize(opt.strValues[0]);
            }
         }
         catch (Exception ex)
         {
            throw new ParserExceptionWithinConfig(userMsg: "Type conversion failed", developerMsg: "An exception occurred during extraction of value from inner representaion", line: opt.Line, inner: ex);
         }
      }

      /// <summary>
      /// Marks internal elements as not seen (used with strict mode)
      /// </summary>
      private void markInternalAsUnseen()
      {
         foreach (var sect in knownSections)
         {
            sect.Value.Seen = false;
            foreach (var opt in sect.Value.Options)
            {
               opt.Value.Seen = false;
            }
         }
      }

      /// <summary>
      /// Checks internal elements for being seen (used with strict mode)
      /// </summary>
      private void checkInternalAsSeen()
      {
         bool throwing = false;
         foreach (var sect in knownSections)
         {
            if (!sect.Value.Seen)
            {
               throwing = true;
            }

            foreach (var opt in sect.Value.Options)
            {
               if (!opt.Value.Seen)
               {
                  throwing = true;
               }
            }
         }

         if (throwing)
            throw new ParserException(userMsg: "Config does not comply with the strict mode", developerMsg: "Undefined elements found in the config");
      }

      /// <summary>
      /// Set option info and it's value.
      /// NOTE: Only options which are set via this call can be ADDED into output (readed options are included automatically).
      /// </summary>
      /// <param name="info">Structure describing option format</param>
      /// <param name="value">Structure wearing option value</param>
      internal void SetOption(OptionInfo info, OptionValue value)
      {
         // Check inner constraints
         checkValidity(info, value);
         
         // obtain qNames
         QualifiedSectionName qSect = value.Name.Section;
         QualifiedOptionName qOpt = value.Name;

         // Ensure section exists
         if (!knownSections.ContainsKey(qSect))
            knownSections.Add(qSect, new InnerSection(qSect, null));

         // Ensure option exists
         if (!knownSections[qSect].Options.ContainsKey(qOpt))
         {
            InnerOption newOpt = new InnerOption(qOpt, null);
            knownSections[qSect].Options.Add(qOpt, newOpt);
            newOpt.Comment = info.DefaultComment;
         }

         // Pass value into option
         InnerOption curOpt = knownSections[qSect].Options[qOpt];
         IValueConverter converter;
         try
         {
            converter = Converters.ConfigConverters.getConverter(info);
         }
         catch (Exception ex)
         {
            throw new ParserException(userMsg: "Error when deserializing value", developerMsg: "Unsupported value tried to be deserialized", inner: ex);
         }

         if (info.IsContainer)
         {
            curOpt.strValues.Clear();
            foreach (var elem in ConfigReader.ConfigCreation.StructureFactory.GetContainerElements(value.ConvertedValue))
               curOpt.strValues.Add(converter.Serialize(elem));
         }
         else
         {
            curOpt.strValues.Clear();
            curOpt.strValues.Add(converter.Serialize(value.ConvertedValue));
         }
      }
      
      /// <summary>
      /// Checks whether given otpion value satisfies given option constraints
      /// </summary>
      /// <param name="info">Structure describing option format</param>
      /// <param name="value">Structure wearing option value</param>
      internal void checkValidity(OptionInfo info, OptionValue value)
      {
         if (
            (info.LowerBound != null && wrongOrder(info.LowerBound, value.ConvertedValue))
            ||
            (info.UpperBound != null && wrongOrder(value.ConvertedValue, info.UpperBound))
            )
         {
            throw new ParserExceptionWithinConfig(
               userMsg: "Value out of obunds",
               developerMsg: "Given value is out of predefined bounds",
               section: value.Name.Section.ID,
               option: value.Name.ID);
         }
      }

      /// <summary>
      /// Returns true when lower object is actually bigger
      /// </summary>
      /// <param name="lower">Value that should be lower</param>
      /// <param name="bigger">Value that should be bigger</param>
      /// <returns></returns>
      private static bool wrongOrder(object lower, object bigger)
      {
         return ((IComparable)lower).CompareTo(bigger) > 0;
      }

      /// <summary>
      /// Overrides default and parsed comment.
      /// </summary>
      /// <param name="name">option to which to set the comment</param>
      /// <param name="comment">The comment</param>
      internal void SetComment(QualifiedName name, string comment)
      {
         // Option
         if (name is QualifiedOptionName)
         {
            try
            {
               knownSections[((QualifiedOptionName)name).Section].Options[(QualifiedOptionName)name].Comment = comment;
            }
            catch (Exception ex)
            {
               throw new ParserException(userMsg: "Error setting comment", developerMsg: "Setting of comment failed, possibly due to non-existent option", inner: ex);
            }
         }
         // Section
         else if (name is QualifiedSectionName)
         {
            try
            {
               knownSections[(QualifiedSectionName)name].Comment = comment;
            }
            catch (Exception ex)
            {
               throw new ParserException(userMsg: "Error setting comment", developerMsg: "Setting of comment failed, possibly due to non-existent section", inner: ex);
            }
         }
         // Unsupported
         else
         {
            throw new ParserException(userMsg: "Error setting comment", developerMsg: "Setting of comment for this element is not supported");
         }
      }
   }
}