using System.Collections.Generic;
using System.Linq;
using System;

namespace EDFileSystem.Loader
{
    public class FileSystemCache
    {
        public const int defaultExpiryMS = 4000;
        public readonly DateTime Expiry;

        private ElementType elementType = ElementType.None;
        public Element[] Contents { get; private set; }
        public FileSystemCache(ElementType elementType)
        {
            Expiry = DateTime.Now.AddMilliseconds(defaultExpiryMS);
        }

        public IEnumerable<Element> Store(IEnumerable<Element> elements)
        {
            if (elements == null || elements?.Length == 0)
                throw new ArgumentException("Expected an array of Elements in call to FileSystemCache.Store, got null or empty");

            var firstElementFullName = elements.FirstOrDefault().GetType().FullName;
            if (!Enum.TryParse(firstElementFullName, out ElementType elType))
                throw new NotImplementedException(
                    $"Element of type \"{firstElementFullName}\" is not implemented in call to FileSystemCache.Store"
                );

            // TODO: DEEP-COPY array here
            // Contents = elements

            Contents = elements.Select(el => el.Clone());

            return Contents;
        }
    }
}