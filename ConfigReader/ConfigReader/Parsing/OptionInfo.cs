﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.Parsing
{
    /// <summary>
    /// Info describing option.
    /// </summary>
    class OptionInfo
    {
        /// <summary>
        /// Config scope unique name for option.
        /// </summary>
        internal readonly QualifiedOptionName Name;
        /// <summary>
        /// Type that option value in .NET representation will have.
        /// </summary>
        internal readonly Type ExpectedType;
        /// <summary>
        /// Determine that option is optional.
        /// </summary>
        internal readonly bool IsOptional;
        /// <summary>
        /// Default value for option in .NET representation.
        /// </summary>
        internal readonly object DefaultValue;
        /// <summary>
        /// Default comment associated with option.
        /// </summary>
        internal readonly string DefaultComment;
        /// <summary>
        /// Name of property that is associated with option.
        /// </summary>
        internal readonly string AssociatedProperty;

        /// <summary>
        /// Lower bound for numerical values.
        /// NOTE: There is no lower bound if is null.
        /// </summary>
        internal readonly object LowerBound;
        /// <summary>
        /// Upper bound for numerical values
        /// NOTE: There is no upper bound if is null.
        /// </summary>
        internal readonly object UpperBound;

        internal OptionInfo(QualifiedOptionName name, Type expectedType,string associatedProperty, object defaultValue,bool isOptional, string defaultComment,object lowBound,object upBound)
        {
            Name = name;
            ExpectedType = expectedType;
            DefaultValue = defaultValue;
            IsOptional = isOptional;
            DefaultComment = defaultComment;
            AssociatedProperty = associatedProperty;
            LowerBound = lowBound;
            UpperBound = upBound;
        }
    }
}
