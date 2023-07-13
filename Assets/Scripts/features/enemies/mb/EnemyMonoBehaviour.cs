using UnityEngine;
using UnityEngine.UI;

namespace td.features.enemies.mb
{
    public class EnemyMonoBehaviour : MonoBehaviour
    {
        [SerializeField] public GameObject body;
        [SerializeField] public Slider hp;
        [SerializeField] public Image hpLine;
    }
}