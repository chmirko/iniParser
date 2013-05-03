using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigRW.ConfigCreation.ContainerBuilders
{
    /// <summary>
    /// Build array containers.
    /// </summary>
    class ArrayBuilder:IContainerBuilder
    {
        public Type ResolveElementType(Type containerType)
        {
            if (!containerType.IsArray)
            {
                return null;
            }

            return containerType.GetElementType();
        }

        public object CreateContainer(Type containerType, IEnumerable<object> elements)
        {
            var elementType=containerType.GetElementType();
            var container=Array.CreateInstance(elementType, elements.Count());

            //fill container
            int position=0;
            foreach (var element in elements)
            {
                var converted= convert(element, elementType);
                container.SetValue(converted, position);
                ++position;
            }

            return container;
        }

        private object convert(object element, Type expectedType)
        {
            if(!expectedType.IsEnum){
                //conversions are needed only for enums
                return element;
            }

            return Enum.ToObject(expectedType, element);
        }

        public IEnumerable<object> GetElements(object container)
        {
            return (container as Array).Cast<object>();
        }
    }
}
