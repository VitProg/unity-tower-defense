using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace td.utils
{
/*
Radial Layout Group by Just a Pixel (Danny Goodayle) - http://www.justapixel.co.uk
Copyright (c) 2015
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
    public class RadialLayout : LayoutGroup
    {
        [OnValueChanged("CalculateRadial")]
        public float fDistance;
        
        [OnValueChanged("CalculateRadial")]
        [Range(0f, 360f)] public float MinAngle;
        
        [OnValueChanged("CalculateRadial")]
        [Range(0f, 360f)] public float MaxAngle;
        
        [OnValueChanged("CalculateRadial")]
        [Range(0f, 360f)] public float StartAngle;

        protected override void OnEnable()
        {
            base.OnEnable();
            CalculateRadial();
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }

        public override void CalculateLayoutInputVertical()
        {
            CalculateRadial();
        }

        public override void CalculateLayoutInputHorizontal()
        {
            CalculateRadial();
        }
// #if UNITY_EDITOR
//         protected override void OnValidate()
//         {
//             base.OnValidate();
//             CalculateRadial();
//         }
// #endif
        void CalculateRadial()
        {
            // Debug.Log("CalculateRadial");
            
            m_Tracker.Clear();
            if (transform.childCount == 0) return;

            var activeChildCount = 0;
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = (RectTransform)transform.GetChild(i);
                if (child == null || !child.gameObject.activeSelf) continue;
                activeChildCount++;
            }
            
            var fOffsetAngle = ((MaxAngle - MinAngle)) / activeChildCount;

            var fAngle = StartAngle;
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = (RectTransform)transform.GetChild(i);
                if (child == null || !child.gameObject.activeSelf) continue;
                
                //Adding the elements to the tracker stops the user from modifiying their positions via the editor.
                m_Tracker.Add(this, child,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.Pivot
                );
                
                var vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0);

                child.localPosition = vPos * fDistance;
                
                // Debug.Log("child.localPosition = " + child.localPosition);
                
                //Force objects to be center aligned, this can be changed however I'd suggest you keep all of the objects with the same anchor points.
                child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);

                // Debug.Log("fAngle = " + fAngle);
                fAngle += fOffsetAngle;
            }
        }
    }
}