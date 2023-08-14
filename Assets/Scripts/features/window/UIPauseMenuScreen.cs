using Leopotam.EcsProto.QoL;
using td.features.window.common;
using td.utils.di;
using UnityEngine;
using UnityEngine.UI;

namespace td.features.window
{
    [RequireComponent(typeof(FadeInOut))]
    [RequireComponent(typeof(UIWindowPopup))]
    public class UIPauseMenuScreen : MonoBehaviour
    {
        private Window_Service WindowsService =>  ServiceContainer.Get<Window_Service>();

        [SerializeField] private Button closeButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            closeButton.onClick.AddListener(OnCloseClicked);
            resumeButton.onClick.AddListener(OnCloseClicked);
            restartButton.onClick.AddListener(OnRestartClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnQuitClicked()
        {
            Debug.Log("OnQuitClicked");
            //todo
        }

        private async void OnSettingsClicked()
        {
            Debug.Log("OnSettingsClicked");
            await WindowsService.Open(Window_Service.Type.SettingsMenu);
        }

        private void OnRestartClicked()
        {
            Debug.Log("OnRestartClicked");
            //todo
        }

        private async void OnCloseClicked()
        {
            Debug.Log("OnCloseClicked");
            await WindowsService.Close(Window_Service.Type.PauseMenu);
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.RemoveAllListeners();
            restartButton.onClick.RemoveAllListeners();
            settingsButton.onClick.RemoveAllListeners();
            quitButton.onClick.RemoveAllListeners();
        }
    }
}
