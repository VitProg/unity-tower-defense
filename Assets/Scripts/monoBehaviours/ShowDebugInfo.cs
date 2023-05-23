using System.Collections;
using System.Collections.Generic;
using td.monoBehaviours;
using UnityEngine;

namespace td
{
    public class ShowDebugInfo : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()  
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private bool show = true;

        public void OnCheckBoxChanged()
        {
            show = !show;
            foreach (var cellDebug in FindObjectsOfType<CellDebug>(true))
            {
                cellDebug.gameObject.SetActive(show);
            }
        }
    }
}
