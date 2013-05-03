using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace ConfigRW.ConfigCreation.ContainerBuilders
{
    /// <summary>
    /// Build containers for interfaces IEnumerable, ICollection, because they can be satisfied via List.
    /// </summary>
    class ListCompatibleBuilder : IContainerBuilder
    {
        public Type ResolveElementType(Type containerType)
        {
            if (!containerType.IsInterface)
            {
                return null;
            }

            var isEnumerable = ReflectionUtils.NonGenericMatch(containerType, typeof(IEnumerable<>));
            var isCollection = ReflectionUtils.NonGenericMatch(containerType, typeof(ICollection<>));

            if (!isEnumerable && !isCollection)
            {
                return null;
            }

            //contains element type in first generic argument.
            return containerType.GetGenericArguments()[0];
        }

        public object CreateContainer(Type containerType, IEnumerable<object> elements)
        {
            var elementType = ResolveElementType(containerType);

            var listType= typeof(List<>).MakeGenericType(new Type[] { elementType });

            var inputArray = Array.CreateInstance(elementType, elements.Count());

            int index=0;
            foreach (var element in elements)
            {
                inputArray.SetValue(element, index);
                ++index;
            }

            return Activator.CreateInstance(listType,inputArray);
        }

        public IEnumerable<object> GetElements(object container)
        {
            return (container as IEnumerable).Cast<object>();
        }
    }
}
