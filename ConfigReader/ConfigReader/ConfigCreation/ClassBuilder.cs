using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DataBuilderAssembly")]

namespace ConfigReader.ConfigCreation
{

    internal class ClassBuilder<ParentClass>
        where ParentClass : PropertyStorage
    {
      
        TypeBuilder _typeBuilder;

        public ClassBuilder(Type implementedInterface)
        {
            AssemblyName assemblyName = new AssemblyName("DataBuilderAssembly");
            AssemblyBuilder assemBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

            ModuleBuilder moduleBuilder = assemBuilder.DefineDynamicModule("DataBuilderModule");

            _typeBuilder = moduleBuilder.DefineType("config_"+implementedInterface.Name,TypeAttributes.Class, typeof(ParentClass));
            _typeBuilder.AddInterfaceImplementation(implementedInterface);
        }

        public Type Build()
        {
            Type type = _typeBuilder.CreateType();
            return type;
        }

        public void AddProperty(string name, Type type)
        {
            var field = _typeBuilder.DefineField("property_" + name, type, FieldAttributes.Private);
            var propertyBuilder = _typeBuilder.DefineProperty(name, PropertyAttributes.None, type, null);

            var getSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual;

            var getter = _typeBuilder.DefineMethod("get_" + name, getSetAttr, type, Type.EmptyTypes);


            var genericParams = new Type[] { type };
            ILGenerator getIL = getter.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);        //load this
            getIL.Emit(OpCodes.Ldfld,field);    //get storage field
            getIL.Emit(OpCodes.Ret);            //return value from storage field

            var setter = _typeBuilder.DefineMethod("set_" + name, getSetAttr, null, new Type[] { type });

            ILGenerator setIL = setter.GetILGenerator();
   
            setIL.Emit(OpCodes.Ldarg_0);        //load this for call
            setIL.Emit(OpCodes.Ldstr, name);    //load property name

            setIL.Emit(OpCodes.Ldarg_0);        //load this for field load
            setIL.Emit(OpCodes.Ldflda, field);  //load address of storage field
       
            setIL.Emit(OpCodes.Ldarg_1);        //load value to be stored
       
            //call to storage provider
            setIL.EmitCall(OpCodes.Call, PropertyStorage.SetterProvider.MakeGenericMethod(genericParams), null);
            setIL.Emit(OpCodes.Ret);            //return from setter

            propertyBuilder.SetSetMethod(setter);
            propertyBuilder.SetGetMethod(getter);
        }


    }
}
