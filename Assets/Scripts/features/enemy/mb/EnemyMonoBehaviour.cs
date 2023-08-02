using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.enemy.mb
{
    public class EnemyMonoBehaviour : MonoBehaviour
    {
        [SerializeField] public GameObject body;
        [SerializeField] public Slider hp;
        [SerializeField] public Image hpLine;
        [CanBeNull] public Animator animator;
        public float baseAnimationSpeed = 1f;
    }
}