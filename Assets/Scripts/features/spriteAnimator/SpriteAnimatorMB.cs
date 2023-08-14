using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Leopotam.EcsProto.QoL;
using NaughtyAttributes;
using td.features.state;
using td.utils.di;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace td.features.spriteAnimator
{
    [ExecuteAlways]
    public class SpriteAnimatorMB : MonoBehaviour
    {
        // [DI] private State state;
        [CanBeNull] private State State => ServiceContainer.Get<State>();

        [Required] public SpriteRenderer spriteRenderer;
        [FormerlySerializedAs("Frames")] public List<SpriteAnimatorFrame> frames;
        public ushort frameIndex;
        [ShowNativeProperty] public ushort FramesCount => frames != null ? (ushort)frames.Count : (ushort)0;
        public float speed = 1f;
        public bool reverse = false;
        private float frameIndexFloat;
        private float timeFromPrevFrame = 0f;
        public bool autoPlay = false;
        public bool loop = true;

        private bool isPlayed = false;
        [ShowNativeProperty] public bool IsPlayed => isPlayed;

        public UnityEvent OnFinish { get; set; } = new();

        private void Start()
        {
            if (autoPlay) Play();
        }

        private void OnDestroy()
        {
            OnFinish.RemoveAllListeners();
        }

        [Button]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Play()
        {
            if (FramesCount == 0) return; 
            isPlayed = true;
        }
        
        [Button]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Stop()
        {
            if (FramesCount == 0) return;
            isPlayed = false;
            SetFrameIndex(0);
        }        
        
        [Button]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Pause()
        {
            if (FramesCount == 0) return;
            isPlayed = false;
        }        
        
        [Button]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FirstFrame()
        {
            if (FramesCount == 0) return;
            SetFrameIndex(0);
        }

        [Button]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LastFrame()
        {
            if (FramesCount == 0) return;
            SetFrameIndex((ushort)(FramesCount - 1));
        }
        
        [Button]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void NextFrame()
        {
            if (FramesCount == 0) return;
            if (frameIndex + 1 >= FramesCount) FirstFrame(); 
            else SetFrameIndex((ushort)(frameIndex + 1));
        }

        [Button]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PrevFrame()
        {
            if (FramesCount == 0) return;
            if (frameIndex == 0) LastFrame(); 
            else SetFrameIndex((ushort)(frameIndex - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetPrevFrameIndex()
        {
            if (frameIndex == 0) return (ushort)(FramesCount - 1);
            return (ushort)(frameIndex - 1);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetNextFrameIndex()
        {
            if (frameIndex == (ushort)(FramesCount - 1)) return 0;
            return (ushort)(frameIndex + 1);
        }
        
        private void Update()
        {
            if (!isPlayed || FramesCount == 0) return;
            
            timeFromPrevFrame += speed * Time.deltaTime * (State?.GetGameSpeed() ?? 1f);

            var fIndex = frameIndex;

            var currentFrame = frames[fIndex];
            var nextFrame = frames[reverse ? GetPrevFrameIndex() : GetNextFrameIndex()];

            var alpha = Mathf.Lerp(currentFrame.alpha, nextFrame.alpha, timeFromPrevFrame / currentFrame.duration);

            var color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;

            if (!(timeFromPrevFrame >= currentFrame.duration)) return;

            timeFromPrevFrame = 0f;
            
            if (reverse)
            {
                if (fIndex == 0)
                {
                    if (!loop)
                    {
                        OnFinish.Invoke();
                        Stop();
                        return;
                    }
                    fIndex = (ushort)(frames.Count - 1);
                }
                else fIndex--;
            }
            else
            {
                if (!loop && fIndex == FramesCount - 1)
                {
                    OnFinish.Invoke();
                    Stop();
                    return;
                }

                fIndex++;
                if (fIndex >= frames.Count) fIndex = 0;
            }
            
            SetFrameIndex(fIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFrameIndex(ushort fIndex)
        {
            fIndex = (ushort)Math.Min(fIndex, frames.Count - 1);
            if (fIndex == frameIndex) return;
            frameIndex = fIndex;
            spriteRenderer.sprite = frames[frameIndex].sprite;
        }
    }

    [Serializable]
    public struct SpriteAnimatorFrame
    {
        public Sprite sprite;
        public float duration;
        public float alpha;
    }
}