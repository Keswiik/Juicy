using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace Juicy.Reflection {

    /// <summary>
    /// A utility class to abstract away most of the overhead that comes with using reflection.
    /// </summary>
    /// <remarks>
    /// Is this actually harder? Who knows.
    /// </remarks>
    public sealed class Reflector {
        private readonly CachedTypeFactory cachedTypeFactory;

        private readonly Dictionary<Type, ICachedType> typeCache;

        /// <summary>
        /// Creates a new <see cref="Reflector"/>.
        /// </summary>
        public Reflector() {
            cachedTypeFactory = new CachedTypeFactory();
            typeCache = new Dictionary<Type, ICachedType>();
        }

        /// <summary>
        /// Invokes a method directly, using its cached information instead of by name.
        /// </summary>
        /// <param name="method">The cached method information.</param>
        /// <param name="instance">The object to call the method on.</param>
        /// <param name="parameters">The parameters to call the method with.</param>
        /// <returns>The object returned by the method, or <c>null</c> if nothing was returned.</returns>
        public object Invoke(ICachedMethod method, object instance, params object[] parameters) {
            return method.Invoke(instance, parameters);
        }

        /// <summary>
        /// Gets all methods attributed with <paramref name="attribute"/>.
        /// </summary>
        /// <remarks>
        /// This includes instance methods and static methods. It <b>excludes</b> constructors.
        /// </remarks>
        /// <param name="type">The type to find methods within.</param>
        /// <param name="attribute">The attribute to look for.</param>
        /// <returns>A list of cached method information which may be empty.</returns>
        public List<ICachedMethod> GetAttributedMethods(Type type, Type attribute)
        {
            CacheType(type);

            List<ICachedMethod> methods = new List<ICachedMethod>();
            ICachedType cachedType = typeCache[type];
            foreach (string key in cachedType.Methods.Keys)
            {
                methods.AddRange(cachedType.Methods[key].FindAll(m => m.HasAttribute(attribute)));
            }

            return methods;
        }

        public ICachedMethod GetInjectableConstructor(Type type) {
            CacheType(type);
            return typeCache[type].InjectableConstructor;
        }

        /// <summary>
        /// Gets the default constructor - the constructor with no parameters.
        /// </summary>
        /// <param name="type">The type to look for a default constructor in.</param>
        /// <returns>The cached method information which may be <c>null</c>.</returns>
        public ICachedMethod GetDefaultConstructor(Type type) {
            CacheType(type);

            // TODO: maybe refactor type cache to separate default constructor.
            return typeCache[type].Constructors.Find(c => c.Parameters.Count == 0);
        }

        #region Private methods

        private void CacheType(Type type) {
            if (typeCache.ContainsKey(type)) {
                return;
            }

            typeCache[type] = cachedTypeFactory.GetCachedType(type);
        }

        #endregion Private methods
    }
}