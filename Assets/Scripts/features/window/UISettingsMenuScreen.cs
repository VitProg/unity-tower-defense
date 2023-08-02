using Leopotam.EcsLite;
using td.features.window.common;
using td.utils.di;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.window
{
    [RequireComponent(typeof(FadeInOut))]
    [RequireComponent(typeof(UIWindowPopup))]
    public class UISettingsMenuScreen : MonoInjectable
    {
        private readonly EcsInject<Windows_Service> windowsService;

        [SerializeField] private Button closeButton;

        private new void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(OnCloseClicked);
        }

        private async void OnCloseClicked()
        {
            Debug.Log("OnCloseClicked");

            await windowsService.Value.Close(Windows_Service.Type.SettingsMenu);
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveAllListeners();
        }
    }
}
