using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DefaultCommentAttribute:Attribute
    {
        public readonly string CommentText;

        public DefaultCommentAttribute()
        {
            CommentText = null;
        }

        public DefaultCommentAttribute(string commentText)
        {
            CommentText = commentText;
        }
    }
}
