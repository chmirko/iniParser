using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigReader.ConfigCreation.ContainerBuilders
{
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
            var container=Array.CreateInstance(containerType.GetElementType(), elements.Count());

            //fill container
            int position=0;
            foreach (var element in elements)
            {
                container.SetValue(element, position);
                ++position;
            }

            return container;
        }


        public IEnumerable<object> GetElements(object container)
        {
            return (container as Array).Cast<object>();
        }
    }
}
