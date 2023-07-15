using td.features.windows.common;
using td.utils.ecs;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.windows
{
    [RequireComponent(typeof(FadeInOut))]
    [RequireComponent(typeof(UIWindowPopup))]
    public class UISettingsMenuScreen : MonoBehaviour
    {
        private WindowsService windowsService => DI.Get<WindowsService>();

        [SerializeField] private Button closeButton;

        void Awake()
        {
            closeButton.onClick.AddListener(OnCloseClicked);
        }

        private async void OnCloseClicked()
        {
            Debug.Log("OnCloseClicked");

            await windowsService.Close(WindowsService.Type.SettingsMenu);
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveAllListeners();
        }
    }
}
