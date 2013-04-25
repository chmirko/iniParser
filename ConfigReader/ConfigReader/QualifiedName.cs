﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConfigReader.Parsing;

namespace ConfigReader
{
    /// <summary>
    /// Represents config scope unique name of it's elements.
    /// NOTE: Can be used in natural way as key in hash containers.
    /// </summary>
    public abstract class QualifiedName
    {
        
        /// <summary>
        /// Get qualified name for section with given sectionID.
        /// </summary>
        /// <param name="sectionID">ID of section.</param>
        /// <returns>Qualified name for section with sectionID.</returns>
        public static QualifiedName ForSection(string sectionID)
        {
            return new QualifiedSectionName(sectionID);
        }

        /// <summary>
        /// Get qualified name for option with given sectionID and optionID.
        /// </summary>
        /// <param name="sectionID">ID of section, where option is defined.</param>
        /// <param name="optionID">ID of option</param>
        /// <returns>Qualified name for option with optionID in section with sectionID.</returns>
        public static QualifiedName ForOption(string sectionID, string optionID)
        {
            var sectionName = new QualifiedSectionName(sectionID);
            return new QualifiedOptionName(sectionName, optionID);
        }

        /// <summary>
        /// When overriden, creates config scope unique representation of qualified name.
        /// WARNING: Is used as key for comparing and hashing across instances.
        /// </summary>
        /// <returns>Config scope unique representation of qualified name.</returns>
        internal abstract string toString();

        #region Standard method overrides
        public override bool Equals(object obj)
        {
            return ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return toString();
        }
        #endregion
    }
}
