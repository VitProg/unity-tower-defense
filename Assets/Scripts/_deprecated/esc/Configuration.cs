using UnityEngine;

namespace esc
{
    [CreateAssetMenu]
    sealed class Configuration : ScriptableObject {
        // Ширина и высота сетки.
        public int GridWidth;
        public int Gridheight;
    }
}