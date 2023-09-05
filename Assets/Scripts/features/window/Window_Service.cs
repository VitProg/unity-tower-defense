using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Leopotam.EcsProto.QoL;
using td.features.prefab;
using td.features.window.common;
using td.utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace td.features.window
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Window_Service
    {
        [DI] private Prefab_Service prefabService;
        // [DI] private Camera_Service cameraService;
        
        private readonly Dictionary<Type, GameObject> cache = new();
        private readonly List<UniTask> tasksList = new(1);

        private readonly Stack<Type> stack = new();
        private readonly GameObject container;

        public GameObject Get(Type type) => GetWindow(type);

        public Window_Service()
        {
            container = GameObject.FindGameObjectWithTag(Constants.Tags.WindowsContainer);
        }
        
        public async UniTask<bool> Open(Type type, bool immediately = false)
        {
            var openingWindow = GetWindow(type);

            if (openingWindow == null) return false;
            
            var openingWindowPopup = openingWindow.GetComponent<UIWindowPopup>();
            var openingWindowScreen = openingWindow.GetComponent<UIWindowScreen>();
            var openingFadeInOut = openingWindow.GetComponent<FadeInOut>();

            var lastWindow = LastOpened;

            if (lastWindow.HasValue)
            {
                //todo close lastWindow
            }
            
            openingWindow.transform.SetAsLastSibling();
            
            // var tasks = new List<Task>();

            // var fadeMax = openingWindowScreen ? 1f : 0.5f;
            // if (
            //     sharedData.fade.state != MenuState.Normal ||
            //     !FloatUtils.IsEquals(sharedData.fade.currentAlpha, fadeMax))
            // {
            //     sharedData.fade.max = fadeMax;
            //     tasks.Add(sharedData.fade.FadeIn(immediately));
            // }

            // tasks.Add(openingFadeInOut.FadeIn(immediately));
            await openingFadeInOut.FadeIn(immediately);

            stack.Push(type);
            DebugStack();

            return true;
        }
        
        public async UniTask CloseAll(bool immediately = false)
        {
            tasksList.Clear();
            while (stack.TryPop(out var window))
            {
                DebugStack();
                tasksList.Add(Close(window, immediately));
            }

            await UniTask.WhenAll(tasksList);
            
            tasksList.Clear();
            stack.Clear();
            DebugStack();
        }

        public async UniTask<bool> Close(Type type, bool immediately = false)
        {
            var window = GetWindow(type);
            
            if (window == null) return false;
            
            var fadeInOut = window.GetComponent<FadeInOut>();
            
            DebugStack();
            
            if (stack.Contains(type))
            {
                if (LastOpened == type)
                {
                    stack.Pop();
                    DebugStack();
                }
                else
                {
                    //todo нужно удалить из середины стека (((
                }
            }

            await fadeInOut.FadeOut(immediately);
            
            return true;
        }

        // public async Task Close(Type type, bool immediately = false) => await Close(, immediately);
        
        public async UniTaskVoid CloseLast(bool immediately = false)
        {
            if (stack.TryPop(out var window))
            {
                DebugStack();
                await Close(window, immediately);
            }
        }

        public async UniTask WaitClose(Type type)
        {
            if (!stack.Contains(type)) return;

            while (true)
            {
                if (!stack.Contains(type)) return;
                await UniTask.Yield();
            }
        }

        public Type? LastOpened => stack.Count > 0 ? stack.Peek() : null;


        private GameObject GetPrefab(Type type)
        {
            return type switch
            {
                Type.MainMenu => prefabService.GetPrefab(PrefabCategory.Windows, "MainMenu"),
                Type.PauseMenu => prefabService.GetPrefab(PrefabCategory.Windows, "PauseMenu"),
                Type.VictoryPopup => prefabService.GetPrefab(PrefabCategory.Windows, "VictoryPopup"),
                Type.FailPopup => prefabService.GetPrefab(PrefabCategory.Windows, "FailPopup"),
                Type.ChoiseLevelMenu => prefabService.GetPrefab(PrefabCategory.Windows, "ChoiseLevelMenu"),
                Type.SettingsMenu => prefabService.GetPrefab(PrefabCategory.Windows, "SettingsMenu"),
                Type.ProfilePopup => prefabService.GetPrefab(PrefabCategory.Windows, "ProfilePopup"),
                Type.GameExitConfirm => prefabService.GetPrefab(PrefabCategory.Windows, "GameExitConfirm"),
#if UNITY_EDITOR
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
#endif
            };
        }

        

        private GameObject GetWindow(Type type)
        {
            if (cache.TryGetValue(type, out var fromCache))
            {
                fromCache.gameObject.SetActive(false);
                return fromCache;
            }

            var menuPrefab = GetPrefab(type);

            if (menuPrefab == null) return null;

            var menuGO = Object.Instantiate(menuPrefab, container.transform);//cameraService.GetCanvas().transform);
            menuGO.SetActive(false);
            
            cache.Add(type, menuGO);

            return menuGO;
        }

        private void DebugStack()
        {
            var s = new StringBuilder("Stack: ");
            foreach (var type in stack)
            {
                s.Append(type.ToString());
            }

            Debug.Log(s.ToString());
            
        }
        

        public enum Type
        {
            MainMenu,
            PauseMenu,
            ChoiseLevelMenu,
            SettingsMenu,
            ProfilePopup,
            VictoryPopup,
            FailPopup,
            GameExitConfirm,
            //
        }
    }

    
}