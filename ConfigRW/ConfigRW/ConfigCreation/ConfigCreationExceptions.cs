using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace ConfigRW
{
    /// <summary>
    /// Exception thrown on invalid usage of property in configuration structure definition.
    /// </summary>
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
        /// <param name="validatedProperty">Property which validation failed</param>
        internal PropertyValidationException(string userMsg, string developerMsg, PropertyInfo validatedProperty, Exception inner = null)
            : base(
                string.Format(userMsg, validatedProperty),
                string.Format(developerMsg, validatedProperty),
                inner: inner
            )
        {
            ValidatedProperty = validatedProperty;
        }
    }


    /// <summary>
    /// Exception thrown on invalid usage of ID in configuration structure definition.
    /// </summary>
    public class IDValidationException : ConfigRWException
    {
        /// <summary>
        /// ID which validation failed
        /// </summary>
        public readonly string ValidatedID;
        /// <summary>
        /// Property where invalid ID was found
        /// </summary>
        public readonly PropertyInfo ValidatedProperty;

        /// <summary>
        /// Exception constructor
        /// </summary>
        /// <param name="userMsg">Message designated for user</param>
        /// <param name="developerMsg">Generic exception message, desingated for application developer</param>            
        /// <param name="validatedID">ID which validation failed</param>
        /// <param name="validatedProperty">Property where ID was found</param>
        internal IDValidationException(string userMsg, string developerMsg, string validatedID, PropertyInfo validatedProperty)
            : base(
                string.Format(userMsg, validatedID, validatedProperty),
                string.Format(developerMsg, validatedID, validatedProperty)
            )
        {
            ValidatedID = validatedID;
            ValidatedProperty = validatedProperty;
        }
    }

    /// <summary>
    /// Exception thrown on invalid usage of structure defining type
    /// </summary>
    public class TypeValidationException : ConfigRWException
    {
        /// <summary>
        /// Type where validation error has been found.
        /// </summary>
        public readonly Type ValidatedType;
        /// <summary>
        /// Exception constructor
        /// </summary>
        /// <param name="userMsg">Message designated for user</param>
        /// <param name="developerMsg">Generic exception message, desingated for application developer</param>        
        /// <param name="inner">Inner exception, for chained exceptions (null if no chained exception)</param>
        /// <param name="validatedType">Type which validation failed</param>
        internal TypeValidationException(string userMsg, string developerMsg, Type validatedType, Exception inner = null)
            : base(
            string.Format(userMsg, validatedType),
            string.Format(developerMsg, validatedType),
            inner: inner)
        {
            ValidatedType = validatedType;
        }
    }

    /// <summary>
    /// Exception thrown on invalid usage of container types
    /// </summary>
    public class ContainerBuildException : ConfigRWException
    {
        /// <summary>
        /// Type of container that usage was invalid
        /// </summary>
        public readonly Type ContainerType;

        /// <summary>
        /// Exception constructor
        /// </summary>
        /// <param name="userMsg">Message designated for user</param>
        /// <param name="developerMsg">Generic exception message, desingated for application developer</param>        
        /// <param name="validatedOptionProperty">Property describing option with invalid containerType usage</param>
        /// <param name="inner">Inner exception, for chained exceptions (null if no chained exception)</param>
        /// <param name="containerType">Type of container that usage was invalid</param>
        internal ContainerBuildException(string userMsg, string developerMsg, PropertyInfo validatedOptionProperty, Type containerType, Exception inner = null) :
            base(
            string.Format(userMsg,validatedOptionProperty,containerType), 
            string.Format(developerMsg,validatedOptionProperty,containerType),
            null, inner)
        {
            ContainerType = containerType;
        }
    }

    /// <summary>
    /// Exception thrown on Activator.CreateInstance fail
    /// Usually it means that object thrown exception during construction (Probably malformed container type was present in structure definition)
    /// </summary>
    public class CreateInstanceException : ConfigRWException
    {
        /// <summary>
        /// Exception constructor
        /// </summary>
        /// <param name="userMsg">Message designated for user</param>
        /// <param name="developerMsg">Generic exception message, desingated for application developer</param>        
        /// <param name="inner">Exception which caused that instance creation failed</param>
        internal CreateInstanceException(string userMsg, string developerMsg, Exception inner) :
            base(userMsg, developerMsg, inner: inner)
        {
        }
    }

}
