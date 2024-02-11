using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public sealed class ObjectPool
    {
        [SerializeField]
        private GameObject _prefab;

        [SerializeField]
        private int _initialCount;

        [SerializeField]
        private Transform _container;

        [SerializeField]
        private Transform _world;

        private readonly Queue<GameObject> _poolObjects = new();
        private readonly HashSet<GameObject> _activeObjects = new();
        private bool _isInitialized = false;

        public void InitializePool()
        {
            for (var i = 0; i < _initialCount; i++)
            {
                var newPoolObject = GameObject.Instantiate(_prefab, _container);
                _poolObjects.Enqueue(newPoolObject);
            }
            _isInitialized = true;
        }

        public GameObject GetFromPool()
        {
            if (!_isInitialized) return null;
            if (_poolObjects.TryDequeue(out var poolObject))
            {
                poolObject.transform.SetParent(_world);
            }
            else
            {
                poolObject = GameObject.Instantiate(_prefab, _world);
            }

            _activeObjects.Add(poolObject);
            return poolObject;
        }

        public void ReturnToPool(GameObject poolObject)
        {
            if (!_isInitialized) return;
            if (_activeObjects.Remove(poolObject))
            {
                poolObject.transform.SetParent(_container);
                _poolObjects.Enqueue(poolObject);
            }
        }

        public void CleanPool()
        {
            while (_activeObjects.Count > 0)
            {
                ReturnToPool(_activeObjects.ElementAt(0));
            }
            _isInitialized = false;
            while (_poolObjects.TryDequeue(out var poolObject))
            {
                GameObject.Destroy(poolObject);
            }
        }
    }
}
