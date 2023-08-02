using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Pool;

namespace td.features.goPool
{
    public interface IPoolable<T> where T : MonoBehaviour
    {
        public void SetPool(IObjectPool<T> pool);
        [CanBeNull] public IObjectPool<T> GetPool();
    }
}