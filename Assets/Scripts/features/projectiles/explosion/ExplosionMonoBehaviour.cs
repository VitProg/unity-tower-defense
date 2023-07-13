using NaughtyAttributes;
using UnityEngine;

namespace td.features.projectiles.explosion
{
    public class ExplosionMonoBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject cicle;
        
        public float maxDiameter;
        
        [OnValueChanged("Refresh")]
        public float currentDiameter;

        public float SetDiameter(float newDiameter)
        {
            currentDiameter = newDiameter;
            
            Refresh();

            return currentDiameter;
        }

        private void Refresh()
        {
            cicle.transform.localScale = new Vector3(currentDiameter, currentDiameter, 1f);
        }
    }
}