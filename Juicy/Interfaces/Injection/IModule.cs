using Juicy.Inject.Binding;
using System.Collections;

namespace Juicy.Interfaces.Injection {
    public interface IModule {
        ConcreteBinding.ConcreteBindingBuilder Bind<T>();
        CollectionBinding.CollectionBindingBuilder BindMany<T>() where T : IEnumerable;
        FactoryBinding.FactoryBindingBuilder BindFactory<T>();
    }
}
