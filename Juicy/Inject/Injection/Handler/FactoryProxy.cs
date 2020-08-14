using Juicy.Inject.Binding.Attributes;
using Juicy.Interfaces.Injection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Juicy.Inject.Injection.Handler {
    public class FactoryProxy : DispatchProxy {

        internal ICreator Creator { get; set; }

        internal Type ResultType { get; set; }

        private string[] parameterNames;

        protected override dynamic Invoke(MethodInfo targetMethod, object[] args) {
            if (parameterNames == null) {
                var parameters = targetMethod.GetParameters();
                parameterNames = new string[args.Length];
                for (int i = 0; i < parameterNames.Length; i++) {
                    parameterNames[i] = parameters[i].GetCustomAttribute<NamedAttribute>()?.Name;
                }
            }

            var parameterData = new ParameterData[args.Length];
            for (int i = 0; i < parameterData.Length; i++) {
                parameterData[i] = new ParameterData(parameterNames[i], args[i]);
            }

            return Creator.CreateInstanceWithParameters(ResultType, parameterData);
        }
    }
}
