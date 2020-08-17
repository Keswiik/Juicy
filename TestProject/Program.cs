using Juicy.Interfaces.Injection;
using Juicy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TestProject.Code;

namespace Reflector {

    internal class Program {

        private static void Main(string[] args) {
            IInjector injector = Juicer.CreateInjector(new TestModule());
            IInjector injector2 = injector.Get<IInjector>();
            Console.WriteLine($"Injector was able to inject itself: {injector == injector2}.");

            Console.WriteLine();
            IService service = injector.Get<IService>();
            ServiceImpl service2 = injector.Get<ServiceImpl>();
            service.DoThing();
            Console.WriteLine($"Same instance of service when using untargeted vs targeted binding: {service == service2}.");

            Console.WriteLine();
            Console.WriteLine($"Number from IService: {service.GetNumber()}");

            Console.WriteLine();
            HashSet<IMultiImplService> services = injector.Get<HashSet<IMultiImplService>>();
            Console.WriteLine($"Number of services created for IMultiImplService: {services?.Count}");

            Console.WriteLine();
            IOtherServiceFactory serviceFactory = injector.Get<IOtherServiceFactory>();
            IOtherService otherService = serviceFactory.CreateService(5);
            Console.WriteLine($"Factory service numbers: {otherService.GetNumber1()} | {otherService.GetNumber2()}");

            Console.WriteLine();
            NoInterfaceService noInterfaceService = injector.Get<NoInterfaceService>();
            noInterfaceService.PrintString();

            Console.WriteLine();
            Console.WriteLine($"Doubled Num1: {injector.Get<int>("DoubledNum1")}");
#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}