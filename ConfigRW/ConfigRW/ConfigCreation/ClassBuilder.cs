using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

using System.Runtime.CompilerServices;


[assembly: InternalsVisibleTo(ConfigRW.ConfigCreation.ReflectionUtils.DynamicAssembly_Name)]

namespace ConfigRW.ConfigCreation
{
    /// <summary>
    /// Class builder, which can build type from given interface and ParentClass
    /// </summary>
    /// <typeparam name="ParentClass">ParentClass which will be used as base type of builded type.</typeparam>
    internal class ClassBuilder<ParentClass>
        where ParentClass : PropertyStorage
    {
        /// <summary>
        /// type builder which is used for building type
        /// </summary>
        private TypeBuilder _typeBuilder;

        /// <summary>
        /// Create class builder, which can build type from given interfaceToImplement and PropertyStorage.
        /// </summary>
        /// <param name="interfaceToImplement">Interface that will be implemented in builded type.</param>
        public ClassBuilder(Type interfaceToImplement)
        {
            //prepare storage for created type
            var assemblyName = new AssemblyName(ReflectionUtils.DynamicAssembly_Name);
            var assemBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemBuilder.DefineDynamicModule(ReflectionUtils.DynamicModule_Name);

            //prepare type builder
            var typeName = ReflectionUtils.DynamicType_Prefix + interfaceToImplement.Name;
            _typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Class, typeof(ParentClass));
            _typeBuilder.AddInterfaceImplementation(interfaceToImplement);
        }

        /// <summary>
        /// Add property to builded type.
        /// </summary>
        /// <param name="name">Name of property.</param>
        /// <param name="type">Type of property.</param>
        public void AddProperty(string name, Type type)
        {
            var fieldName = ReflectionUtils.DynamicField_Prefix + name;
            var storageField = _typeBuilder.DefineField(fieldName, type, FieldAttributes.Private);

            var propertyBuilder = _typeBuilder.DefineProperty(name, PropertyAttributes.None, type, null);


            var getter = buildGetter(type, name, storageField);
            var setter = buildSetter(type, name, storageField);

            propertyBuilder.SetSetMethod(setter);
            propertyBuilder.SetGetMethod(getter);
        }
        
        /// <summary>
        /// Build type from class.
        /// </summary>
        /// <returns>Builded type.</returns>
        public Type Build()
        {
            Type type = _typeBuilder.CreateType();
            return type;
        }

        /// <summary>
        /// Build getter for specified property.
        /// </summary>
        /// <param name="propertyType">Property type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="storageField">Field where value is stored.</param>
        /// <returns>Method builder with getter code.</returns>
        private MethodBuilder buildGetter(Type propertyType, string propertyName, FieldBuilder storageField)
        {
            var getterName = ReflectionUtils.Getter_Prefix + propertyName;
            var getter = _typeBuilder.DefineMethod(getterName, ReflectionUtils.PropertyMethod_Attributes, propertyType, Type.EmptyTypes);

            ILGenerator getIL = getter.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);                    //load this
            getIL.Emit(OpCodes.Ldfld, storageField);        //get storage field
            getIL.Emit(OpCodes.Ret);                        //return value from storage field

            return getter;
        }

        /// <summary>
        /// Build setter for specified property.
        /// </summary>
        /// <param name="propertyType">Property type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="storageField">Field where value is stored.</param>
        /// <returns>Method builder with setter code.</returns>
        private MethodBuilder buildSetter(Type propertyType, string propertyName, FieldBuilder storageField)
        {
            var setterName = ReflectionUtils.Setter_Prefix + propertyName;
            var setterParams = new Type[] { propertyType };
            var setter = _typeBuilder.DefineMethod(setterName, ReflectionUtils.PropertyMethod_Attributes, null, setterParams);

            ILGenerator setIL = setter.GetILGenerator();

            setIL.Emit(OpCodes.Ldarg_0);                        //load this for call
            setIL.Emit(OpCodes.Ldstr, propertyName);            //load property name

            setIL.Emit(OpCodes.Ldarg_0);                        //load this for field load
            setIL.Emit(OpCodes.Ldflda, storageField);           //load address of storage field

            setIL.Emit(OpCodes.Ldarg_1);                        //load value to be stored

            var genericParams = new Type[] { propertyType };
            var storageSetter = ReflectionUtils.PropertyStorage_Setter.MakeGenericMethod(genericParams);

            setIL.EmitCall(OpCodes.Call, storageSetter, null);  //call to storage provider
            setIL.Emit(OpCodes.Ret);                            //return from setter
            return setter;
        }
    }
}
