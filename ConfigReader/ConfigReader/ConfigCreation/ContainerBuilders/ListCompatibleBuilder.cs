using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace ConfigReader.ConfigCreation.ContainerBuilders
{
    /// <summary>
    /// Satisfy IEnumerable, ICollection interfaces with list.
    /// </summary>
    class ListCompatibleBuilder : IContainerBuilder
    {
        public Type ResolveElementType(Type containerType)
        {
            if (!containerType.IsInterface)
            {
                return null;
            }

            var isEnumerable = StructureFactory.InterfaceMatch(containerType, typeof(IEnumerable<>));
            var isCollection = StructureFactory.InterfaceMatch(containerType, typeof(ICollection<>));

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
            return Activator.CreateInstance(listType, elements);
        }

        public IEnumerable<object> GetElements(object container)
        {
            return (container as IEnumerable).Cast<object>();
        }
    }
}
