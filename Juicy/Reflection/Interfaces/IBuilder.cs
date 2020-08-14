namespace Juicy.Reflection.Interfaces {

    internal interface IBuilder<T>
            where T : class {

        T Build();
    }
}