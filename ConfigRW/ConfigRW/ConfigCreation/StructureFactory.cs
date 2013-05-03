using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Reflection;

using ConfigRW.Parsing;
using ConfigRW.ConfigCreation.ContainerBuilders;

namespace ConfigRW.ConfigCreation
{
    /// <summary>
    /// Factory for structure
    /// </summary>
    static class StructureFactory
    {
        /// <summary>
        /// Container builders which are available for instantiating containers. 
        /// </summary>
        static IEnumerable<IContainerBuilder> _containerBuilders = new List<IContainerBuilder>()
            {
                new ArrayBuilder(),
                new ListCompatibleBuilder(),                
                new CollectionBuilder()
            };


        /// <summary>
        /// Create structure info described by structureType.
        /// </summary>
        /// <param name="structureType">Type describing structure.</param>
        /// <returns>Structure info.</returns>
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

        /// <summary>
        /// Create section info described by sectionProperty.
        /// </summary>
        /// <param name="sectionProperty">Property describing section.</param>
        /// <returns>Section info.</returns>
        internal static SectionInfo CreateSectionInfo(PropertyInfo sectionProperty)
        {
            var options = new List<OptionInfo>();
                        
            var infoAttr = ReflectionUtils.GetAttribute<SectionInfoAttribute>(sectionProperty);
            var commentAttr = ReflectionUtils.GetAttribute<DefaultCommentAttribute>(sectionProperty);

            var sectionID = resolveID(infoAttr.ID, sectionProperty);
            var sectionName = new QualifiedSectionName(sectionID);

            foreach (var optionProperty in GetOptionProperties(sectionProperty.PropertyType))
            {
                var optionInfo = CreateOptionInfo(sectionName, optionProperty);
                options.Add(optionInfo);
            }

            return new SectionInfo(sectionName, sectionProperty, options, commentAttr.CommentText);
        }

        /// <summary>
        /// Create option info described by optionProperty. Option is placed in section with sectionName.
        /// </summary>
        /// <param name="sectionName">Name of parent section.</param>
        /// <param name="optionProperty">Property describing option.</param>
        /// <returns>Option info.</returns>
        internal static OptionInfo CreateOptionInfo(QualifiedSectionName sectionName, PropertyInfo optionProperty)
        {
          
            var infoAttr = ReflectionUtils.GetAttribute<OptionInfoAttribute>(optionProperty);
            var commentAttr = ReflectionUtils.GetAttribute<DefaultCommentAttribute>(optionProperty);
            var rangeAttr = ReflectionUtils.GetAttribute<RangeAttribute>(optionProperty);


            var optionID = resolveID(infoAttr.ID, optionProperty);
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
            if (container == null)
            {
                return new object[0];
            }

            var builder = GetContainerBuilder(container.GetType());
            return builder.GetElements(container);
        }



        /// <summary>
        /// Get type of elements which can be stored in containerType
        /// </summary>
        /// <param name="containerType">Type of container.</param>
        /// <returns>Type of elements if containerType is valid known container, null otherwise.</returns>
        internal static Type GetElementType(Type containerType)
        {
            var builder = GetContainerBuilder(containerType);
            if (builder == null)
            {
                return null;
            }

            return builder.ResolveElementType(containerType);
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

        /// <summary>
        /// Get properties that describe sections. 
        /// </summary>
        /// <param name="structureType">Type describing configuration structure.</param>
        /// <returns>Section describing properties.</returns>
        internal static IEnumerable<PropertyInfo> GetSectionProperties(Type structureType)
        {
            return structureType.GetProperties();
        }

        /// <summary>
        /// Get properties that describe options.
        /// </summary>
        /// <param name="sectionType">Type describing section.</param>
        /// <returns>Option describing properties.</returns>
        internal static IEnumerable<PropertyInfo> GetOptionProperties(Type sectionType)
        {
            return sectionType.GetProperties();
        }

        /// <summary>
        /// Resolve id of section.
        /// </summary>
        /// <param name="sectionProperty">Property describing section.</param>
        /// <returns>Id of section.</returns>
        internal static string ResolveSectionID(PropertyInfo sectionProperty)
        {
            var info = ReflectionUtils.GetAttribute<SectionInfoAttribute>(sectionProperty);
            return resolveID(info.ID, sectionProperty);
        }

        /// <summary>
        /// Resolve id of option.
        /// </summary>
        /// <param name="optionProperty">Property describing option.</param>
        /// <returns>Id of option.</returns>
        internal static string ResolveOptionID(PropertyInfo optionProperty)
        {
            var info = ReflectionUtils.GetAttribute<OptionInfoAttribute>(optionProperty);
            return resolveID(info.ID, optionProperty);
        }

        /// <summary>
        /// Resolve id from given property.
        /// </summary>
        /// <param name="preferedID">Id which is prefered.</param>
        /// <param name="property">Which name can be used as Id.</param>
        /// <returns>Prefered id if possible, id from property otherwise.</returns>
        private static string resolveID(string preferedID, PropertyInfo property)
        {
            if (preferedID == null)
                return property.Name;

            return preferedID;
        }

        /// <summary>
        /// Create object that can be used as option default.
        /// </summary>
        /// <param name="defaultValue">Default value specified by user.</param>
        /// <param name="expectedType">Type that option expects.</param>
        /// <returns>Default value object.</returns>
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
                throw new ContainerBuildException(
                    userMsg: "Cannot create default object for container option",
                    developerMsg: "StructureFactory::createDefaultObject missing builder for container type",
                    containerType: expectedType

                    );
            }

            var defaultElements = (defaultValue as Array).Cast<object>();
            return builder.CreateContainer(expectedType, defaultElements);
        }

    }
}
