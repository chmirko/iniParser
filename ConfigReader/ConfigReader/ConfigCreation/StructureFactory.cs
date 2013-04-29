using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Reflection;

using ConfigReader.Parsing;
using ConfigReader.ConfigCreation.ContainerBuilders;

namespace ConfigReader.ConfigCreation
{
    static class StructureFactory
    {
        static IEnumerable<IContainerBuilder> _containerBuilders = new List<IContainerBuilder>()
            {
                new ArrayBuilder(),
                new ListCompatibleBuilder(),                
                new CollectionBuilder()
            };


        internal static StructureInfo CreateStructureInfo(Type structureType)
        {
            var sections = new List<SectionInfo>();
            foreach (var sectionProperty in GetSectionProperties(structureType))
            {
                var sectionInfo = CreateSectionInfo(sectionProperty);
                sections.Add(sectionInfo);
            }

            return new StructureInfo(structureType, sections);
        }

        internal static SectionInfo CreateSectionInfo(PropertyInfo sectionProperty)
        {
            var options = new List<OptionInfo>();

            var propertyAttribs = sectionProperty.GetCustomAttributes(false);
            var infoAttr = GetAttribute<SectionInfoAttribute>(propertyAttribs);
            var commentAttr = GetAttribute<DefaultCommentAttribute>(propertyAttribs);

            var sectionID = ResolveID(infoAttr.ID, sectionProperty);
            var sectionName = new QualifiedSectionName(sectionID);

            foreach (var optionProperty in GetOptionProperties(sectionProperty.PropertyType))
            {
                var optionInfo = CreateOptionInfo(sectionName, optionProperty);
                options.Add(optionInfo);
            }

            return new SectionInfo(sectionName, sectionProperty, options, commentAttr.CommentText);
        }

        internal static OptionInfo CreateOptionInfo(QualifiedSectionName sectionName, PropertyInfo optionProperty)
        {
            var propertyAttribs = optionProperty.GetCustomAttributes(false);
            var infoAttr = GetAttribute<OptionInfoAttribute>(propertyAttribs);
            var commentAttr = GetAttribute<DefaultCommentAttribute>(propertyAttribs);
            var rangeAttr = GetAttribute<RangeAttribute>(propertyAttribs);


            var optionID = ResolveID(infoAttr.ID, optionProperty);
            var optionName = new QualifiedOptionName(sectionName, optionID);

            var expectedType = optionProperty.PropertyType;

            var defaultValue = createDefaultObject(infoAttr.DefaultValue, expectedType);

            return new OptionInfo(
                optionName, expectedType, optionProperty.Name,
                defaultValue, infoAttr.IsOptional,
                commentAttr.CommentText,
                rangeAttr.LowerBound, rangeAttr.UpperBound
                );
        }

        /// <summary>
        /// Creates container with appropriate type for option. Container will contains given elements.
        /// </summary>
        /// <param name="option">Option where container will be stored.</param>
        /// <param name="elements">Elements that will be stored in container.</param>
        /// <returns>Container which is valid for given option.</returns>
        internal static object CreateContainer(OptionInfo option, IEnumerable<object> elements)
        {
            var builder = GetContainerBuilder(option.ExpectedType);

            return builder.CreateContainer(option.ExpectedType, elements);
        }

        /// <summary>
        /// Get elements which are contained in container.
        /// </summary>
        /// <param name="container">Container which elements will be returned.</param>
        /// <returns>Elements that are contained in container.</returns>
        internal static IEnumerable<object> GetContainerElements(object container)
        {
            var builder = GetContainerBuilder(container.GetType());

            return builder.GetElements(container);
        }

        private static object createDefaultObject(object defaultValue, Type expectedType)
        {
            if (defaultValue == null)
            {
                //there is no default value
                return null;
            }

            var defaultValueType = defaultValue.GetType();
            if (!defaultValueType.IsArray)
            {
                //only arrays will be expanded into containers
                return defaultValue;
            }

            var builder = GetContainerBuilder(expectedType);
            if (builder == null)
            {
                throw new NotSupportedException("Missing builder for container type");
            }

            var defaultElements = (defaultValue as Array).Cast<object>();            
            return builder.CreateContainer(expectedType, defaultElements);
        }

        /// <summary>
        /// Get type of elements which can be stored in type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static Type GetElementType(Type type)
        {
            var builder = GetContainerBuilder(type);
            if (builder == null)
                return null;

            return builder.ResolveElementType(type);
        }

        /// <summary>
        /// Get container builder for given containerType.
        /// </summary>
        /// <param name="containerType">Type of container which will be created.</param>
        /// <returns>Container builder for given containerType. If none exists returns null.</returns>
        internal static IContainerBuilder GetContainerBuilder(Type containerType)
        {
            foreach (var builder in _containerBuilders)
            {
                if (builder.ResolveElementType(containerType) != null)
                {
                    return builder;
                }
            }
            return null;
        }


        internal static string ResolveID(string id, PropertyInfo info)
        {
            if (id == null)
                return info.Name;

            return id;
        }


        internal static AttributeType GetAttribute<AttributeType>(IEnumerable<object> attributes)
            where AttributeType : Attribute
        {
            foreach (var attribute in attributes)
            {
                if (attribute is AttributeType)
                {
                    return attribute as AttributeType;
                }
            }

            return Activator.CreateInstance<AttributeType>();
        }

        internal static IEnumerable<PropertyInfo> GetSectionProperties(Type structureType)
        {
            return structureType.GetProperties();
        }

        internal static IEnumerable<PropertyInfo> GetOptionProperties(Type sectionType)
        {
            return sectionType.GetProperties();
        }


        internal static string ResolveSectionID(PropertyInfo property)
        {
            var info = GetAttribute<SectionInfoAttribute>(property.GetCustomAttributes(false));
            return ResolveID(info.ID, property);
        }

        internal static string ResolveOptionID(PropertyInfo property)
        {
            var info = GetAttribute<OptionInfoAttribute>(property.GetCustomAttributes(false));
            return ResolveID(info.ID, property);
        }


        internal static string GetNonGenericName(Type type)
        {
            return type.Namespace + "." + type.Name;
        }

        internal static bool InterfaceMatch(Type type1, Type type2)
        {
            var n1 = GetNonGenericName(type1);
            var n2 = GetNonGenericName(type2);
            return n1 == n2;            
        }
    }
}
