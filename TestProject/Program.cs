using Juicy.Interfaces.Injection;
using Juicy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TestProject.Code;
using TestProject.Code.MappedService;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Serilog.Extensions.Logging;
using Serilog;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Reflector {

    internal static class Program {

        private static void Main() {
            //var configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json")
            //    .Build();
            //var logger = new LoggerConfiguration()
            //    .ReadFrom.Configuration(configuration)
            //    .CreateLogger();

            //Juicer.AddLogging(new SerilogLoggerFactory(logger: logger));

            IInjector injector = Juicer.CreateInjector(new TestModule().Override(new ModuleToOverride()));
            IInjector injector2 = injector.Get<IInjector>();
            IInjector injector3 = injector.CreateChildInjector();
            Console.WriteLine($"Injector was able to inject itself: {injector == injector2}.");

            Console.WriteLine();
            IService service = injector.Get<IService>();
            ServiceImpl service2 = injector.Get<ServiceImpl>();
            IService service3 = injector3.Get<IService>();
            service.DoThing();
            Console.WriteLine($"Same instance of service when using untargeted vs targeted binding: {service == service2}.");
            Console.WriteLine($"Same instance of service retrieved from the child injector: {service == service3}");

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

            Console.WriteLine();
            var externallyProvidedService = injector.Get<IExternallyProvidedService>();
            Console.WriteLine($"String from externally provided service: {externallyProvidedService.GetPrintString()}");
            Console.WriteLine($"Same instance of externally provided service received: {externallyProvidedService == injector.Get<IExternallyProvidedService>()}.");

            Console.WriteLine();
            var mappedServices = injector.Get<Dictionary<MappedServiceTypes, IMappedService>>();
            foreach (var type in Enum.GetValues(typeof(MappedServiceTypes)).Cast<MappedServiceTypes>()) {
                var mappedService = mappedServices[type];
                mappedService.DoSomething();
            }
#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}