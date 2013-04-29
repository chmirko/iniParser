using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace ConfigReader.ConfigCreation.ContainerBuilders
{
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
                if(!StructureFactory.InterfaceMatch(implemented,typeof(ICollection<>)))
                    continue;
                                
                return implemented.GetGenericArguments()[0];
            }
            return null;
        }

        public object CreateContainer(Type containerType, IEnumerable<object> elements)
        {
            var container = Activator.CreateInstance(containerType);

            foreach (var el in elements)
            {
                containerType.GetMethod("Add").Invoke(container, new object[]{el});
            }

            return container;
        }


        public IEnumerable<object> GetElements(object container)
        {
            return (container as IEnumerable).Cast<object>();            
        }
    }
}
