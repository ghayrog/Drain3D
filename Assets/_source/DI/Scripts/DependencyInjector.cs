using System;
using System.Reflection;
using System.Collections.Generic;

namespace DI
{
    public static class DependencyInjector
    {
        private static List<object> AlreadyInjected = new List<object>();

        public static void Inject(object target, ServiceLocator serviceLocator)
        {
            if (AlreadyInjected.Contains(target)) return;

            var methods = ReflectionTools.GetMethods(target);

            foreach ( MethodInfo method in methods )
            {
                if (method.IsDefined(typeof(InjectAttribute)))
                {
                        InvokeConstruct(method, target, serviceLocator);
                }
            }
        }

        private static void InvokeConstruct(MethodInfo method, object target, ServiceLocator serviceLocator)
        {
            ParameterInfo[] parameters = method.GetParameters();
            int count = parameters.Length;
            object[] args = new object[count];

            for (int i = 0; i < count; i++)
            {
                ParameterInfo parameter = parameters[i];
                Type type = parameter.ParameterType;
                object service = serviceLocator.GetService(type);
                args[i] = service;
            }

            method.Invoke(target, args);
            AlreadyInjected.Add(target);
        }
    }
}
