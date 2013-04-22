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

namespace ConfigReader
{



    internal class ClassBuilder<ParentClass>
        where ParentClass : PropertyStorage
    {


        private static readonly MethodInfo GetterProvider = typeof(PropertyStorage).GetMethod("Getter", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo SetterProvider = typeof(PropertyStorage).GetMethod("Setter", BindingFlags.Instance | BindingFlags.NonPublic);

        TypeBuilder _typeBuilder;

        public ClassBuilder(Type implementedInterface)
        {
            AssemblyName assemblyName = new AssemblyName("DataBuilderAssembly");
            AssemblyBuilder assemBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

            ModuleBuilder moduleBuilder = assemBuilder.DefineDynamicModule("DataBuilderModule");

            _typeBuilder = moduleBuilder.DefineType("NewClass", TypeAttributes.Class, typeof(ParentClass));
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
            getIL.Emit(OpCodes.Ldarg_0);
/*            getIL.Emit(OpCodes.Ldstr, name);
            getIL.EmitCall(OpCodes.Call, GetterProvider.MakeGenericMethod(genericParams), null);*/
            getIL.Emit(OpCodes.Ldfld,field);
            getIL.Emit(OpCodes.Ret);

            var setter = _typeBuilder.DefineMethod("set_" + name, getSetAttr, null, new Type[] { type });

            ILGenerator setIL = setter.GetILGenerator();
        
   
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, field);
   //       setIL.Emit(OpCodes.Stind_Ref);
   //       setIL.EmitCall(OpCodes.Call, SetterProvider.MakeGenericMethod(genericParams), null);
            setIL.Emit(OpCodes.Ret);



            propertyBuilder.SetSetMethod(setter);
            propertyBuilder.SetGetMethod(getter);
        }


    }
}
