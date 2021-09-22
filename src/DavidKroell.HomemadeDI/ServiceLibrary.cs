using System;
using System.Collections.Generic;
using System.Linq;

namespace DavidKroell.HomemadeDI
{
    public class ServiceLibrary
    {
        private readonly Dictionary<Type, Type> _mappings = new();

        private void Map(Type interfaceType, Type implementationType)
        {
            var ctorCount = implementationType.GetConstructors()
                .Length;

            if (ctorCount > 1)
            {
                throw new ServiceMappingException("Implementation types may only contain a single constructor." +
                                                  $"The type {implementationType.FullName} has {ctorCount} constructors");
            }

            _mappings[interfaceType] = implementationType;
        }

        private Type GetImplementationType(Type interfaceType)
        {
            if (_mappings.TryGetValue(interfaceType, out var implType))
            {
                return implType;
            }

            throw new ServiceMappingNotFoundException(interfaceType);
        }

        private object CreateInstance(Type implType, params object[] dependentInstances)
        {
            try
            {
                var instance = Activator.CreateInstance(implType, dependentInstances);


                if (instance == null)
                {
                    throw new ServiceCreationException(implType);
                }

                return instance;
            }
            catch (ServiceCreationException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ServiceCreationException(implType, e);
            }
        }


        private object GetServiceWithDependency(Type type)
        {
            var implType = GetImplementationType(type);
            var dependentTypes = GetDependentTypes(implType);

            if (dependentTypes.Length == 0)
            {
                return CreateInstance(implType);
            }

            var dependentInstances = new object[dependentTypes.Length];

            for (var i = 0; i < dependentTypes.Length; i++)
            {
                dependentInstances[i] = GetServiceWithDependency(dependentTypes[i]);
            }

            return CreateInstance(implType, dependentInstances);
        }

        private Type[] GetDependentTypes(Type implType)
        {
            var ctorInfo = implType.GetConstructors()
                .First();

            var parameterInfos = ctorInfo.GetParameters();

            if (parameterInfos.Length == 0)
            {
                return Array.Empty<Type>();
            }

            return parameterInfos.Select(x => x.ParameterType)
                .ToArray();
        }

        public void Map<TInterface, TImpl>()
            where TImpl : TInterface
        {
            var interfaceType = typeof(TInterface);
            var implementationType = typeof(TImpl);

            Map(interfaceType, implementationType);
        }

        public T GetService<T>()
        {
            var interfaceType = typeof(T);

            return (T) GetServiceWithDependency(interfaceType);
        }
    }
}
