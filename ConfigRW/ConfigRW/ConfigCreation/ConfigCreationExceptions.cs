using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace ConfigRW
{
    public class PropertyValidationException : ConfigRWException
    {
        /// <summary>
        /// Property where validation error has been found.
        /// </summary>
        public readonly PropertyInfo ValidatedProperty;
        /// <summary>
        /// Exception constructor
        /// </summary>
        /// <param name="userMsg">Message designated for user</param>
        /// <param name="developerMsg">Generic exception message, desingated for application developer</param>        
        /// <param name="inner">Inner exception, for chained exceptions (null if no chained exception)</param>
        public PropertyValidationException(string userMsg, string developerMsg,PropertyInfo validatedProperty, Exception inner = null)
            : base(userMsg, developerMsg, inner:inner)
        {
            ValidatedProperty = validatedProperty;
        }
    }

    public class TypeValidationException: ConfigRWException
    {
        /// <summary>
        /// Property where validation error has been found.
        /// </summary>
        public readonly Type ValidatedType;
        /// <summary>
        /// Exception constructor
        /// </summary>
        /// <param name="userMsg">Message designated for user</param>
        /// <param name="developerMsg">Generic exception message, desingated for application developer</param>        
        /// <param name="inner">Inner exception, for chained exceptions (null if no chained exception)</param>
        public TypeValidationException(string userMsg, string developerMsg, Type validatedType, Exception inner = null)
            : base(userMsg, developerMsg, inner: inner)
        {
            ValidatedType = validatedType;
        }
    }

    public class ContainerBuildException : ConfigRWException
    {
        public readonly Type ContainerType;

        public ContainerBuildException(string userMsg,string developerMsg,Type containerType):
            base(userMsg,developerMsg)
        {
            ContainerType = containerType;
        }
    }

    public class CreateInstanceException: ConfigRWException
    {
        

        public CreateInstanceException(string userMsg, string developerMsg, Exception inner) :
            base(userMsg, developerMsg, inner:inner)
        {            
        }
    }
}
