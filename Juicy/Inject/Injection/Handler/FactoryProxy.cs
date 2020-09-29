using Juicy.Inject.Binding.Attributes;
using Juicy.Interfaces.Injection;
using System;
using System.Reflection;

namespace Juicy.Inject.Injection.Handler {

    /// <summary>
    /// Proxy implementation and delegates requests to an ICreator instance to handle injection with external parameters.
    /// </summary>
    public class FactoryProxy : DispatchProxy {
        internal ICreator Creator { get; set; }

        internal Type ResultType { get; set; }

        // used to cache parameter names for later
        private string[] parameterNames;

        protected override dynamic Invoke(MethodInfo targetMethod, object[] args) {
            // cache parameter names if we did not have them before
            if (parameterNames == null) {
                var parameters = targetMethod.GetParameters();
                parameterNames = new string[args.Length];
                for (int i = 0; i < parameterNames.Length; i++) {
                    parameterNames[i] = parameters[i].GetCustomAttribute<NamedAttribute>()?.Name;
                }
            }

            // TODO: see if this is a valid use case for value tuples
            var parameterData = new ParameterData[args.Length];
            for (int i = 0; i < parameterData.Length; i++) {
                parameterData[i] = new ParameterData(parameterNames[i], args[i]);
            }

            return Creator.CreateInstanceWithParameters(ResultType, parameterData);
        }
    }
}