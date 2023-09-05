using System;
using System.Runtime.CompilerServices;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace td.features.ui
{
    public class ShowFpsMonoBehaviour : MonoBehaviour
    {
        [Required][SerializeField] private TMP_Text tFps;

        private int lastFrameIndex = 0;
        private const int FrameDeltaTimeArrayLength = 60;
        private int showFpsCounter = 0;
        private float[] frameDeltaTimeArray;

        private void Awake()
        {
            frameDeltaTimeArray = new float[FrameDeltaTimeArrayLength];
        }

        private void Update()
        {
            frameDeltaTimeArray[lastFrameIndex] = Time.deltaTime;
            lastFrameIndex++;
            showFpsCounter = Math.Min(showFpsCounter + 1, FrameDeltaTimeArrayLength);
            if (lastFrameIndex >= FrameDeltaTimeArrayLength) lastFrameIndex = 0;
            tFps.text = showFpsCounter >= FrameDeltaTimeArrayLength 
                ? ((int)CalcFPS()).ToString()
                : "";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float CalcFPS()
        {
            var total = 0f;
            for (var idx = 0; idx < FrameDeltaTimeArrayLength; idx++)
            {
                total += frameDeltaTimeArray[idx];
            }
            return FrameDeltaTimeArrayLength / total;
        }
    }
}