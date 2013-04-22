using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using ConfigReader.Parsing;

namespace ConfigReader.ConfigCreation
{
    static class InfoUtils
    {
        internal static StructureInfo CreateStructureInfo(Type structureType)
        {

            var sections = new List<SectionInfo>();
            foreach (var sectionProperty in GetSectionProperties(structureType))
            {
                var sectionInfo = CreateSectionInfo(sectionProperty);
                sections.Add(sectionInfo);
            }

            return new StructureInfo(sections);
        }

        internal static SectionInfo CreateSectionInfo(PropertyInfo sectionProperty)
        {
            var options = new List<OptionInfo>();

            var propertyAttribs=sectionProperty.GetCustomAttributes();
            var infoAttr = GetAttribute<SectionInfoAttribute>(propertyAttribs);
            var commentAttr = GetAttribute<DefaultCommentAttribute>(propertyAttribs);

            var sectionID = infoAttr.GetSectionID(sectionProperty);
            var sectionName = new QualifiedSectionName(sectionID);

            foreach (var optionProperty in GetOptionProperties(sectionProperty.PropertyType))
            {
                var optionInfo = CreateOptionInfo(sectionName, optionProperty);
                options.Add(optionInfo);
            }

            return new SectionInfo(sectionName, options, commentAttr.CommentText);
        }

        internal static OptionInfo CreateOptionInfo(QualifiedSectionName sectionName, PropertyInfo optionProperty)
        {
            var propertyAttribs=optionProperty.GetCustomAttributes();
            var infoAttr = GetAttribute<OptionInfoAttribute>(propertyAttribs);
            var commentAttr = GetAttribute<DefaultCommentAttribute>(propertyAttribs);

            var optionID = infoAttr.GetOptionID(optionProperty);
            var optionName = new QualifiedOptionName(sectionName, optionID);

            var expectedType = optionProperty.PropertyType;

            return new OptionInfo(optionName, expectedType, 
                infoAttr.DefaultValue,infoAttr.IsOptional,
                commentAttr.CommentText);
        }

        internal static AttributeType GetAttribute<AttributeType>(IEnumerable<Attribute> attributes)
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

    }
}
