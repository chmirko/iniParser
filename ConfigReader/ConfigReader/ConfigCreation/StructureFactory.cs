using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Reflection;

using ConfigReader.Parsing;

namespace ConfigReader.ConfigCreation
{
    static class StructureFactory
    {
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

            var propertyAttribs=sectionProperty.GetCustomAttributes(false);
            var infoAttr = GetAttribute<SectionInfoAttribute>(propertyAttribs);
            var commentAttr = GetAttribute<DefaultCommentAttribute>(propertyAttribs);

            var sectionID = ResolveID(infoAttr.ID, sectionProperty);
            var sectionName = new QualifiedSectionName(sectionID);

            foreach (var optionProperty in GetOptionProperties(sectionProperty.PropertyType))
            {
                var optionInfo = CreateOptionInfo(sectionName, optionProperty);
                options.Add(optionInfo);
            }

            return new SectionInfo(sectionName,sectionProperty, options, commentAttr.CommentText);
        }

        internal static OptionInfo CreateOptionInfo(QualifiedSectionName sectionName, PropertyInfo optionProperty)
        {
            var propertyAttribs=optionProperty.GetCustomAttributes(false);
            var infoAttr = GetAttribute<OptionInfoAttribute>(propertyAttribs);
            var commentAttr = GetAttribute<DefaultCommentAttribute>(propertyAttribs);
            var rangeAttr = GetAttribute<RangeAttribute>(propertyAttribs);


            var optionID = ResolveID(infoAttr.ID,optionProperty);
            var optionName = new QualifiedOptionName(sectionName, optionID);

            var expectedType = optionProperty.PropertyType;

            var defaultValue = createDefaultObject(infoAttr.DefaultValue, expectedType);

            return new OptionInfo(
                optionName, expectedType, optionProperty.Name,
                defaultValue,infoAttr.IsOptional,
                commentAttr.CommentText,
                rangeAttr.LowerBound,rangeAttr.UpperBound
                );
        }

        /// <summary>
        /// Creates container with appropriate type for option. Container will contains given elements.
        /// </summary>
        /// <param name="option"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        internal static object CreateContainer(OptionInfo option, IEnumerable<object> elements)
        {
            throw new NotImplementedException();
        }

        private static object createDefaultObject(object defaultValue, Type expectedType)
        {
            if (defaultValue == null){
                return null;
            }

            var defaultValueType = defaultValue.GetType();

            if (!defaultValueType.IsArray)
            {
                //only arrays can be expanded into collections
                return defaultValue;
            }

            var elementType=defaultValueType.GetElementType();

            if(expectedType.IsArray){
                //copy array
                var defaultArray = defaultValue as Array;

                return defaultArray.Clone();                
            }

        //    var collectionType = typeof(ICollection<>).MakeGenericType(defaultValueType.GetElementType());

            return Activator.CreateInstance(expectedType, new object[] { defaultValue});
        }

        internal static string ResolveID(string id, PropertyInfo info)
        {
            if (id == null)
                return info.Name;

            return id;
        }


        internal static AttributeType GetAttribute<AttributeType>(IEnumerable<object> attributes)
            where AttributeType:Attribute
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
    }
}
