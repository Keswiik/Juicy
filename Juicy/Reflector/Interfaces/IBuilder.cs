namespace Juicy.Reflector.Interfaces {

    internal interface IBuilder<T>
            where T : class {

        T Build();
    }
}