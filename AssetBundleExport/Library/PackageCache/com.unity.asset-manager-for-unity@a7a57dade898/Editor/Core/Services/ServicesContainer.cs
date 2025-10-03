using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    interface IService
    {
        bool Enabled { get; set; }
        Type RegistrationType { get; }
    }

    abstract class BaseService : IService
    {
        public abstract Type RegistrationType { get; }

        public bool Enabled
        {
            get => m_Enabled;
            set
            {
                if (m_Enabled == value)
                    return;

                if (value)
                {
                    m_EnableCount++;
                    Utilities.DevAssert(m_EnableCount == 1, $"Multiple OnEnable calls detected on the same service. Make sure that you don't call ServiceContainer.Resolve in an OnEnable. ({RegistrationType})");
                    OnEnable();
                }
                else
                {
                    OnDisable();
                    m_EnableCount--;
                }

                m_Enabled = value;
            }
        }

        bool m_Enabled;
        int m_EnableCount;

        public virtual void OnEnable()
        {
            ValidateServiceDependencies();
        }

        /// <summary>
        /// Checks that service dependencies are resolved.
        /// This is generally as a result of services not being previously serialized.
        /// </summary>
        protected virtual void ValidateServiceDependencies() { }

        public virtual void OnDisable() { }
    }

    abstract class BaseService<T> : BaseService where T : IService
    {
        public override Type RegistrationType => typeof(T);
    }

    [AttributeUsage(AttributeTargets.Method)]
    class ServiceInjectionAttribute : Attribute { }

    [Serializable]
    class SerializedService
    {
        [SerializeReference]
        public List<IService> Dependencies;

        [SerializeReference]
        public IService Service;
    }

    [Serializable]
    [ExcludeFromCodeCoverage]
    sealed class ServicesContainer : ScriptableSingleton<ServicesContainer>, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<SerializedService> m_SerializedServices = new();

        readonly Dictionary<IService, HashSet<IService>> m_Dependencies = new();
        readonly Dictionary<Type, IService> m_RegisteredServices = new();
        readonly Dictionary<IService, HashSet<IService>> m_ReverseDependencies = new();

        public void OnDisable()
        {
            foreach (var service in m_RegisteredServices.Values)
            {
                service.Enabled = false;
            }
        }

        public void OnBeforeSerialize()
        {
            m_SerializedServices = new List<SerializedService>();

            var processedServices = new HashSet<IService>();

            foreach (var service in m_RegisteredServices.Values)
            {
                if (processedServices.Contains(service))
                {
                    continue;
                }

                m_SerializedServices.Add(new SerializedService
                {
                    Service = service,
                    Dependencies = m_Dependencies.TryGetValue(service, out var dependencies)
                        ? dependencies.ToList()
                        : new List<IService>()
                });

                processedServices.Add(service);
            }
        }

        public void OnAfterDeserialize()
        {
            if (m_SerializedServices == null || m_SerializedServices.Count == 0)
                return;

            foreach (var serviceInfo in m_SerializedServices)
            {
                // A previously serialized service may no longer exist
                if (serviceInfo.Service == null)
                    continue;
                
                Register_Internal(serviceInfo.Service);

                // Remove any dependencies that may have been serialized but are no longer valid
                var dependencies = serviceInfo.Dependencies.Where(d => d != null).ToHashSet();
                m_Dependencies.TryAdd(serviceInfo.Service, dependencies);
            }

            BuildReverseDependencies();
        }

        public void InitializeServices(params IService[] services)
        {
            Utilities.DevLog("Initializing Asset Manager Services");

            m_RegisteredServices.Clear();

            foreach (var service in services)
            {
                Register_Internal(service);
            }

            InjectServicesAndBuildDependencies();

            BuildReverseDependencies();
        }

        public void TryInitializeServices(params IService[] services)
        {
            var hasUnregisteredServices = false;

            foreach (var service in services)
            {
                if (m_RegisteredServices.GetValueOrDefault(service.RegistrationType) == null)
                {
                    Register_Internal(service);
                    hasUnregisteredServices = true;
                }
            }

            if (hasUnregisteredServices)
            {
                InjectServicesAndBuildDependencies();
                BuildReverseDependencies();
            }
        }

        public bool IsInitialized()
        {
            return m_RegisteredServices.Count > 0;
        }

        public void Register(IService service)
        {
            Register_Internal(service);
            InjectService(service);
        }

        internal void Register_Internal(IService service)
        {
            if (service != null)
            {
                var serviceType = service.RegistrationType ?? service.GetType();
                m_RegisteredServices[serviceType] = service;
            }
        }

        void RegisterReverseDependencies(IService service)
        {
            if (!m_Dependencies.TryGetValue(service, out var dependencies))
                return;

            foreach (var dependency in dependencies)
            {
                if (m_ReverseDependencies.TryGetValue(dependency, out var result))
                {
                    result.Add(service);
                }
                else
                {
                    m_ReverseDependencies[dependency] = new HashSet<IService> { service };
                }
            }
        }

        void BuildReverseDependencies()
        {
            m_ReverseDependencies.Clear();

            foreach (var service in m_RegisteredServices.Values)
            {
                RegisterReverseDependencies(service);
            }
        }

        /// <summary>
        /// Internal method to get a service without resolving its dependencies or enabling it.
        /// This should primarily be used in a service's OnEnable method to get other services in cases where they were not previously serialized.
        /// </summary>
        internal T Get<T>() where T : class, IService
        {
            if (m_RegisteredServices.TryGetValue(typeof(T), out var result))
            {
                return result as T;
            }

            return null;
        }

        public T Resolve<T>() where T : class, IService
        {
            var service = m_RegisteredServices.TryGetValue(typeof(T), out var result) ? result as T : null;
            if (service == null || service.Enabled)
            {
                return service;
            }

            var serviceEnablingQueue = new Queue<IService>();
            serviceEnablingQueue.Enqueue(service);
            while (serviceEnablingQueue.Count > 0)
            {
                EnableService(serviceEnablingQueue.Dequeue(), serviceEnablingQueue);
            }

            return service;
        }

        void EnableService(IService service, Queue<IService> serviceEnablingQueue)
        {
            if (service == null || service.Enabled)
                return;

            if (m_Dependencies.TryGetValue(service, out var dependencies))
            {
                foreach (var dependency in dependencies)
                {
                    EnableService(dependency, serviceEnablingQueue);
                }
            }

            service.Enabled = true;

            // All the reverse dependencies go into the queue to avoid nested enabling
            if (m_ReverseDependencies.TryGetValue(service, out var reverseDependencies))
            {
                foreach (var reverseDependency in reverseDependencies)
                {
                    serviceEnablingQueue.Enqueue(reverseDependency);
                }
            }
        }

        void InjectServicesAndBuildDependencies()
        {
            m_Dependencies.Clear();

            foreach (var service in m_RegisteredServices.Values)
            {
                InjectService(service);
            }
        }

        void InjectService(IService inst)
        {
            if (inst == null)
                return;

            var type = inst.GetType();
            var methods = type.GetMethods();

            foreach (var method in methods)
            {
                var injectAttribute =
                    (ServiceInjectionAttribute)Attribute.GetCustomAttribute(method, typeof(ServiceInjectionAttribute));
                if (injectAttribute == null)
                    continue;

                var parameters = method.GetParameters();
                var parameterValues = new object[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameterType = parameters[i].ParameterType;
                    if (m_RegisteredServices.ContainsKey(parameterType))
                    {
                        parameterValues[i] = m_RegisteredServices[parameterType];
                    }
                    else
                    {
                        throw new InvalidOperationException($"Service of type {parameterType} not registered.");
                    }
                }

                if (m_Dependencies.TryGetValue(inst, out var list))
                {
                    list.UnionWith(parameterValues.Cast<IService>());
                }
                else
                {
                    m_Dependencies[inst] = parameterValues.Cast<IService>().ToHashSet();
                }

                method.Invoke(inst, parameterValues);
            }
        }
    }
}
