using UnityEngine;
using UnityEngine.Serialization;

namespace td.features.goPool
{
    public class PoolableObject : MonoBehaviour
    {
        [FormerlySerializedAs("UniqID")] public string uniqID;
    }
}