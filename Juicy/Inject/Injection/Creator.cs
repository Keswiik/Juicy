using Juicy.Inject.Exceptions;
using Juicy.Inject.Storage;
using Juicy.Interfaces.Injection;
using Juicy.Interfaces.Storage;
using Juicy.Reflection;
using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juicy.Inject.Injection {

    /// <inheritdoc cref="ICreator"/>
    internal class Creator : ICreator {
        private Injector Injector { get; }

        private Reflector Reflector { get; }

        private ICache<List<ICachedParameter>, Type, string> ParameterMapping { get; }

        private ICache<List<ICachedParameter>, Type, string> InjectableParametersMapping { get; }

        /// <summary>
        /// Create an instance with the specified parent injector and reflector.
        /// </summary>
        /// <param name="injector">The parent injector for fetching parameter instances.</param>
        /// <param name="reflector">The reflector used to get method information.</param>
        internal Creator(Injector injector, Reflector reflector) {
            Injector = injector;
            Reflector = reflector;
            ParameterMapping = new InMemoryCache<List<ICachedParameter>, Type, string>();
            InjectableParametersMapping = new InMemoryCache<List<ICachedParameter>, Type, string>();
        }

        public object CreateInstance(Type type) {
            ICachedMethod constructor = GetConstructor(type);

            if (constructor.Parameters.Count == 0) {
                return Reflector.Invoke(constructor, null);
            } else if (constructor.Parameters.Count > 0) {
                var parameters = new object[constructor.Parameters.Count];
                int i = 0;

                // TODO: maybe sort the parameters when I cache them to avoid needing to do this nonsense
                // (technically speaking they are already sorted, but I don't do it explicitly)
                // match the parameters with their position in the map
                try {
                    for (; i < parameters.Length; i++) {
                        var parameter = constructor.Parameters.Find(p => p.Position == i);

                        if (!string.IsNullOrWhiteSpace(parameter.Name)) {
                            parameters[i] = Injector.Get(parameter.Type, parameter.Name);
                        } else {
                            parameters[i] = Injector.Get(parameter.Type, null);
                        }
                    }

                    return Reflector.Invoke(constructor, null, parameters);
                } catch (Exception e) {
                    throw new InjectionException(FormatFailedInjectionMessage(type, constructor.Parameters[i].Type, constructor.Parameters[i].Position + 1, constructor), e);
                }
            }

            throw new InjectionException($"Something horrible went wrong, could not create an instance of the type {type}.");
        }

        public object CreateInstanceWithParameters(Type type, params IParameterData[] args) {
            ICachedMethod constructor = GetConstructor(type);

            if (constructor.Parameters.Count == 0 && args.Length != 0) {
                throw new InjectionException($"Arguments were provided to create type {type.FullName}, but its constructor takes no arguments");
            } else if (constructor.Parameters.Count < args.Length) {
                throw new InjectionException($"Arguments were provided to create type {type.FullName}, but its constructor takes less arguments");
            }

            var parameters = new object[constructor.Parameters.Count];
            var injectableParameters = InjectableParametersMapping.IsCached(type) ?
                InjectableParametersMapping.Get(type) : //
                new List<ICachedParameter>(constructor.Parameters);
            if (ParameterMapping.IsCached(type)) {
                // assigned mapped external parameters to their position in the parameters array
                var mapping = ParameterMapping.Get(type);
                for (int i = 0; i < mapping.Count; i++) {
                    parameters[mapping[i].Position] = args[i].Value;
                }
            } else {
                // go through all known method parameters
                var mapping = new List<ICachedParameter>();
                for (int i = 0; i < args.Length; i++) {
                    var argType = args[i].Value.GetType();
                    bool foundParameter = false;
                    foreach (var parameter in constructor.Parameters) {
                        // map parameters if they having matching type and name
                        if (parameter.Type.IsAssignableFrom(argType) && args[i].Name == parameter.Name) {
                            foundParameter = true;
                            mapping.Add(parameter);
                            // also assign the values to avoid doing this elsewhere
                            parameters[mapping[i].Position] = args[i].Value;
                            injectableParameters.Remove(parameter);
                        }
                    }

                    if (!foundParameter) {
                        throw new InjectionException(FormatFailedParameterMatchingMessage(type, args[i], constructor));
                    }
                }

                // cache parameter mapping for the next invocation of this type
                ParameterMapping.Cache(mapping, type);
                InjectableParametersMapping.Cache(injectableParameters, type);
            }

            // map injected parameters
            foreach (var injectableParameter in injectableParameters) {
                try {
                    parameters[injectableParameter.Position] = Injector.Get(injectableParameter.Type, injectableParameter.Name);
                } catch (Exception e) {
                    throw new InjectionException(FormatFailedInjectionMessage(type, injectableParameter.Type, injectableParameter.Position + 1, constructor), e);
                }
            }

            return Reflector.Invoke(constructor, null, parameters);
        }

        private ICachedMethod GetConstructor(Type type) {
            return Reflector.GetInjectableConstructor(type);
        }

        private string FormatFailedInjectionMessage(Type beingCreated, Type failedToCreate, int parameterNumber, ICachedMethod constructor) {
            return $"Failed to inject {FormatParameterNumber(parameterNumber)} parameter ({failedToCreate.Name}) in constructor {FormatConstructor(constructor)} while creating {beingCreated.FullName}.";
        }

        private string FormatFailedParameterMatchingMessage(Type beingCreated, IParameterData missingParameter, ICachedMethod constructor) {
            return string.IsNullOrWhiteSpace(missingParameter.Name) ?
                $"Failed to find a matching parameter of type {missingParameter.Value.GetType().Name} named {missingParameter.Name} in constructor {FormatConstructor(constructor)} while creating {beingCreated.FullName}." :
                $"Failed to find a matching parameter of type {missingParameter.Value.GetType().Name} in constructor {FormatConstructor(constructor)} while creating {beingCreated.FullName}.";
        }

        private string FormatConstructor(ICachedMethod constructor) {
            StringBuilder sb = new StringBuilder();
            sb.Append(constructor.Name).Append('(');
            for (var i = 0; i < constructor.Parameters.Count; i++) {
                sb.Append(constructor.Parameters[i].Type.Name);
                if (i != constructor.Parameters.Count - 1) {
                    sb.Append(", ");
                }
            }
            sb.Append(")");
            return sb.ToString();
        }

        private string FormatParameterNumber(int parameterNumber) {
            // Realistically speaking, this should never go over 100. If it does, someone has a problem and it isn't me.
            if (parameterNumber > 10 && parameterNumber <= 20) {
                return "th";
            }

            string suffix = "th";
            switch (parameterNumber % 10) {
                case 1:
                    suffix = "st";
                    break;

                case 2:
                    suffix = "nd";
                    break;

                case 3:
                    suffix = "rd";
                    break;
            }

            return $"{parameterNumber}{suffix}";
        }
    }
}