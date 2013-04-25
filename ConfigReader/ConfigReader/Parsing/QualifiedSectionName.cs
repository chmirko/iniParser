﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.Parsing
{
    /// <summary>
    /// Represents config scope unique name of section.
    /// NOTE: Can be used in natural way as key in hash containers.
    /// </summary>
    sealed class QualifiedSectionName:QualifiedName
    {
        /// <summary>
        /// ID of section. This ID is used in input/output file.
        /// </summary>
        public readonly string ID;

        public QualifiedSectionName(string name)
        {
            ID = name;
        }
        internal override string toString()
        {
            return ID;
        }
    }
}
