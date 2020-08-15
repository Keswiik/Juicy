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
        /// Invokes a method by name.
        /// </summary>
        /// <remarks>
        /// This is mainly intended to call void methods. Other methods can be called, but their return values will be lost.
        /// </remarks>
        /// <param name="name">The name of the method.</param>
        /// <param name="instance">The object to call the method on.</param>
        /// <param name="parameters">The parameters to call the method with.</param>
        public void Invoke(string name, object instance, params object[] parameters) {
            Type type = instance.GetType();
            ICachedMethod method = GetAndValidateMethod(name, type, parameters);

            method.Invoke(instance, parameters);
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
        /// Instantiates a type with the given parameters.
        /// </summary>
        /// <remarks>
        /// This will attempt to find a constructor that matches the provided <paramref name="parameters"/>.
        /// </remarks>
        /// <param name="type">The type to instantiate.</param>
        /// <param name="parameters">The parameters to instantiate the object with.</param>
        /// <returns>The new object.</returns>
        public object Instantiate(Type type, params object[] parameters)
        {
            CacheType(type);
            ICachedMethod method = GetMatchingMethod(parameters, typeCache[type].Constructors);
            if (method == null)
            {
                throw new InvalidOperationException($"Type {type} had no constructor matching parameters ");
            }

            if (!type.IsAssignableFrom(method.ReturnType))
            {
                throw new InvalidOperationException($"Constructor return type does not match type {type}");
            }
            
            return method.Invoke(null, parameters);
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

        /// <summary>
        /// Gets all constructors attributed with <paramref name="attribute"/>
        /// </summary>
        /// <param name="type">The type to find constructors within.</param>
        /// <param name="attribute">The attribute to look for.</param>
        /// <returns></returns>
        public List<ICachedMethod> GetAttributedConstructors(Type type, Type attribute)
        {
            CacheType(type);

            return typeCache[type].Constructors.FindAll(c => c.HasAttribute(attribute));
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

        private ICachedMethod GetAndValidateMethod(string name, Type type, object[] parameters) {
            CacheType(type);
            ICachedType cachedType = typeCache[type];

            if (!cachedType.Methods.ContainsKey(name)) {
                throw new InvalidOperationException($"Type ${type} does not contain a definition for method {name}");
            }

            ICachedMethod matchingMethod = GetMatchingMethod(parameters, cachedType.Methods[name]);
            if (matchingMethod == null) {
                throw new InvalidOperationException($"Method {name} in {type} does not match parameters {parameters.Select(p => p.GetType())}");
            }

            return matchingMethod;
        }

        private ICachedMethod GetMatchingMethod(object[] parameters, List<ICachedMethod> methods) {
            if (methods.Count == 1) {
                return methods[0];
            }

            Type[] types = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++) {
                types[i] = parameters[i].GetType();
            }

            foreach (ICachedMethod method in methods) {
                if (method.Parameters.Count != types.Length) {
                    continue;
                }

                bool isMatch = true;
                for (int i = 0; i < types.Length; i++) {
                    if (!types[i].Equals(method.Parameters[i].Type)) {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch) {
                    return method;
                }
            }

            return null;
        }

        private void CacheType(Type type) {
            if (typeCache.ContainsKey(type)) {
                return;
            }

            typeCache[type] = cachedTypeFactory.GetCachedType(type);
        }

        #endregion Private methods
    }
}