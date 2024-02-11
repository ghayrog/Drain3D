using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DI
{
    public abstract class DependencyInstaller : MonoBehaviour,
        IServiceProvider, IInjectProvider
    {
        public virtual void Inject(ServiceLocator serviceLocator)
        {
            var fields = ReflectionTools.GetFields(this);

            foreach (var field in fields)
            {
                var target = field.GetValue(this);
                if (target != null)
                {
                    DependencyInjector.Inject(target, serviceLocator);
                }
            }
        }

        public virtual IEnumerable<(Type, object)> ProvideServices()
        {
            var fields = ReflectionTools.GetFields(this);

            foreach (var field in fields)
            {
                var customAttribute = field.GetCustomAttribute<ServiceAttribute>();
                if (customAttribute != null)
                {
                    Type type = customAttribute.Contract;
                    object service = field.GetValue(this);

                    yield return(type, service);

                }
            }
        }
    }
}
