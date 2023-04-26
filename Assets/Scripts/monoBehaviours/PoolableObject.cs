using UnityEngine;
using UnityEngine.Serialization;

namespace td.monoBehaviours
{
    public class PoolableObject : MonoBehaviour
    {
        [FormerlySerializedAs("UniqID")] public string uniqID;
    }
}