﻿using Juicy.Constants;
using Juicy.Inject.Binding.Attributes;
using Juicy.Inject.Injection;
using System;
using System.Collections.Generic;
using System.Text;
using TestProject.Code.Attributes;
using TestProject.Code.MappedService;

namespace TestProject.Code {
    public class TestModule : AbstractModule {
        public override void Configure() {
            Bind<IService>() //
                .To<ServiceImpl>() //
                .In(BindingScope.Singleton);
            Bind<ServiceImpl>() //
                .In(BindingScope.Singleton);
            Bind<IExternallyProvidedService>() //
                .ToProvider<ExternalProvider>()
                .In(BindingScope.Singleton);
            BindMany<HashSet<IMultiImplService>>() //
                .To<MultiImplService1>() //
                .To<MultiImplService2>();
            BindFactory<IOtherServiceFactory>() //
                .Implement<IOtherService, OtherServiceImpl>();
            BindDictionary<Dictionary<MappedServiceTypes, IMappedService>>()
                .To(MappedServiceTypes.One, typeof(MappedService1))
                .To(MappedServiceTypes.Two, typeof(MappedService2))
                .In(BindingScope.Singleton);
            Bind<string>()
                .ToInstance("This is a string to print")
                .Attributed<PrintStringAttribute>();

            Install(new NestedTestModule());
        }

        [Provides]
        [Scope(BindingScope.Singleton)]
        [Named("PrintString")]
        public string ProvidePrintString() {
            return "This is a string to print";
        }

        [Provides]
        [Scope(BindingScope.Singleton)]
        [Named("DoubledNum1")]
        public int ProvideDoubledNum1([Named("Num1")] int num1) {
            return num1 * 2;
        }
    }
}
