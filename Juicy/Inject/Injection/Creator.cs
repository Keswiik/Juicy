using Juicy.Inject.Binding.Attributes;
using Juicy.Inject.Storage;
using Juicy.Interfaces.Injection;
using Juicy.Interfaces.Storage;
using Juicy.Reflection;
using Juicy.Reflection.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
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
            ParameterMapping = new Cache<List<ICachedParameter>, Type, string>();
            InjectableParametersMapping = new Cache<List<ICachedParameter>, Type, string>();
        }

        object ICreator.CreateInstance(Type type) {
            ICachedMethod selectedConstructor = GetConstructor(type);

            if (selectedConstructor.Parameters.Count == 0) {
                return Reflector.Instantiate(type);
            } else if (selectedConstructor.Parameters.Count > 0) {
                var parameters = new object[selectedConstructor.Parameters.Count];

                // TODO: maybe sort the parameters when I cache them to avoid needing to do this nonsense
                // match the parameters with their position in the map
                for (int i = 0; i < parameters.Length; i++) {
                    var parameter = selectedConstructor.Parameters.Find(p => p.Position == i);
                    if (parameter == null) {
                        throw new InvalidOperationException($"Failed to locate the parameter at position {i} in type {type}.");
                    }

                    // TODO: simplify
                    if (parameter.HasAttribute(typeof(NamedAttribute))) {
                        var attribute = parameter.GetAttribute<NamedAttribute>();
                        if (!string.IsNullOrWhiteSpace(attribute?.Name)) {
                            parameters[i] = Injector.Get(parameter.Type, attribute.Name);
                        }
                    } else {
                        parameters[i] = Injector.Get(parameter.Type, null);
                    }
                }

                return Reflector.Instantiate(type, parameters);
            }

            throw new InvalidOperationException($"Something horrible went wrong, could not create an instance of the type {type}.");
        }

        object ICreator.CreateInstanceWithParameters(Type type, params IParameterData[] args) {
            ICachedMethod constructor = GetConstructor(type);

            if (constructor.Parameters.Count == 0 && args.Length != 0) {
                throw new InvalidOperationException($"Arguments were provided to create type {type.FullName}, but its constructor takes no arguments");
            } else if (constructor.Parameters.Count < args.Length) {
                throw new InvalidOperationException($"Arguments were provided to create type {type.FullName}, but its constructor takes less arguments");
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
                        throw new InvalidOperationException($"Failed to find a matching parameter of type {argType.FullName} while constructing {type.FullName}.");
                    }
                }

                // cache parameter mapping for the next invocation of this type
                ParameterMapping.Cache(mapping, type);
                InjectableParametersMapping.Cache(injectableParameters, type);
            }

            // map injected parameters
            foreach (var injectableParameter in injectableParameters) {
                parameters[injectableParameter.Position] = Injector.Get(injectableParameter.Type, injectableParameter.Name);
            }

            return Reflector.Instantiate(type, parameters);
        }

        private ICachedMethod GetConstructor(Type type) {
            ICachedMethod constructor = Reflector.GetInjectableConstructor(type);

            if (constructor == null) {
                throw new InvalidOperationException($"No injectable constructors found for the type {type}");
            }

            return constructor;
        }
    }
}
