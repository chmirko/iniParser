using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigReader.ConfigCreation
{
    /// <summary>
    /// Interface that is used for creating containers.
    /// </summary>
    internal interface IContainerBuilder
    {
        /// <summary>
        /// Resolve type of element that can be stored in container.
        /// </summary>
        /// <param name="containerType">Type of container, which elementType is resolved.</param>
        /// <returns>Type of element if can be stored in container, null otherwise</returns>
        Type ResolveElementType(Type containerType);
        /// <summary>
        /// Create container of given type, that contains given elements.
        /// NOTE: is guaranteed that ResolveElementType won't return null on containerType.
        /// </summary>
        /// <param name="containerType">Type of container that will be created.</param>
        /// <param name="elements">Elements that will be stored in container.</param>
        /// <returns>Created container.</returns>
        object CreateContainer(Type containerType, IEnumerable<object> elements);

        /// <summary>
        /// Get elements that are stored in container
        /// </summary>
        /// <param name="container">Container which elements will be returned.</param>
        /// <returns>Elements that are contained in container.</returns>
        IEnumerable<object> GetElements(object container);
    }
}
