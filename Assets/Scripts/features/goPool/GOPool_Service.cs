using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using td.monoBehaviours;
using td.utils;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace td.features.goPool
{
    public class GOPool_Service
    {
        private readonly Dictionary<string, ObjectPool<PoolableObject>> poolDictionary = new();

        public PoolableObject Get(
            GameObject prefab,
            int defaultCapacity = 10,
            int maxCapacity = 10,
            Func<GameObject, PoolableObject> initializeFunc = null,
            Action<PoolableObject> actionOnGet = null,
            Action<PoolableObject> actionOnRelease = null,
            Action<PoolableObject> actionOnDestroy = null
        ) =>
            GetInternal(prefab, null, defaultCapacity, maxCapacity, initializeFunc, actionOnGet, actionOnRelease, actionOnDestroy);

        public PoolableObject Get(
            GameObject prefab,
            Transform parent,
            int defaultCapacity = 10,
             int maxCapacity = 10,
            Func<GameObject, PoolableObject> initializeFunc = null,
            Action<PoolableObject> actionOnGet = null,
            Action<PoolableObject> actionOnRelease = null,
            Action<PoolableObject> actionOnDestroy = null
        ) =>
            GetInternal(prefab, parent, defaultCapacity, maxCapacity, initializeFunc, actionOnGet, actionOnRelease, actionOnDestroy);

        private PoolableObject GetInternal(
            GameObject prefab,
            Transform parent = null,
            int defaultCapacity = 10,
            int maxCapacity = 1000,
            Func<GameObject, PoolableObject> initializeFunc = null,
            Action<PoolableObject> actionOnGet = null,
            Action<PoolableObject> actionOnRelease = null,
            Action<PoolableObject> actionOnDestroy = null
        )
        {
            var pool = GetPool(
                prefab,
                parent,
                defaultCapacity,
                maxCapacity,
                initializeFunc,
                actionOnGet,
                actionOnRelease,
                actionOnDestroy
            );
            var gameObject = pool.Get();
            return gameObject;
        }


        public void Release(GameObject gameObject) =>
            Release(GetPoolKey(gameObject));

        public void Release(PoolableObject poolableObject)
        {
            var pool = GetPool(poolableObject);
            pool.Release(poolableObject);
            Log(poolableObject, pool);
        }

        public PoolableObject GetPoolKey(GameObject prefab)
        {
            var poolableObject = prefab.GetComponent<PoolableObject>();
            if (poolableObject == null)
            {
                poolableObject = prefab.AddComponent<PoolableObject>();
                poolableObject.uniqID = $"_{prefab.name}_{prefab.GetInstanceID()}_{RandomUtils.IntRange(0, 99999)}";
                // throw new NullReferenceException("Prefab dasn't contain component of type PoolableObject");
            }

            return poolableObject;
        }

        private string GetUniqID(PoolableObject poolableObject)
        {
#if UNITY_EDITOR
            if (poolableObject == null || string.IsNullOrEmpty(poolableObject.uniqID))
            {
                throw new Exception("uniqId is empty");
            }
#endif

            return poolableObject.uniqID;
        }

        public ObjectPool<PoolableObject> GetPool(PoolableObject poolableObject)
        {
            var id = GetUniqID(poolableObject);
            poolDictionary.TryGetValue(id, out var pool);
            return pool;
        }
        
        public ObjectPool<PoolableObject> GetPool(
            GameObject prefab,
            Transform parent = null,
            int defaultCapacity = 10,
            int maxCapacity = 1000,
            Func<GameObject, PoolableObject> initializeFunc = null,
            Action<PoolableObject> actionOnGet = null,
            Action<PoolableObject> actionOnRelease = null,
            Action<PoolableObject> actionOnDestroy = null
        )
        {
            var poolableObject = GetPoolKey(prefab);
            var id = GetUniqID(poolableObject);

            if (!poolDictionary.TryGetValue(id, out var pool))
            {
                pool = new ObjectPool<PoolableObject>(
                    () =>
                    {
                        var go = Object.Instantiate(prefab, parent);
                        initializeFunc?.Invoke(go);
                        return go.GetComponent<PoolableObject>();
                    },
                    actionOnGet ?? OnGet,
                    actionOnRelease ?? OnRelease,
                    actionOnDestroy ?? OnDestroy,
                    false,
                    defaultCapacity,
                    maxCapacity
                );

                poolDictionary.Add(id, pool);
            }

            return pool;
        }

        private void OnGet(PoolableObject o)
        {
            o.gameObject.SetActive(true);
            // todo o.OnGet()
        }

        private void OnRelease(PoolableObject o)
        {
            o.gameObject.SetActive(false);
            
            // todo o.OnRelease()
        }

        private void OnDestroy(PoolableObject o)
        {
            Object.Destroy(o.gameObject);
        }

        private void Log([CanBeNull] PoolableObject poolableObject, ObjectPool<PoolableObject> pool)
        {
#if GOPOOL_DEBUG
            Debug.Log($"POOL(${GetUniqID(poolableObject)}: active:{pool.CountActive}; inactive:{pool.CountInactive}; all:{pool.CountAll}");
#endif
        }
    }
}