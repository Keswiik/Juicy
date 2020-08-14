using Juicy.Reflection.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

 namespace Juicy.Reflection {

    internal class CachedTypeFactory {

        internal CachedType GetCachedType(Type type) {
            if (!type.IsClass) {
                throw new InvalidOperationException(($"{type} is a invalid, cannot be a value-type or interface."));
            }

            CachedType.Builder builder = new CachedType.Builder() //
                .Constructors(GetCachedMethods(type, true).ToArray()) //
                .Methods(GetCachedMethods(type).ToArray()) //
                .AddAttributes(GetAttributes(type).ToArray());

            return builder.Build();
        }

        private List<CachedMethod> GetCachedMethods(Type type, bool constructors = false) {
            MethodBase[] methods = constructors ? //
                (MethodBase[])type.GetConstructors() : //
                type.GetMethods();

            List<CachedMethod> cachedMethods = new List<CachedMethod>();
            foreach (MethodBase method in methods) {
                CachedMethod.Builder builder = new CachedMethod.Builder() //
                    .MethodBase(method) //
                    .Name(method.Name) //
                    .Parameters(GetCachedParameters(method).ToArray()) //
                    .AddAttributes(GetAttributes(method).ToArray());

                if (method is MethodInfo) {
                    builder.ReturnType((method as MethodInfo).ReturnType);
                } else if (method is ConstructorInfo) {
                    builder.ReturnType(type);
                }

                cachedMethods.Add(builder.Build());
            }

            return cachedMethods;
        }

        private List<CachedParameter> GetCachedParameters(MethodBase methodBase) {
            ParameterInfo[] parameters = methodBase.GetParameters();

            List<CachedParameter> cachedParameters = new List<CachedParameter>();
            foreach (ParameterInfo parameter in parameters) {
                CachedParameter.Builder builder = new CachedParameter.Builder() //
                    .AddAttributes(GetAttributes(parameter).ToArray()) //
                    .Position(parameter.Position) //
                    .Type(parameter.ParameterType);

                cachedParameters.Add(builder.Build());
            }

            return cachedParameters;
        }

        private List<Attribute> GetAttributes(ICustomAttributeProvider attributeProvider) {
            object[] attributes = attributeProvider.GetCustomAttributes(true);

            List<Attribute> attributeList = new List<Attribute>();
            foreach (object attribute in attributes) {
                attributeList.Add(attribute as Attribute);
            }

            return attributeList;
        }
    }
}