using Juicy.Reflector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Juicy.Reflector {

    public class Reflector {
        private readonly CachedTypeFactory cachedTypeFactory;

        private readonly Dictionary<Type, ICachedType> typeCache;

        public Reflector() {
            cachedTypeFactory = new CachedTypeFactory();
            typeCache = new Dictionary<Type, ICachedType>();
        }

        public void InvokeMethod(string name, object instance, params object[] parameters) {
            Type type = instance.GetType();
            ICachedMethod method = GetAndValidateMethod(name, type, parameters);

            method.Invoke(instance, parameters);
        }

        public T InvokeMethod<T>(string name, object instance, params object[] parameters) {
            Type type = instance.GetType();
            ICachedMethod method = GetAndValidateMethod(name, type, parameters);

            if (!typeof(T).IsAssignableFrom(method.ReturnType)) {
                throw new InvalidOperationException($"Return type of method {name} cannot be cast to {typeof(T)}");
            }

            return (T)method.Invoke(instance, parameters);
        }

        public T Instantiate<T>(Type type, params object[] parameters)
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
            
            return method.Invoke<T>(null, parameters);
        }

        public List<ICachedMethod> GetAttributedMethods(Type type, Type annotation)
        {
            CacheType(type);

            List<ICachedMethod> methods = new List<ICachedMethod>();
            ICachedType cachedType = typeCache[type];
            foreach (string key in cachedType.Methods.Keys)
            {
                methods.AddRange(cachedType.Methods[key].FindAll(m => m.HasAttribute(annotation)));
            }

            return methods;
        }

        public List<ICachedMethod> GetAttributedConstructors(Type type, Type attribute)
        {
            CacheType(type);

            return typeCache[type].Constructors.FindAll(c => c.HasAttribute(attribute));
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