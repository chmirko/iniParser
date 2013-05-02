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
            checkSignature(structureType);
            checkSectionUniqueness(structureType);

            foreach (var property in structureType.GetProperties())
            {
                if (property.GetSetMethod() != null)
                {
                    throw new PropertyValidationException(
                        userMsg: "Section properties cannot have setters",
                        developerMsg: "StructureValidation::ThrowOnInvalid failed because setter has been found on section property",
                        validatedProperty: property
                        );
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
                    throw new PropertyValidationException(
                       userMsg: "Duplicit ID has been found",
                       developerMsg: "StructureValidation::checkIDUniqueness failed because duplicit ID has been found on property",
                       validatedProperty: property
                       );
                }
            }
        }

        /// <summary>
        /// Check that type contains valid constructs only.
        /// </summary>
        /// <param name="structureType">Type which signature will be checked.</param>
        private static void checkSignature(Type structureType)
        {
            if (!structureType.IsInterface)
            {
                throw new TypeValidationException(
                       userMsg: "Type describing configuration structure has to be interface",
                       developerMsg: "StructureValidation::checkSignature failed because structure type isn't interface",
                       validatedType: structureType
                       );
            }

            //note: all methods are public because we are in interface
            foreach (var method in structureType.GetMethods())
            {
                if (method.IsStatic)
                {
                    throw new TypeValidationException(
                          userMsg: "Type describing configuration structure can't have static methods",
                          developerMsg: "StructureValidation::checkSignature failed because static method has been found in structure type",
                          validatedType: structureType
                          );
                }

                if (!method.IsSpecialName)
                {
                    throw new TypeValidationException(
                     userMsg: "Type describing configuration structure can only have property methods (getters and setters)",
                     developerMsg: "StructureValidation::checkSignature failed because method without assigned property has been found in structure type",
                     validatedType: structureType
                     );
                }
            }
        }
    }
}
