using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

 namespace Juicy.Reflection.Models {

    /// <inheritdoc cref="ICachedMethod"/>
    internal sealed class CachedMethod : AttributeHolder, ICachedMethod {

        private readonly MethodBase methodBase;

        private List<ICachedParameter> parameters;

        public string Name { get; }

        public Type ReturnType { get; }

        /// <summary>
        /// Consumes builder to fill out attributes.
        /// </summary>
        /// <param name="component">The component to pull method information from.</param>
        private CachedMethod(ICachedMethodComponent component) : base(component) {
            Name = component._Name;
            ReturnType = component._ReturnType;
            Parameters = component._Parameters;
            methodBase = component._MethodBase;
        }

        // TODO: remove the unneeded new list creation
        public List<ICachedParameter> Parameters {
            get => new List<ICachedParameter>(parameters);

            set => parameters = value;
        }

        public T Invoke<T>(object instance, params object[] args) {
            Type type = typeof(T);
            if (!type.IsAssignableFrom(ReturnType)) {
                throw new InvalidOperationException($"Cannot invoke method {Name}, return type of {ReturnType.Name} cannot be cast to {type.Name}");
            }

            object value = Invoke(instance, args);

            if (value == null) {
                return default;
            }

            return (T)value;
        }

        public object Invoke(object instance, params object[] args) {
            return methodBase.IsConstructor ? //
                (methodBase as ConstructorInfo).Invoke(args) : //
                methodBase.Invoke(instance, args);
        }

        #region Builder

        /// <inheritdoc/>
        private interface ICachedMethodComponent : IAttributeHolderComponent {
            string _Name { get; }

            Type _ReturnType { get; }

            List<ICachedParameter> _Parameters { get; }

            MethodBase _MethodBase { get; }
        }

        /// <inheritdoc/>
        public class CachedMethodComponent<T> : AttributeHolderComponent<T>, ICachedMethodComponent
                where T : CachedMethodComponent<T> {
            public string _Name { get; private set; }

            public Type _ReturnType { get; private set; }

            public List<ICachedParameter> _Parameters { get; }

            public MethodBase _MethodBase { get; private set; }

            public CachedMethodComponent() {
                _Parameters = new List<ICachedParameter>();
            }

            /// <summary>
            /// Sets the method's name.
            /// </summary>
            /// <param name="name">The name of the method.</param>
            /// <returns>The builder.</returns>
            public T Name(string name) {
                _Name = name;
                return this as T;
            }

            /// <summary>
            /// Sets the method's return type.
            /// </summary>
            /// <param name="type">The return type.</param>
            /// <returns>The builder.</returns>
            public T ReturnType(Type type) {
                _ReturnType = type;
                return this as T;
            }

            /// <summary>
            /// Adds a single parameter to the cached method's information.
            /// </summary>
            /// <param name="parameter">The parameter to add.</param>
            /// <returns>The builder.</returns>
            public T Parameter(ICachedParameter parameter) {
                _Parameters.Add(parameter);
                return this as T;
            }

            /// <summary>
            /// Adds multiple parameters to the cached method's information.
            /// </summary>
            /// <param name="parameters">The parameters to add.</param>
            /// <returns>The builder.</returns>
            public T Parameters(params ICachedParameter[] parameters) {
                foreach (ICachedParameter parameter in parameters) {
                    _Parameters.Add(parameter);
                }

                return this as T;
            }

            /// <summary>
            /// Sets the raw <see cref="System.Reflection.MethodBase"/> that contains method information.
            /// </summary>
            /// <param name="methodBase">The method base.</param>
            /// <returns>The builder.</returns>
            public T MethodBase(MethodBase methodBase) {
                _MethodBase = methodBase;
                return this as T;
            }
        }

        /// <summary>
        /// Builder used to produce new cached methods.
        /// </summary>
        new public class Builder : CachedMethodComponent<Builder>, IBuilder<CachedMethod> {

            public CachedMethod Build() {
                return new CachedMethod(this);
            }
        }

        #endregion Builder
    }
}