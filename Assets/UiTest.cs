using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace td
{
    public class UiTest : EventTrigger
    {
        public override void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("OnPointerDown");
            Debug.Log(eventData.position);
        }

        
        public void OnClick()
        {
            Debug.Log("CLICK");
        }
    }
}
