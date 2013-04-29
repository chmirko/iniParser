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
      internal Lexer(string line)
      {
         // Lexer info
         this.line = line;
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
            advanceStep();

         // Return processed lexes
         return lexes;
      }

      /// <summary>
      /// Advances step by one (more in some special cases)
      /// </summary>
      private void advanceStep()
      {
         ///////////// TODO: implement, and tear into small methods
         switch (curState)
         {
            case state.gatherIdentifierStart:
               break;
            case state.gatherIdentifierStart_escaped:
               break;
            case state.gatherIdentifierBody:
               break;
            case state.gatherIdentifierBody_spaces:
               break;
            case state.gatherIdentifierBody_escaped:
               break;
            case state.newLexemeStart:
               break;
            case state.gatherElementBody:
               break;
            case state.gatherElementBody_escaped:
               break;
            case state.gatherElementBody_spaces:
               break;
            case state.gatherLinkSection:
               break;
            case state.gatherLinkSection_escaped:
               break;
            case state.gatherLinkOption:
               break;
            case state.gatherLinkOption_escaped:
               break;
            case state.comment:
               break;
            default:
               throw new ParserException(userMsg: "Error when parsing config", developerMsg: "Unexpected automaton state in Lexer::advanceStep");
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
   }
}
