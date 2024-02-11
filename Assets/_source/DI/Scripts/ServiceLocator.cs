using System;
using System.Collections.Generic;
using UnityEngine;

namespace DI
{

    public sealed class ServiceLocator
    {
        private readonly Dictionary<Type, object> _services = new();

        public ServiceLocator()
        {

        }

        public object GetService(Type type)
        { 
            return _services[type];
        }

        public T GetService<T>() where T : class
        {
            return _services[typeof(T)] as T;
        }

        public void BindService(Type type, object service)
        {
            Debug.Log($"DI: Binding service of type {type}");
            _services.Add(type, service);
        }
    }
}
