using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using td.common;
using td.features.menu;
using td.services;
using td.utils;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace td.features.windows
{
    public class WindowsService
    {
        [Inject] private PrefabService prefabService;
        [InjectShared] private SharedData sharedData;
        
        private Dictionary<Type, GameObject> cache = new();

        private Stack<GameObject> stack = new();

        public GameObject Get(Type type) => GetWindow(type);
        
        public async Task<bool> Open(Type type, bool immediately = false)
        {
            var openingWindow = GetWindow(type);

            if (openingWindow == null) return false;
            
            var openingWindowPopup = openingWindow.GetComponent<UIWindowPopup>();
            var openingWindowScreen = openingWindow.GetComponent<UIWindowScreen>();
            var openingFadeInOut = openingWindow.GetComponent<FadeInOut>();

            var lastWindow = stack.Count > 0 ? stack.Last() : null;

            if (lastWindow)
            {
                //todo close lastWindow
            }
            
            stack.Push(openingWindow);

            var tasks = new List<Task>();

            var fadeMax = openingWindowScreen ? 1f : 0.5f;
            if (
                sharedData.fade.state != MenuState.Normal ||
                !FloatUtils.IsEquals(sharedData.fade.currentAlpha, fadeMax))
            {
                sharedData.fade.max = fadeMax;
                tasks.Add(sharedData.fade.FadeIn(immediately));
            }

            tasks.Add(openingFadeInOut.FadeIn(immediately));

            await Task.WhenAll(tasks);

            tasks.Clear();

            return true;
        }
        
        public async Task CloseAll(bool immediately = false)
        {
            var tasks = new List<Task>();
            while (stack.TryPop(out var window))
            {
                tasks.Add(Close(window, immediately));
            }

            await Task.WhenAll(tasks);
            
            tasks.Clear();
            stack.Clear();
        }

        private async Task<bool> Close(GameObject window, bool immediately = false)
        {
            if (window == null) return false;
            
            // var windowPopup = window.GetComponent<UIWindowPopup>();
            // var windowScreen = window.GetComponent<UIWindowScreen>();
            var fadeInOut = window.GetComponent<FadeInOut>();
            
            // var lastWindow = stack.Count > 0 ? stack.Last() : null;

            var tasks = new List<Task>();

            if (sharedData.fade.state != MenuState.Hidden)
            {
                tasks.Add(sharedData.fade.FadeOut(immediately));
            }

            tasks.Add(fadeInOut.FadeOut(immediately));

            await Task.WhenAll(tasks);
            
            tasks.Clear();

            return true;
        }

        private async Task Close(Type type, bool immediately = false) => await Close(GetWindow(type), immediately);
        
        public async Task CloseLast(bool immediately = false)
        {
            if (stack.TryPop(out var window))
            {
                await Close(window, immediately);
            }
        }

        public GameObject LastOpened => stack.Count > 0 ? stack.Last() : null;


        private GameObject GetPrefab(Type type)
        {
            return type switch
            {
                Type.MainMenu => prefabService.GetPrefab(PrefabCategory.UI, "MainMenu"),
                Type.PauseMenu => prefabService.GetPrefab(PrefabCategory.UI, "PauseMenu"),
                Type.VictoryPopup => prefabService.GetPrefab(PrefabCategory.UI, "VictoryPopup"),
                Type.FailPopup => prefabService.GetPrefab(PrefabCategory.UI, "FailPopup"),
                Type.ChoiseLevelMenu => prefabService.GetPrefab(PrefabCategory.UI, "ChoiseLevelMenu"),
                Type.SettingsMenu => prefabService.GetPrefab(PrefabCategory.UI, "SettingsMenu"),
                Type.ProfilePopup => prefabService.GetPrefab(PrefabCategory.UI, "ProfilePopup"),
                Type.GameExitConfirm => prefabService.GetPrefab(PrefabCategory.UI, "GameExitConfirm"),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
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
            
            var menuGO = Object.Instantiate(menuPrefab, sharedData.canvas.transform);
            menuGO.SetActive(false);
            
            cache.Add(type, menuGO);

            return menuGO;
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