using System;

namespace Juicy.Injection.Interfaces {

    internal interface IModule {

        void Configure();

        IBindingBuilder Bind(Type baseType);

        void Install(IModule module);

        void Install(IFactoryBuilder factoryBuilder);
    }
}