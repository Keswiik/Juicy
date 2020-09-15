using Juicy.Inject.Binding;
using System.Collections;

namespace Juicy.Interfaces.Injection {

    /// <summary>
    /// Specifies binding functionality that must be exposed through all modules.
    /// </summary>
    /// <remarks>Honestly, I could probably move these into AbstractModule.</remarks>
    public interface IModule {

        /// <summary>
        /// Bind <typeparamref name="T"/> to a single implementation or instance.
        /// </summary>
        /// <typeparam name="T">The type to bind.</typeparam>
        /// <returns>A binding builder for concrete bindings.</returns>
        ConcreteBinding.ConcreteBindingBuilder Bind<T>();

        /// <summary>
        /// Bind <typeparamref name="T"/> to several implementations.
        /// </summary>
        /// <remarks>The injected type <b>must</b> match <typeparamref name="T"/>, otherwise binding will fail.</remarks>
        /// <typeparam name="T">The type being bound.</typeparam>
        /// <returns>A binding builder for collection bindings.</returns>
        CollectionBinding.CollectionBindingBuilder BindMany<T>() where T : IEnumerable;

        /// <summary>
        /// Binds a factory that uses injection to create instances of another type.
        /// </summary>
        /// <remarks>A factory should only have <b>one</b></remarks>
        /// <typeparam name="T">The interface that the factory will be bound to.</typeparam>
        /// <returns>A binding builder for factory bindings.</returns>
        FactoryBinding.FactoryBindingBuilder BindFactory<T>();

        /// <summary>
        /// Installs bindings from an external module.
        /// </summary>
        /// <remarks>
        /// This can be used to install modules containing dependencies of the current module. This is to create self-contained modules.
        /// </remarks>
        /// <param name="module">The module to install bindings from.</param>
        void Install(IModule module);
    }
}
