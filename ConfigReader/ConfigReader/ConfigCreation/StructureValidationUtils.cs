using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.ConfigCreation
{
    static class StructureValidationUtils
    {
        static internal void ThrowOnInvalid(Type structureType)
        {
            checkSignature(structureType);
            checkSectionUniqueness(structureType);

            foreach (var property in structureType.GetProperties())
            {
                var optionType = property.PropertyType;
                checkSignature(optionType);
                checkOptionUniqueness(optionType);
            }
        }

        private static void checkOptionUniqueness(Type optionType)
        {
            checkIDUniqueness(optionType, InfoUtils.ResolveOptionID);
        }

        private static void checkSectionUniqueness(Type sectionType)
        {
            checkIDUniqueness(sectionType, InfoUtils.ResolveSectionID);
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
