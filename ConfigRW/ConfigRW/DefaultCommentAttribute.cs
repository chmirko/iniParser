using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConfigRW
{
    /// <summary>
    /// Attribute for specifiing default comments.
    /// Can be attached to option or section property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DefaultCommentAttribute:Attribute
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
}
