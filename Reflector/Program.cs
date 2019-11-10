using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Reflector {

    public class TestClass {

        private int y;

        [Test]
        public TestClass(int y)
        {
            this.y = y;
        }

        [Test]
        public void PrintString(string value) {
            Console.WriteLine(value);
        }

        public void PrintString(string first, string second) {
            Console.WriteLine(first);
            Console.WriteLine(second);
        }

        public int Multiply(int x)
        {
            return x * y;
        }
    }

    public class TestAttribute : Attribute
    {
        
    }

    public interface TestInterface
    {
        int Square(int x);
    }

    public class Proxy : DispatchProxy
    {

        public void Print(string value)
        {
            Console.WriteLine(value);
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {

            Type returnType = targetMethod.ReturnType;
            Console.WriteLine("I guess we hit this?");
            return returnType.IsValueType ? //
                Activator.CreateInstance(returnType) : //
                null;
        }
    }

    internal class Program {

        private static void Main(string[] args) {
            TestInterface proxy = MakeProxy<TestInterface>(typeof(TestInterface));
            proxy.Square(10);
        }

        private static T MakeProxy<T>(Type toProxy)
        {
            MethodInfo create = typeof(DispatchProxy).GetMethod("Create");
            object instance = create.MakeGenericMethod(toProxy, typeof(Proxy)) //
                .Invoke(null, new object[] { });
            (instance as Proxy).Print("found an alternative?");
            return (T)instance;
        }
    }
}