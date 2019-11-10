using System;

namespace Juicy.Injection.Interfaces {

    internal interface IFactoryBuilder {

        IFactoryBuilder Implement(Type baseType, Type implementationType);

        IFactoryBuilder Build(Type factoryType);
    }
}