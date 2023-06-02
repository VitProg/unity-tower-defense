using UnityEngine;
using td.utils.ecs;
using td.features.state;
using UnityEngine.UI;

namespace td.features.timeFlow
{
    public class UITimeFlowButton : MonoBehaviour
    {
        [SerializeField] private float timeFlowRate;
        
        public void ChangeTimeFlow()
        {
            DI.GetCustom<State>().TimeFlow = timeFlowRate; 

            float aColorSelected = 1;
            float aColorNotSelected = 0.6f;
            float aColorNewClick;
            
            foreach (var timeButtonGO in transform.parent.GetComponentsInChildren<Button>())
            {
                _= timeButtonGO.gameObject == gameObject ? aColorNewClick = aColorSelected 
                    : aColorNewClick = aColorNotSelected;

                if (timeButtonGO.transform.GetChild(0).TryGetComponent<Image>(out Image imageUnderTimeButton))
                {
                    imageUnderTimeButton.color = new Color(
                        imageUnderTimeButton.color.r,
                        imageUnderTimeButton.color.g,
                        imageUnderTimeButton.color.b,
                        aColorNewClick);
                }
            }
        }
    }
}