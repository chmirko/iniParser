using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;

using System.Reflection;

namespace ConfigRW.ConfigCreation
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
        private static Regex IDValidator = new Regex("^[a-zA-Z.$:][a-zA-Z0-9_~.:$ -]*$");

        /// <summary>
        /// Check validity of structureType. If error is found, appropriate exception is thrown.
        /// </summary>
        /// <param name="structureType">Type describing validated structure.</param>
        internal static void ThrowOnInvalid(Type structureType)
        {
            if (structureType.GetInterfaces().Count() != 1)
            {
                throw new TypeValidationException(
                        userMsg: "Structure type '{0}' has incorrect format, only IConfiguration interface can be implemented",
                        developerMsg: "StructureValidation::ThrowOnInvalid failed because on structure type '{0}' was found more than IConfiguration interface implemented",
                        validatedType: structureType
                    );
            }

            checkSignature(structureType,false);
            checkIDsValidity(StructureFactory.GetSectionProperties(structureType), StructureFactory.ResolveSectionID);
            checkSectionUniqueness(structureType);

            foreach (var sectionProperty in structureType.GetProperties())
            {
                if (sectionProperty.GetSetMethod() != null)
                {
                    throw new PropertyValidationException(
                        userMsg: "Setter detected on section property '{0}'. Section properties cannot have setters.",
                        developerMsg: "StructureValidation::ThrowOnInvalid failed because setter has been found on section property '{0}'",
                        validatedProperty: sectionProperty
                        );
                }

                var sectionType = sectionProperty.PropertyType;
                checkSignature(sectionType,true);
                checkIDsValidity(StructureFactory.GetOptionProperties(sectionType), StructureFactory.ResolveOptionID);
                checkOptionUniqueness(sectionType);
                checkTypeMatching(sectionType);
            }
        }

        /// <summary>
        /// Check that every default value or range attribute will have data with matching type for property.
        /// </summary>
        /// <param name="sectionType">Section which properties will be checked.</param>
        private static void checkTypeMatching(Type sectionType)
        {
            foreach (var optionProperty in sectionType.GetProperties())
            {
                var requiredType = StructureFactory.GetElementType(optionProperty.PropertyType);

                if (requiredType == null)
                {
                    //property is not container, because we cant determine element type                    
                    requiredType = optionProperty.PropertyType;
                }



                checkDefaultValueMatching(optionProperty, requiredType);
                checkRangeTypeMatching(optionProperty, requiredType);
            }
        }

        /// <summary>
        /// Check that default value object has matching type for optionProperty.
        /// </summary>
        /// <param name="optionProperty">Validated property.</param>
        /// <param name="requiredType">Type that is required for property value or its elements.</param>       
        private static void checkDefaultValueMatching(PropertyInfo optionProperty, Type requiredType)
        {
            var defaultValue = ReflectionUtils.GetAttribute<OptionInfoAttribute>(optionProperty).DefaultValue;
            if (defaultValue == null)
            {
                //There is no default value
                return;
            }

            var isContainer = StructureFactory.GetContainerBuilder(optionProperty.PropertyType) != null;

            if (isContainer)
            {
                if (!testContainerDefault(requiredType, defaultValue))
                {
                    throw new PropertyValidationException(
                        userMsg: "Default value for container option property '{0}' has incorrect type. Array type '" + requiredType + "[]' is required.",
                        developerMsg: "StructureValidation::checkTypeMatching has failed due to incorrect type of default container value on property '{0}'. Array type '" + requiredType + "[]' is required.",
                        validatedProperty: optionProperty
                        );
                }
            }
            else
            {
                if (!testAssignability(requiredType, defaultValue))
                {
                    throw new PropertyValidationException(
                        userMsg: "Default value for option property '{0}' has incorrect type. Type '" + requiredType + "' is required.",
                        developerMsg: "StructureValidation::checkTypeMatching has failed due to incorrect type of default value on property '{0}'. Type '" + requiredType + "' is required.",
                        validatedProperty: optionProperty
                        );
                }
            }
        }

        /// <summary>
        /// Check validity of range attribute usage on optionProperty.
        /// </summary>
        /// <param name="optionProperty">Validated property.</param>
        /// <param name="requiredType">Type that is required for property value or its elements.</param>        
        private static void checkRangeTypeMatching(PropertyInfo optionProperty, Type requiredType)
        {
            var range = ReflectionUtils.GetAttribute<RangeAttribute>(optionProperty);
            var hasRangeBound = range.UpperBound != null && range.LowerBound != null;

            if (!hasRangeBound)
            {
                //there is no range specified
                return;
            }

            if (!typeof(IComparable).IsAssignableFrom(requiredType))
            {
                throw new PropertyValidationException(
                   userMsg: "Range bounds cannot be set for property '{0}' because it doesn't implement IComparable interface",
                   developerMsg: "StructureValidation::checkTypeMatching has failed due to missing IComparable implementation on property '{0}' for range bounds support",
                   validatedProperty: optionProperty
                   );
            }

            if (!testAssignability(requiredType, range.LowerBound))
            {
                throw new PropertyValidationException(
                    userMsg: "Value of lower bound for option property '{0}' has incorrect type. Type '" + requiredType + "' is required.",
                    developerMsg: "StructureValidation::checkTypeMatching has failed due to incorrect lower bound type on property '{0}'. Type '" + requiredType + "' is required.",
                    validatedProperty: optionProperty
                    );
            }

            if (!testAssignability(requiredType, range.UpperBound))
            {
                throw new PropertyValidationException(
                    userMsg: "Value of upper bound for option property '{0}' has incorrect type. Type '" + requiredType + "' is required.",
                    developerMsg: "StructureValidation::checkTypeMatching has failed due to incorrect upper bound type on property '{0}'. Type '" + requiredType + "' is required.",
                    validatedProperty: optionProperty
                    );
            }
        }

        /// <summary>
        /// Test that assignedObject can be assigned to property with requiredType.
        /// </summary>
        /// <param name="requiredType">Type of property that is required for assigning.</param>
        /// <param name="assignedObject">Object that is tested to assignability.</param>
        /// <returns>True if assignedObject can be assigned to property, false otherwise.</returns>
        private static bool testAssignability(Type requiredType, object assignedObject)
        {
            if (assignedObject == null)
            {
                return true;
            }

            return requiredType.IsAssignableFrom(assignedObject.GetType());
        }

        /// <summary>
        /// Determine that defaultValue is valid for container consisting of elementType elements.
        /// </summary>
        /// <param name="elementType">Type of elements stored in container.</param>
        /// <param name="defaultValue">Default value for container.</param>
        /// <returns>True if defaultValue is valid for container, false otherwise.</returns>
        private static bool testContainerDefault(Type elementType, object defaultValue)
        {
            //default value has to be null or RequiredType[]

            if (defaultValue == null)
            {
                return true;
            }

            var defaultValueType = defaultValue.GetType();
            if (!defaultValueType.IsArray)
            {
                return false;
            }

            return elementType.IsAssignableFrom(defaultValueType.GetElementType());
        }

        /// <summary>
        /// Check that every section in structure has unique ID.
        /// </summary>
        /// <param name="structureType">Type describing structure.</param>
        private static void checkSectionUniqueness(Type structureType)
        {
            checkIDUniqueness(StructureFactory.GetSectionProperties(structureType), StructureFactory.ResolveSectionID);
        }

        /// <summary>
        /// Check that every option in section has unique ID.
        /// </summary>
        /// <param name="sectionType">Type describing section.</param>
        private static void checkOptionUniqueness(Type sectionType)
        {
            checkIDUniqueness(StructureFactory.GetOptionProperties( sectionType), StructureFactory.ResolveOptionID);
        }

        /// <summary>
        /// Check that ids produced by resolver are unique.
        /// </summary>
        /// <param name="properties">Properties that will be checked.</param>
        /// <param name="resolver">Resolver which produce id from traversed properties.</param>
        private static void checkIDUniqueness(IEnumerable<PropertyInfo> properties, IDResolver resolver)
        {
            var ids = new HashSet<string>();
            foreach (var property in properties)
            {
                var id = resolver(property);
                if (!ids.Add(id))
                {
                    throw new IDValidationException(
                       userMsg: "ID '{0}' detected on property '{1}' is duplicit",
                       developerMsg: "StructureValidation::checkIDUniqueness ID '{0}' detected on property '{1}' has already been assigned to another property",
                       validatedID: id,
                       validatedProperty: property
                   );
                }
            }
        }

        /// <summary>
        /// Check that ids produced by resolver has valid format.
        /// </summary>
        /// <param name="properties">Properties that will be checked.</param>
        /// <param name="resolver">Resolver which produce id from traversed properties.</param>
        private static void checkIDsValidity(IEnumerable<PropertyInfo> properties, IDResolver resolver)
        {
            foreach (var property in properties)
            {
                var id = resolver(property);
                if (!isValidID(id))
                {
                    throw new IDValidationException(
                        userMsg: "ID '{0}' detected on property '{1}' has incorrect format",
                        developerMsg: "StructureValidation::checkIDsValidity ID '{0}' validation failed due to incorrect format in InfoAttribute on property '{1}'",
                        validatedID: id,
                        validatedProperty: property
                    );
                }
            }
        }

        /// <summary>
        /// Determine that given id has valid format.
        /// </summary>
        /// <param name="id">Validated id.</param>
        /// <returns>True if id has valid format, false otherwise.</returns>
        private static bool isValidID(string id)
        {
            if (id == null || id.Length == 0)
            {
                return false;
            }

            var match = IDValidator.Match(id);

            return match.Success;
        }

        /// <summary>
        /// Check that type contains valid constructs only. For sections deep implemented interface search is proceeded.
        /// </summary>
        /// <param name="isSection">Determine that structureType describes section.</param>
        /// <param name="structureType">Type which signature will be checked.</param>
        private static void checkSignature(Type structureType,bool isSection)
        {
            checkSignatureSingle(structureType);

            if (!isSection)
            { 
                //only sections has to inherit multiple interfaces
                return;
            }

            foreach (var implementedType in structureType.GetInterfaces())
            {
                checkSignature(implementedType, isSection);
            }
        }

        /// <summary>
        /// Check that type contains valid constructs only.
        /// </summary>
        /// <param name="structureType">Type which signature will be checked.</param>
        private static void checkSignatureSingle(Type structureType)
        {

            if (!structureType.IsInterface)
            {
                throw new TypeValidationException(
                       userMsg: "Type describing configuration structure has to be public interface",
                       developerMsg: "StructureValidation::checkSignature failed because structure type isn't interface",
                       validatedType: structureType
                       );
            }


            if (!structureType.IsPublic)
            {
                throw new TypeValidationException(
                     userMsg: "Type describing configuration structure has to be public",
                     developerMsg: "StructureValidation::checkSignature failed because structure type isn't public",
                     validatedType: structureType
                     );
            }

            //NOTE: all methods are public because we are in interface
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
                     developerMsg: string.Format("StructureValidation::checkSignature failed because method '{0}' without assigned property has been found in structure type",method),
                     validatedType: structureType
                     );
                }
            }
        }
    }
}
