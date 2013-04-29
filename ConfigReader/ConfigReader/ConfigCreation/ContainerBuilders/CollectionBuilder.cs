using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace ConfigReader.ConfigCreation.ContainerBuilders
{
    /// <summary>
    /// Build containers which implement generic ICollection interface.
    /// </summary>
    class CollectionBuilder:IContainerBuilder
    {
        public Type ResolveElementType(Type containerType)
        {
            if (containerType.IsInterface || containerType.IsAbstract)
            {
                return null;
            }

            if (containerType.GetConstructor(new Type[] { }) == null)
            {
                //we can't instantiate this type
                return null;
            }

            foreach (var implemented in containerType.GetInterfaces())
            {
                if(!ReflectionUtils.NonGenericMatch(implemented,typeof(ICollection<>)))
                    continue;
                                
                return implemented.GetGenericArguments()[0];
            }
            return null;
        }

        public object CreateContainer(Type containerType, IEnumerable<object> elements)
        {
            var container = Activator.CreateInstance(containerType);
            var elementType = StructureFactory.GetElementType(containerType);
            
            foreach (var el in elements)
            {
               ReflectionUtils.CollectionAdd(elementType,container,el);
            }

            return container;
        }


        public IEnumerable<object> GetElements(object container)
        {
            return (container as IEnumerable).Cast<object>();            
        }
    }
}
