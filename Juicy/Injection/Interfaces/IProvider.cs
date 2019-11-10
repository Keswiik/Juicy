namespace Juicy.Injection.Interfaces {

    public interface IProvider<T> {

        T get();
    }
}