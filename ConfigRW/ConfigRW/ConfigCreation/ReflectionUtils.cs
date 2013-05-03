using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using System.Reflection;

namespace ConfigRW.ConfigCreation
{
    static class ReflectionUtils
    {

        #region Method info constants

        /// <summary>
        /// Setter method on PropertyStorage class
        /// </summary>
        static internal readonly MethodInfo PropertyStorage_Setter;

        #endregion

        #region Dynamic type creation names

        /// <summary>
        /// Name of assembly where created types are loaded
        /// </summary>
        internal const string DynamicAssembly_Name = "ConfigReader.DynamicAssembly";

        /// <summary>
        /// Name of module where created types are stored
        /// </summary>
        internal const string DynamicModule_Name = "ConfigReader.DynamicModule";

        /// <summary>
        /// Prefix for dynamically created configuration type
        /// </summary>
        internal const string DynamicType_Prefix = "ConfigType_";

        /// <summary>
        /// Prefix for dynamically created field 
        /// </summary>
        internal const string DynamicField_Prefix = "field_";

        /// <summary>
        /// Attributes for dynamically created property method
        /// </summary>
        internal const MethodAttributes PropertyMethod_Attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual;

        /// <summary>
        /// Prefix for getter method.
        /// </summary>
        internal const string Getter_Prefix = "get_";
        /// <summary>
        /// Prefix for setter method.
        /// </summary>
        internal const string Setter_Prefix = "set_";

        #endregion

        /// <summary>
        /// Name of adding method on ICollection interface.
        /// </summary>
        static private readonly string name_Collection_Add = "Add";
        /// <summary>
        /// Name of setter provider method on PropertyStorage class.
        /// </summary>
        static private readonly string name_PropertyStorage_Setter = "Setter";

        static ReflectionUtils()
        {
            //initialize method info
            PropertyStorage_Setter = typeof(PropertyStorage).GetMethod(name_PropertyStorage_Setter, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Invoke Add method on collection overloaded to accept elementType. Element is passed as its argument.
        /// </summary>
        /// <param name="elementType">Type of element in collection.</param>
        /// <param name="collection">Collection object where element will be added.</param>
        /// <param name="element">Element to be added into collection.</param>
        internal static void CollectionAdd(Type elementType, object collection, object element)
        {
            var method = collection.GetType().GetMethod(name_Collection_Add, new Type[] { elementType });
            method.Invoke(collection, new object[] { element });
        }

        /// <summary>
        /// Get full name of type, without generic parameters.
        /// </summary>
        /// <param name="type">Type which names will be returned.</param>
        /// <returns>Full name of given type, without generic parameters.</returns>
        internal static string GetNonGenericName(Type type)
        {
            return type.Namespace + "." + type.Name;
        }

        /// <summary>
        /// Determine that type1 has same full name as type2, without generic parameters.
        /// </summary>
        /// <param name="type1">Type 1.</param>
        /// <param name="type2">Type 2.</param>
        /// <returns>True if types has same non generic name, false otherwise.</returns>
        internal static bool NonGenericMatch(Type type1, Type type2)
        {
            var n1 = GetNonGenericName(type1);
            var n2 = GetNonGenericName(type2);
            return n1 == n2;
        }


        /// <summary>
        /// Get attribute of AttributeType for given info.
        /// </summary>
        /// <typeparam name="AttributeType">Type of attribute that will be returned</typeparam>
        /// <param name="info">Member info which attribute will be searched</param>
        /// <returns>Attribute attached to given info, or default instance of AttributeType</returns>
        internal static AttributeType GetAttribute<AttributeType>(MemberInfo info)
            where AttributeType : Attribute, new()
        {
            var attribs = info.GetCustomAttributes(typeof(AttributeType), true);
            return GetAttribute<AttributeType>(attribs);
        }

        /// <summary>
        /// Get attribute of AttributeType from given attribute objects. If no matching attribute is found, default AttributeType instance is returned.
        /// </summary>
        /// <typeparam name="AttributeType">Type of attribute that will be returned</typeparam>
        /// <param name="attributes">Attribute objects</param>
        /// <returns>Attribute from given attributes, or default instance of AttributeType</returns>
        private static AttributeType GetAttribute<AttributeType>(object[] attributes)
            where AttributeType : Attribute, new()
        {
            foreach (var attribute in attributes)
            {
                if (attribute is AttributeType)
                {
                    //Return attached attribute
                    return (AttributeType)attribute;
                }
            }

            //Create default attribute
            return new AttributeType();
        }
    }
}
