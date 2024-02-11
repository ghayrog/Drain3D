using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DI
{
    public sealed class DIContext
    {
        private List<DependencyInstaller> _modules;

        private ServiceLocator _serviceLocator;

        private bool _isInitialized = false;

        public DIContext()
        { 
            _modules = new List<DependencyInstaller>();
            _serviceLocator = new ServiceLocator();
        }

        public void AddModule(DependencyInstaller module)
        {
            if (_isInitialized)
            {
                Debug.Log("Can't add a module - context is already initialized!");
                return;
            }
            _modules.Add(module);
        }

        public void Initialize(Scene scene)
        {
            if (_isInitialized)
            {
                Debug.Log("Can't initialize - context is already initialized!");
                return;
            }

            InstallServices();
            InjectAllDependencies(scene);
            _isInitialized = true;
        }

        private void InstallServices()
        {
            foreach (var module in _modules)
            {
                if (module is IServiceProvider serviceProvider)
                {
                    var services = serviceProvider.ProvideServices();
                    foreach (var (type, service) in services)
                    {
                        _serviceLocator.BindService(type, service);
                    }
                }
            }
        }

        private void InjectAllDependencies(Scene scene)
        {
            foreach (var module in _modules)
            {
                if (module is IInjectProvider injectProvider)
                {
                    Debug.Log($"DI: Injecting into {injectProvider}");
                    injectProvider.Inject(_serviceLocator);
                }
            }

            if (scene == null) return;

            GameObject[] gameObjects = scene.GetRootGameObjects();

            foreach (var gameObj in gameObjects)
            {
                Inject(gameObj.transform);
            }
        }

        private void Inject(Transform targetTransform)
        {
            var targets = targetTransform.GetComponents<MonoBehaviour>();
            foreach (var target in targets)
            {
                DependencyInjector.Inject(target, _serviceLocator);
            }

            foreach (Transform child in targetTransform)
            {
                Inject(child);
            }
        }
    }
}
