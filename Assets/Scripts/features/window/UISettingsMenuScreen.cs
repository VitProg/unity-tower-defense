using Leopotam.EcsProto.QoL;
using td.features.window.common;
using td.utils.di;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.window
{
    [RequireComponent(typeof(FadeInOut))]
    [RequireComponent(typeof(UIWindowPopup))]
    public class UISettingsMenuScreen : MonoBehaviour
    {
        private Window_Service WindowsService => ServiceContainer.Get<Window_Service>();

        [SerializeField] private Button closeButton;

        private void Start()
        {
            closeButton.onClick.AddListener(OnCloseClicked);
        }

        private async void OnCloseClicked()
        {
            await WindowsService.Close(Window_Service.Type.SettingsMenu);
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveAllListeners();
        }
    }
}
