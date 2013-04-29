using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Reflection;

namespace ConfigReader.ConfigCreation
{
    /// <summary>
    /// Method that provide ID resolving.
    /// </summary>
    /// <param name="property">Property which describes ID.</param>
    /// <returns>Resolved ID.</returns>
    internal delegate string IDResolver(PropertyInfo property);

    /// <summary>
    /// Structure validation services.
    /// </summary>
    static class StructureValidation
    {
        /// <summary>
        /// Check validity of structureType. If error is found, appropriate exception is thrown.
        /// </summary>
        /// <param name="structureType">Type describing validated structure.</param>
        internal static void ThrowOnInvalid(Type structureType)
        {
            //TODO attribute semantic correctness

            checkSectionUniqueness(structureType);

            foreach (var property in structureType.GetProperties())
            {
                if (property.GetSetMethod() != null)
                {
                    throw new NotSupportedException("Sections cannot have setters");
                }

                var optionType = property.PropertyType;
                checkSignature(optionType);
                checkOptionUniqueness(optionType);
            }
        }

        /// <summary>
        /// Check that every section in structure has unique ID.
        /// </summary>
        /// <param name="structureType">Type describing structure.</param>
        private static void checkSectionUniqueness(Type structureType)
        {
            checkIDUniqueness(structureType, StructureFactory.ResolveSectionID);
        }

        /// <summary>
        /// Check that every option in section has unique ID.
        /// </summary>
        /// <param name="sectionType">Type describing section.</param>
        private static void checkOptionUniqueness(Type sectionType)
        {
            checkIDUniqueness(sectionType, StructureFactory.ResolveOptionID);
        }

        /// <summary>
        /// Check that ids produced by resolver are unique.
        /// </summary>
        /// <param name="type">Type which properties are traversed.</param>
        /// <param name="resolver">Resolver which produce id from traversed properties.</param>
        private static void checkIDUniqueness(Type type, IDResolver resolver)
        {
            var ids = new HashSet<string>();
            foreach (var property in type.GetProperties())
            {
                var id = resolver(property);
                if (!ids.Add(id))
                {
                    throw new NotSupportedException("Duplicit ID on property '" + property + "' was detected.");
                }
            }
        }

        /// <summary>
        /// Check that type contains valid constructs only.
        /// </summary>
        /// <param name="type">Type which signature will be checked.</param>
        private static void checkSignature(Type type)
        {
            if (!type.IsInterface)
            {
                throw new NotSupportedException("Given type is not supported structure type. Expects interface only.");
            }

            //note: all methods are public because we are in interface
            foreach (var method in type.GetMethods())
            {
                if (method.IsStatic)
                {
                    throw new NotSupportedException("Only instance methods are expected in structure type.");
                }

                if (!method.IsSpecialName)
                {
                    throw new NotSupportedException("Only properties are expected in structure type.");
                }
            }
        }
    }
}
