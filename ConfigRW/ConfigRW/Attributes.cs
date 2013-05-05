using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Reflection;

namespace ConfigRW
{

    /// <summary>
    /// Attribute for specifying additional information on sections.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    sealed public class SectionInfoAttribute : Attribute
    {
        /// <summary>
        /// ID of section. This ID overrides default ID generated from property name.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Determine that section has to be present in configuration file.
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        /// Create section info attribute.
        /// </summary>
        public SectionInfoAttribute()
        {
        }
    }

    /// <summary>
    /// Attribute for specifying additional information on options.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    sealed public class OptionInfoAttribute : Attribute
    {
        /// <summary>
        /// ID of option. This ID overrides default ID from property name.         
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Default value for option. Containers default value has to be writen in array.
        /// </summary>
        public object DefaultValue { get; set; }
        /// <summary>
        /// Determine that option has to be present in configuration file.
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        /// Create option info attribute.
        /// </summary>
        public OptionInfoAttribute()
        {
        }
    }

    /// <summary>
    /// Attribute for specifiing default comments.
    /// 
    /// NOTE:
    ///     Only option or section properties can be decorated by this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    sealed public class DefaultCommentAttribute : Attribute
    {
        /// <summary>
        /// Text of default comment.
        /// </summary>
        public readonly string CommentText;

        /// <summary>
        /// Create default comment attribute without any comment text.
        /// </summary>
        public DefaultCommentAttribute()
        {
            CommentText = null;
        }

        /// <summary>
        /// Create default comment attribute with specified commentText.
        /// </summary>
        /// <param name="commentText">Text of default comment.</param>
        public DefaultCommentAttribute(string commentText)
        {
            CommentText = commentText;
        }
    }

    /// <summary>
    /// Attribute for specifying range bounds on options with comparable types.
    /// Boundaries has to be of same type as property decorated with this attribute.
    /// This type also has to implement IComparable interface (numerical values, string, enums,..).
    /// 
    /// If option is container, range values has to have it's element type. Range is than
    /// applied on every element in container.
    /// 
    /// NOTE:
    ///     Range is checked only when reading from config defaults/file/stream or saving to file/stream.
    ///     So it's allowed for example fill container with out of range values and filter them according
    ///     your application logic. This attribute only ensures that correct data are read/written from configuration 
    ///     storage.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    sealed public class RangeAttribute : Attribute
    {
        /// <summary>
        /// Upper bound of range. Null means that there is no upper bound.
        /// </summary>
        public object UpperBound { get; set; }
        /// <summary>
        /// Lower bound of range. Null means that there is no lower bound.
        /// </summary>
        public object LowerBound { get; set; }
    }
}
