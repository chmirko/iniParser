using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigRW.Parsing
{
   /// <summary>
   /// Line parser for reading configuration file
   /// </summary>
   class Parser
   {
      /// <summary>
      /// Parsing process, processing single trimmed FULL-line as new section start
      /// </summary>
      /// <param name="oneLine">Line to be processed</param>
      /// <param name="fullLineComment">Comment to given section</param>
      /// <param name="curLine">Current line number, for when exception occurs</param>
      /// <param name="knownSections">Dictionary of all pre-parsed sections</param>
      /// <returns>Newly entered section</returns>
      internal static QualifiedSectionName processSingleLineAsSectionStart(string oneLine, string fullLineComment, uint curLine, Dictionary<QualifiedSectionName, InnerSection> knownSections)
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
         knownSections.Add(qName, new InnerSection(qName,fullLineComment));
         return qName;
      }

      /// <summary>
      /// Parsing process, processing single trimmed FULL-line as option
      /// </summary>
      /// <param name="oneLine">Line to be processed</param>
      /// <param name="curLine">Current line number, for when exception occurs</param>
      /// <param name="curSection">Currently processed section</param>
      /// <param name="knownSections">Dictionary of all pre-parsed sections</param>
      internal static void processSingleLineAsOption(string oneLine, uint curLine, QualifiedSectionName curSection, Dictionary<QualifiedSectionName, InnerSection> knownSections)
      {
         // no section started yet
         if (curSection == null)
            throw new ParserExceptionWithinConfig(
               userMsg: "Error when parsing file on line " + curLine + " unexpected element outside of section",
               developerMsg: "Unexpected start of the file on line " + curLine + ", option with no section",
               line: curLine,
               section: curSection.ID);

         try
         {
            Lexer lineLexer = new Lexer(oneLine, knownSections);
            var lexes = lineLexer.GetLexes();

            var lxName = lexes[0];
            lexes.RemoveAt(0);
            if (lxName.Item1 != Lexer.Lexes.T_Identifier)
               throw new ParserExceptionWithinConfig(
                  userMsg: "Error when parsing file on line " + curLine,
                  developerMsg: "Lexer process does not provided option identifier",
                  line: curLine,
                  section: curSection.ID);

            QualifiedOptionName qName = new QualifiedOptionName(curSection, lxName.Item2);
            if (knownSections[curSection].Options.ContainsKey(qName))
               throw new ParserExceptionWithinConfig(
                  userMsg: "Error when parsing file on line " + curLine + ", duplicit option",
                  developerMsg: "Duplicit option ID occurred inside this section",
                  line: curLine,
                  section: curSection.ID,
                  option: qName.ID);

            InnerOption newOpt = new InnerOption(qName, curLine);
            knownSections[curSection].Options.Add(qName, newOpt);

            foreach (var lexeme in lexes)
            {
               if (lexeme.Item1 == Lexer.Lexes.T_Comment && lexeme == lexes[lexes.Count - 1])
               {
                  newOpt.Comment = lexeme.Item2;
               }
               else if (lexeme.Item1 == Lexer.Lexes.T_ValuePart)
               {
                  newOpt.strValues.Add(lexeme.Item2);
               }
               else
               {
                  throw new ParserExceptionWithinConfig(
                     userMsg: "Error when parsing file on line " + curLine,
                     developerMsg: "unexpected lexeme pop off the lexer",
                     line: curLine,
                     section: curSection.ID,
                     option: qName.ID);
               }
            }

         }
         catch (ParserException ex)
         {
            throw new ParserExceptionWithinConfig(
               userMsg: "Error when parsing file on line " + curLine,
               developerMsg: "Exception ocurred in Lexer process",
               line: curLine,
               section: curSection.ID,
               inner: ex);

         }
         catch (Exception ex)
         {
            throw new ParserExceptionWithinConfig(
               userMsg: "Error when parsing file on line " + curLine,
               developerMsg: "Unexpected exception ocurred in Lexer process",
               line: curLine,
               section: curSection.ID,
               inner: ex);
         }
      }

      /// <summary>
      /// Simulates trim function with respect to escaped spaces
      /// </summary>
      /// <param name="value">Untrimmed value with possiblity of escaped spaces</param>
      /// <returns>Final trimmed value, with unescaped spaces</returns>
      private static string TrimEscaped(string value)
      {
         string lTrim = value.TrimStart();
         string toOut = lTrim.TrimEnd();

         // push back trimmed last space
         if (!toOut.Equals(lTrim) && toOut[toOut.Length - 1] == '\\')
         {
            toOut = toOut + " ";
         }

         // Unescape other spaces
         toOut = toOut.Replace("\\ ", " ");

         return toOut;
      }
   }
}
