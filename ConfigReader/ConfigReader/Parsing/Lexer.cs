using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigReader.Parsing
{
   internal class Lexer
   {
      /// <summary>
      /// Type of final lexes
      /// </summary>
      internal enum Lexes
      {
         /// <summary>
         /// Option identifier
         /// </summary>
         T_Identifier,
         /// <summary>
         /// Value of the option
         /// </summary>
         T_ValuePart,
         /// <summary>
         /// Link to previously parsed value
         /// </summary>
         NT_LinkSection,
         /// <summary>
         /// Link to previously parsed value
         /// </summary>
         NT_LinkOption,
         /// <summary>
         /// Comment
         /// </summary>
         T_Comment
      }

      /// <summary>
      /// Possible states of processing
      /// </summary>
      private enum state
      {
         /// <summary>
         /// Next step is expected to gather identifier start,
         /// space may occur, in which case, state is not changed
         /// </summary>
         gatherIdentifierStart,
         /// <summary>
         /// first non-space character was escaper, so identifier start is yet to come
         /// </summary>
         gatherIdentifierStart_escaped,
         /// <summary>
         /// Identifier start is gathered, next step is expected to gather rest of the body
         /// </summary>
         gatherIdentifierBody,
         /// <summary>
         /// Identifier start is gathered, next step is expected to gather rest of the body,
         /// last gahered characters were spaces, possiblility of space rollback
         /// </summary>
         gatherIdentifierBody_spaces,
         /// <summary>
         /// Identifier start is gathered, next step is expected to gather rest of the body,
         /// escaper was present in the last step
         /// </summary>
         gatherIdentifierBody_escaped,
         /// <summary>
         /// Another lexeme in value part of option is expected,
         /// if spaces occur, they should be ignored, if comment section start occurs, comment state is expected
         /// </summary>
         newLexemeStart,
         /// <summary>
         /// In progress of gathering element body
         /// </summary>
         gatherElementBody,
         /// <summary>
         /// In progress of gathering element body, last character was escaper
         /// </summary>
         gatherElementBody_escaped,
         /// <summary>
         /// In progress of gathering element body, collecting spaces, possibility of spaces rollback
         /// </summary>
         gatherElementBody_spaces,
         /// <summary>
         /// Gatherink link section
         /// </summary>
         gatherLinkSection,
         /// <summary>
         /// Gathering link section, last character was escaper
         /// </summary>
         gatherLinkSection_escaped,
         /// <summary>
         /// Gathering link option
         /// </summary>
         gatherLinkOption,
         /// <summary>
         /// Gathering link option, last character was escaper
         /// </summary>
         gatherLinkOption_escaped,
         /// <summary>
         /// Comment area occurred, next of the line is just the comment
         /// </summary>
         comment
      }

      /// <summary>
      /// Currently lexified line
      /// </summary>
      private readonly string line;

      /// <summary>
      /// All known (previously parsed) sections (used for link evaluation)
      /// </summary>
      Dictionary<QualifiedSectionName, InnerSection> knownSections;

      /// <summary>
      /// Used delimiter in between values
      /// </summary>
      private char? delimiter;

      /// <summary>
      /// Position in current line
      /// </summary>
      private int position;

      /// <summary>
      /// Current state of automaton
      /// </summary>
      private state curState;

      /// <summary>
      /// number of unescaped spaces gathered (used with some states)
      /// </summary>
      private int spacesGathered;

      /// <summary>
      /// Lexeme allready being built
      /// </summary>
      StringBuilder curLexeme;

      /// <summary>
      /// Current lexes
      /// </summary>
      private List<Tuple<Lexes,string>> lexes;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="line">Line to be lexified</param>
      /// <param name="knownSections">Dictionary of all pre-parsed sections</param>
      internal Lexer(string line, Dictionary<QualifiedSectionName, InnerSection> knownSections)
      {
         this.line = line;
         this.knownSections = knownSections;
      }

      /// <summary>
      /// Return lexes from given line
      /// </summary>
      /// <returns>Lexes lexified from line provided in construction</returns>
      internal List<Tuple<Lexes, string>> GetLexes()
      {
         // Init automaton
         this.delimiter = null;
         position = 0;
         curState = state.gatherIdentifierStart;
         spacesGathered = 0;
         curLexeme = new StringBuilder();
         lexes = new List<Tuple<Lexes, string>>();

         // Process full line
         while (position < line.Length)
         {
            advanceStep();
         }
         action_finalize();

         // Return processed lexes
         return lexes;
      }

      /// <summary>
      /// Advances step by one (more in some special cases)
      /// </summary>
      private void advanceStep()
      {
         switch (curState)
         {
            case state.gatherIdentifierStart:
               action_gatherIdentifierStart();
               break;
            case state.gatherIdentifierStart_escaped:
               action_gatherIdentifierStart_escaped();
               break;
            case state.gatherIdentifierBody:
               action_gatherIdentifierBody();
               break;
            case state.gatherIdentifierBody_spaces:
               action_gatherIdentifierBody_spaces();
               break;
            case state.gatherIdentifierBody_escaped:
               action_gatherIdentifierBody_escaped();
               break;
            case state.newLexemeStart:
               action_newLexemeStart();
               break;
            case state.gatherElementBody:
               action_gatherElementBody();
               break;
            case state.gatherElementBody_escaped:
               action_gatherElementBody_escaped();
               break;
            case state.gatherElementBody_spaces:
               action_gatherElementBody_spaces();
               break;
            case state.gatherLinkSection:
               action_gatherLinkSection();
               break;
            case state.gatherLinkSection_escaped:
               action_gatherLinkSection_escaped();
               break;
            case state.gatherLinkOption:
               action_gatherLinkOption();
               break;
            case state.gatherLinkOption_escaped:
               action_gatherLinkOption_escaped();
               break;
            case state.comment:
               action_comment();
               break;
            default:
               throw new ParserException(userMsg: "Error when parsing config", developerMsg: "Unexpected automaton state in Lexer::advanceStep");
         }
      }

      private void action_gatherIdentifierStart()
      {
         // Skip preceeding blanks
         if (line[position] == ' ')
         {
            /*just advance*/
         }
         // identifier start
         else if (canBeIdentifierStart(line[position]))
         {
            curLexeme.Append(line[position]);
            curState = state.gatherIdentifierBody;
         }
         // Escaper found
         else if (line[position] == '\\')
         {
            curState = state.gatherIdentifierStart_escaped;
         }
         // unexpected
         else
         {
            throw new ParserException(userMsg: "Error during parsing", developerMsg: "Unexpected character during gatherIdentifierStart");
         }

         ++position;
      }

      private void action_gatherIdentifierStart_escaped()
      {
         // identifier start
         if (canBeIdentifierStart(line[position]))
         {
            curLexeme.Append(line[position]);
            curState = state.gatherIdentifierBody;
         }
         // unexpected
         else
         {
            throw new ParserException(userMsg: "Error during parsing", developerMsg: "Unexpected character during gatherIdentifierStart");
         }

         ++position;
      }

      private void action_gatherIdentifierBody()
      {
         // Gathering spaces
         if (line[position] == ' ')
         {
            curLexeme.Append(line[position]);
            ++spacesGathered;
            curState = state.gatherIdentifierBody_spaces;
         }
         // identifier continuation
         else if (canBeIdentifierPart(line[position]))
         {
            curLexeme.Append(line[position]);
         }
         // Escaper found
         else if (line[position] == '\\')
         {
            curState = state.gatherIdentifierBody_escaped;
         }
         // Identifier end
         else if (line[position] == '=')
         {
            lexes.Add(new Tuple<Lexes,string>(Lexes.T_Identifier,curLexeme.ToString()));
            curLexeme = new StringBuilder();
            curState = state.newLexemeStart;
         }
         // unexpected
         else
         {
            throw new ParserException(userMsg: "Error during parsing", developerMsg: "Unexpected character during gatherIdentifierBody");
         }

         ++position;
      }

      private void action_gatherIdentifierBody_spaces()
      {
         // Gathering spaces
         if (line[position] == ' ')
         {
            curLexeme.Append(line[position]);
            ++spacesGathered;
         }
         // identifier continuation
         else if (canBeIdentifierPart(line[position]))
         {
            curLexeme.Append(line[position]);
            curState = state.gatherIdentifierBody;
            spacesGathered = 0;
         }
         // Escaper found
         else if (line[position] == '\\')
         {
            curState = state.gatherIdentifierBody_escaped;
            spacesGathered = 0;
         }
         // Identifier end (must rollback spaces)
         else if (line[position] == '=')
         {
            curLexeme.Length -= spacesGathered;
            spacesGathered = 0;
            lexes.Add(new Tuple<Lexes, string>(Lexes.T_Identifier, curLexeme.ToString()));
            curLexeme = new StringBuilder();
            curState = state.newLexemeStart;
         }
         // unexpected
         else
         {
            throw new ParserException(userMsg: "Error during parsing", developerMsg: "Unexpected character during gatherIdentifierBody_spaces");
         }

         ++position;
      }

      private void action_gatherIdentifierBody_escaped()
      {
         // identifier continuation
         if (canBeIdentifierPart(line[position]))
         {
            curLexeme.Append(line[position]);
            curState = state.gatherIdentifierBody;
         }
         // unexpected
         else
         {
            throw new ParserException(userMsg: "Error during parsing", developerMsg: "Unexpected character during gatherIdentifierBody_escaped");
         }

         ++position;
      }

      private void action_newLexemeStart()
      {
         // Skip preceeding blanks
         if (line[position] == ' ')
         {
            /*just advance*/
            ++position;
         }
         // Comment start
         else if (line[position] == ';')
         {
            curState = state.comment;
            ++position;
         }
         // Escaper found
         else if (line[position] == '\\')
         {
            curState = state.gatherElementBody_escaped;
            ++position;
         }
         // Link start
         else if (line[position] == '$')
         {
            if (position + 1 == line.Length || line[position + 1] != '{')
            {
               throw new ParserException(userMsg: "Error while parsing", developerMsg: "Inappropriate link start");
            }

            curState = state.gatherLinkSection;
            position += 2;
         }
         // Delimiter found
         else if (line[position] == ',' || line[position] == ':')
         {
            // Check homogenous delimiters
            if (delimiter == null)
            {
               delimiter = line[position];
            }
            else if (delimiter != line[position])
            {
               throw new ParserException(
                  userMsg: "Error during parsing, heterogenous delimiters",
                  developerMsg: "At least two different types of delimiter found on single line");
            }

            // store previous lexeme and start again
            lexes.Add(new Tuple<Lexes, string>(Lexes.T_ValuePart, ""));
            ++position;
         }
         // Take as part of value
         else
         {
            curLexeme.Append(line[position]);
            curState = state.gatherElementBody;
            ++position;
         } 
      }

      private void action_gatherElementBody()
      {
         // Collectiong spaces (possibility of spaces rollback)
         if (line[position] == ' ')
         {
            curLexeme.Append(line[position]);
            curState = state.gatherIdentifierBody_spaces;
            ++spacesGathered;
         }
         // Comment start
         else if (line[position] == ';')
         {
            lexes.Add(new Tuple<Lexes,string>(Lexes.T_ValuePart,curLexeme.ToString()));
            curLexeme = new StringBuilder();
            curState = state.comment;
         }
         // Escaper found
         else if (line[position] == '\\')
         {
            curState = state.gatherElementBody_escaped;
         }
         // Delimiter found
         else if (line[position] == ',' || line[position] == ':')
         {
            // Check homogenous delimiters
            if (delimiter == null)
            {
               delimiter = line[position];
            }
            else if (delimiter != line[position])
            {
               throw new ParserException(
                  userMsg: "Error during parsing, heterogenous delimiters",
                  developerMsg: "At least two different types of delimiter found on single line");
            }

            // store previous lexeme and start again
            lexes.Add(new Tuple<Lexes, string>(Lexes.T_ValuePart, curLexeme.ToString()));
            curLexeme = new StringBuilder();
            curState = state.newLexemeStart;
         }
         // Take as part of value
         else
         {
            curLexeme.Append(line[position]);
         }

         ++position;
      }

      private void action_gatherElementBody_escaped()
      {
         // Take as part of value
         curLexeme.Append(line[position]);
         ++position;
      }

      private void action_gatherElementBody_spaces()
      {
         // Collectiong spaces (possibility of spaces rollback)
         if (line[position] == ' ')
         {
            curLexeme.Append(line[position]);
            ++spacesGathered;
         }
         // Comment start
         else if (line[position] == ';')
         {
            curLexeme.Length -= spacesGathered;
            spacesGathered = 0;
            lexes.Add(new Tuple<Lexes, string>(Lexes.T_ValuePart, curLexeme.ToString()));
            curLexeme = new StringBuilder();
            curState = state.comment;
         }
         // Escaper found
         else if (line[position] == '\\')
         {
            curState = state.gatherElementBody_escaped;
            spacesGathered = 0;
         }
         // Delimiter found
         else if (line[position] == ',' || line[position] == ':')
         {
            // Check homogenous delimiters
            if (delimiter == null)
            {
               delimiter = line[position];
            }
            else if (delimiter != line[position])
            {
               throw new ParserException(
                  userMsg: "Error during parsing, heterogenous delimiters",
                  developerMsg: "At least two different types of delimiter found on single line");
            }

            // store previous lexeme and start again
            curLexeme.Length -= spacesGathered;
            spacesGathered = 0;
            lexes.Add(new Tuple<Lexes, string>(Lexes.T_ValuePart, curLexeme.ToString()));
            curLexeme = new StringBuilder();
            curState = state.newLexemeStart;
         }
         // Take as part of value
         else
         {
            curLexeme.Append(line[position]);
            spacesGathered = 0;
            curState = state.gatherElementBody;
         }

         ++position;
      }

      private void action_gatherLinkSection()
      {
         // Escaper
         if (line[position] == '\\')
         {
            curState = state.gatherLinkSection_escaped;
         }
         // Delimiter section/option
         else if (line[position] == '#')
         {
            lexes.Add(new Tuple<Lexes, string>(Lexes.NT_LinkSection, curLexeme.ToString()));
            curLexeme = new StringBuilder();
            curState = state.gatherLinkOption;
         }
         // part of link
         else
         {
            curLexeme.Append(line[position]);
         }

         ++position;
      }

      private void action_gatherLinkSection_escaped()
      {
         curLexeme.Append(line[position]);
         curState = state.gatherLinkSection;
         ++position;
      }

      private void action_gatherLinkOption()
      {
         // Escaper
         if (line[position] == '\\')
         {
            curState = state.gatherLinkOption_escaped;
         }
         // Delimiter section/option
         else if (line[position] == '}')
         {
            lexes.Add(new Tuple<Lexes, string>(Lexes.NT_LinkOption, curLexeme.ToString()));
            evaluateLink();
            curLexeme = new StringBuilder();
            curState = state.newLexemeStart;
         }
         // part of link
         else
         {
            curLexeme.Append(line[position]);
         }

         ++position;
      }

      private void action_gatherLinkOption_escaped()
      {
         curLexeme.Append(line[position]);
         curState = state.gatherLinkOption;
         ++position;
      }

      private void action_comment()
      {
         curLexeme.Append(line[position]);
         ++position;
      }

      /// <summary>
      /// Finalizes last action on lineEnd
      /// </summary>
      private void action_finalize()
      {
         switch (curState)
         {
            case state.gatherIdentifierStart:
            case state.gatherIdentifierStart_escaped:
            case state.gatherIdentifierBody:
            case state.gatherIdentifierBody_spaces:
            case state.gatherIdentifierBody_escaped:
               throw new ParserException(userMsg: "Error when parsing config", developerMsg: "unexpected final state in action_finalize");
            case state.newLexemeStart:
               /*no action*/
               break;
            case state.gatherElementBody:
            case state.gatherElementBody_escaped:
            case state.gatherElementBody_spaces:
               lexes.Add(new Tuple<Lexes,string>(Lexes.T_ValuePart,curLexeme.ToString()));
               curLexeme = new StringBuilder();
               break;
            case state.gatherLinkSection:
            case state.gatherLinkSection_escaped:
            case state.gatherLinkOption:
            case state.gatherLinkOption_escaped:
               throw new ParserException(userMsg: "Error when parsing config", developerMsg: "unexpected final state in action_finalize");
            case state.comment:
               lexes.Add(new Tuple<Lexes,string>(Lexes.T_Comment,curLexeme.ToString()));
               curLexeme = new StringBuilder();
               break;
            default:
               throw new ParserException(userMsg: "Error when parsing config", developerMsg: "Unexpected automaton state in Lexer::action_finalize");
         }
      }

      /// <summary>
      /// Determines whether given char can be start of identifier
      /// </summary>
      /// <param name="chr">character to be checked</param>
      /// <returns>decision</returns>
      private bool canBeIdentifierStart(char chr)
      {
         //řetězec znaků z množiny { a-z, A-Z, 0-9, _, ~, -, ., :, $, mezera } začínající znakem z množiny { a-z, A-Z, . , $, : }

         if (
            (chr >= 'a' && chr <= 'z')
            ||
            (chr >= 'A' && chr <= 'Z')
            ||
            (chr == '.' || chr == '$' || chr == ':'))
            return true;
         else
            return false;
      }

      /// <summary>
      /// Determines whether given char can be part of identifier
      /// </summary>
      /// <param name="chr">character to be checked</param>
      /// <returns>decision</returns>
      private bool canBeIdentifierPart(char chr)
      {
         //řetězec znaků z množiny { a-z, A-Z, 0-9, _, ~, -, ., :, $, mezera } začínající znakem z množiny { a-z, A-Z, . , $, : }

         if (
            (chr >= 'a' && chr <= 'z')
            ||
            (chr >= 'A' && chr <= 'Z')
            || 
            (chr >= '0' && chr <= '9')
            ||
            (chr == '_' || chr=='~' || chr=='-' || chr == '.' || chr == ':' || chr == '$' || chr==' '))
            return true;
         else
            return false;
      }

      /// <summary>
      /// Evaluates last link in the lexes list
      /// </summary>
      private void evaluateLink()
      {
         var lnkSection = lexes.Last();
         lexes.RemoveAt(lexes.Count - 1);
         var lnkOption = lexes.Last();
         lexes.RemoveAt(lexes.Count - 1);

         if (lnkSection.Item1 != Lexes.NT_LinkSection || lnkOption.Item1 != Lexes.NT_LinkOption)
            throw new ParserException(userMsg: "Error when parsing configuration file", developerMsg: "unexpected element instead of Link parts in Lexer::evaluateLink");

         QualifiedSectionName qSec = new QualifiedSectionName(lnkSection.Item2);
         QualifiedOptionName qOpt = new QualifiedOptionName(qSec,lnkOption.Item2);

         InnerOption opt;
         try
         {
            opt = knownSections[qSec].Options[qOpt];
         }
         catch (Exception ex)
         {
            throw new ParserException(
               userMsg: "Error when evaluating link during parsing of configuration file", 
               developerMsg: "Link inside option value points to nonexistent section#option",
               inner: ex);
         }

         foreach (string val in opt.strValues)
         {
            lexes.Add(new Tuple<Lexes, string>(Lexes.T_ValuePart, val));
         }
      }
   }
}
