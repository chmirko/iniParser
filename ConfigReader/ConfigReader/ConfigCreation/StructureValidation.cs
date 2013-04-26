using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Reflection;

namespace ConfigReader.ConfigCreation
{

    internal delegate string IDResolver(PropertyInfo property);

    static class StructureValidation
    {
        static internal void ThrowOnInvalid(Type structureType)
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

        private static void checkOptionUniqueness(Type optionType)
        {
            checkIDUniqueness(optionType, StructureFactory.ResolveOptionID);
        }

        private static void checkSectionUniqueness(Type sectionType)
        {
            checkIDUniqueness(sectionType, StructureFactory.ResolveSectionID);
        }

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
