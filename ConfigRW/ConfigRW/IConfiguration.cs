using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.IO;

using ConfigRW.Parsing;

namespace ConfigRW
{
    /// <summary>
    /// Required interface for configuration structure.
    /// Provide access to configuration services.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Save configuration into output file.
        /// </summary>
        /// <param name="outputFile">Output file where configuration will be saved.</param>
        void SaveTo(string outputFile);
        /// <summary>
        /// Write configuration into output.
        /// </summary>
        /// <param name="output">Output for configuration.</param>
        void WriteTo(StreamWriter output);
        /// <summary>
        /// Set comment to specified element.
        /// </summary>
        /// <param name="name">Element which comment will be set.</param>
        /// <param name="commentText">Text of comment.</param>
        void SetComment(QualifiedName name, string commentText);
    }
}
