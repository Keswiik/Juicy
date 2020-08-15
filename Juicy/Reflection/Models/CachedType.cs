using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;

namespace Juicy.Reflection.Models {

    ///<inheritdoc cref="ICachedType"/>
    internal sealed class CachedType : AttributeHolder, ICachedType {
        private List<ICachedMethod> constructors;

        private Dictionary<string, List<ICachedMethod>> methods;

        /// <summary>
        /// Consumes builder to fill out attributes.
        /// </summary>
        /// <param name="component">The component to pull method information from.</param>
        private  CachedType(ICachedTypeComponent component) : base(component) {
            Constructors = component._Constructors;
            Methods = component._Methods;
            Type = component._Type;
        }

        // TODO: remove unnecessary list creation
        public List<ICachedMethod> Constructors {
            get => new List<ICachedMethod>(constructors);

            set => constructors = value;
        }

        public Dictionary<string, List<ICachedMethod>> Methods {
            get => new Dictionary<string, List<ICachedMethod>>(methods);

            set => methods = value;
        }

        public Type Type { get; private set; }

        #region Builder

        ///<inheritdoc/>
        private interface ICachedTypeComponent : IAttributeHolderComponent {
            List<ICachedMethod> _Constructors { get; }

            Dictionary<string, List<ICachedMethod>> _Methods { get; }

            Type _Type { get; }
        }

        ///<inheritdoc/>
        public class CachedTypeComponent<T> : AttributeHolderComponent<T>, ICachedTypeComponent
                where T : CachedTypeComponent<T> {

            public CachedTypeComponent() {
                _Constructors = new List<ICachedMethod>();
                _Methods = new Dictionary<string, List<ICachedMethod>>();
            }

            public List<ICachedMethod> _Constructors { get; }

            public Dictionary<string, List<ICachedMethod>> _Methods { get; }

            public Type _Type { get; private set; }

            /// <summary>
            /// Adds a constructor to the type.
            /// </summary>
            /// <param name="cachedMethod">The cached method to add.</param>
            /// <returns>The builder.</returns>
            public T Constructor(ICachedMethod cachedMethod) {
                _Constructors.Add(cachedMethod);
                return this as T;
            }

            /// <summary>
            /// Adds multiple constructors to the type.
            /// </summary>
            /// <param name="methods">The methods to add.</param>
            /// <returns>The builder.</returns>
            public T Constructors(params ICachedMethod[] methods) {
                _Constructors.AddRange(methods);
                return this as T;
            }
            
            /// <summary>
            /// Adds a method to the type.
            /// </summary>
            /// <param name="method">The method to add.</param>
            /// <returns>The builder.</returns>
            public T Method(ICachedMethod method) {
                AddMethod(method);
                return this as T;
            }

            /// <summary>
            /// Adds multiple methods to the type.
            /// </summary>
            /// <param name="methods">The methods to add.</param>
            /// <returns>The builder.</returns>
            public T Methods(params ICachedMethod[] methods) {
                foreach (ICachedMethod method in methods) {
                    AddMethod(method);
                }

                return this as T;
            }

            /// <summary>
            /// Sets the type's actual type instance.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns>The builder.</returns>
            public T Type(Type type)
            {
                _Type = type;

                return this as T;
            }

            private void AddMethod(ICachedMethod method) {
                if (!_Methods.ContainsKey(method.Name)) {
                    _Methods[method.Name] = new List<ICachedMethod>();
                }

                _Methods[method.Name].Add(method);
            }
        }

        /// <summary>
        /// Builder used to create new cached types.
        /// </summary>
        new public class Builder : CachedTypeComponent<Builder>, IBuilder<CachedType> {

            public CachedType Build() {
                return new CachedType(this);
            }
        }

        #endregion Builder
    }
}