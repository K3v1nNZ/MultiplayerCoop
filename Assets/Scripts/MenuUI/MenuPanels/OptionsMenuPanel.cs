using System;
using System.IO;
using Game.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MenuUI.MenuPanels
{
    public class OptionsMenuPanel : MenuPanel
    {
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject displayPanel;
        [SerializeField] private GameObject audioPanel;
        [SerializeField] private GameObject controlsPanel;
        [SerializeField] private GameObject aboutPanel;
        [SerializeField] private Button applyButton;
        [Space(10)]
        [SerializeField] private Slider FieldOfViewSlider;
        [SerializeField] private TMP_Text FieldOfViewText;
        [SerializeField] private TMP_Dropdown windowModeDropdown;
        [SerializeField] private Toggle vsyncToggle;
        [SerializeField] private TMP_Dropdown msaaDropdown;
        [SerializeField] private Slider renderScaleSlider;
        [SerializeField] private TMP_Text renderScaleText;
        [SerializeField] private Slider shadowCascadeCountSlider;
        [SerializeField] private TMP_Text shadowCascadeCountText;
        [SerializeField] private Slider shadowDistanceSlider;
        [SerializeField] private TMP_Text shadowDistanceText;
        private GameSettings _gameSettings;
        private bool _settingsChanged;
    
        public override void PanelStart()
        {
            LoadSettings();
        }

        private void Update()
        { 
            applyButton.interactable = _settingsChanged;
        }

        public void GameplayButton()
        {
            gameplayPanel.SetActive(true);
            displayPanel.SetActive(false);
            audioPanel.SetActive(false);
            controlsPanel.SetActive(false);
            aboutPanel.SetActive(false);
        }
        
        public void DisplayButton()
        {
            gameplayPanel.SetActive(false);
            displayPanel.SetActive(true);
            audioPanel.SetActive(false);
            controlsPanel.SetActive(false);
            aboutPanel.SetActive(false);
        }
        
        public void AudioButton()
        {
            gameplayPanel.SetActive(false);
            displayPanel.SetActive(false);
            audioPanel.SetActive(true);
            controlsPanel.SetActive(false);
            aboutPanel.SetActive(false);
        }
        
        public void ControlsButton()
        {
            gameplayPanel.SetActive(false);
            displayPanel.SetActive(false);
            audioPanel.SetActive(false);
            controlsPanel.SetActive(true);
            aboutPanel.SetActive(false);
        }

        public void AboutButton()
        {
            gameplayPanel.SetActive(false);
            displayPanel.SetActive(false);
            audioPanel.SetActive(false);
            controlsPanel.SetActive(false);
            aboutPanel.SetActive(true);
        }

        public void ApplyButton()
        {
            File.WriteAllText(Application.persistentDataPath + "/settings.json", JsonUtility.ToJson(_gameSettings));
            LoadSettings();
            _settingsChanged = false;
            MainMenuManager.Instance.ShowModal("Success", "Settings applied.", "Continue", null, null, null);
        }
        
        private void LoadSettings()
        {
            _gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/settings.json"));
            
            Screen.fullScreenMode = _gameSettings.WindowMode switch
            {
                0 => FullScreenMode.Windowed,
                1 => FullScreenMode.FullScreenWindow,
                2 => FullScreenMode.ExclusiveFullScreen,
                _ => Screen.fullScreenMode
            };
            QualitySettings.vSyncCount = _gameSettings.VSync ? 1 : 0;
            GameLoadSettings.UrpAsset.msaaSampleCount = _gameSettings.MsaaSampleCount switch
            {
                0 => 1,
                1 => 2,
                2 => 4,
                3 => 8,
                _ => GameLoadSettings.UrpAsset.msaaSampleCount
            };
            GameLoadSettings.UrpAsset.renderScale = _gameSettings.RenderScale;
            GameLoadSettings.UrpAsset.shadowCascadeCount = _gameSettings.ShadowCascades;
            GameLoadSettings.UrpAsset.shadowDistance = _gameSettings.ShadowDistance;

            FieldOfViewSlider.value = _gameSettings.FieldOfView;
            FieldOfViewText.text = _gameSettings.FieldOfView.ToString("F0");
            windowModeDropdown.value = _gameSettings.WindowMode;
            vsyncToggle.isOn = _gameSettings.VSync;
            msaaDropdown.value = _gameSettings.MsaaSampleCount;
            renderScaleSlider.value = _gameSettings.RenderScale;
            renderScaleText.text = _gameSettings.RenderScale.ToString("F2");
            shadowCascadeCountSlider.value = _gameSettings.ShadowCascades;
            shadowCascadeCountText.text = _gameSettings.ShadowCascades.ToString();
            shadowDistanceSlider.value = _gameSettings.ShadowDistance;
            shadowDistanceText.text = _gameSettings.ShadowDistance.ToString("F0");
            _settingsChanged = false;
        }

        public void BackButton()
        {
            if (_settingsChanged)
            {
                MainMenuManager.Instance.ShowModal("Warning", "You have unsaved changes. Please save and then go back.", "Continue", null, null, null);
            }
            else
            {
                MainMenuManager.Instance.optionsMenuPanel.HideCanvasGroup();
                MainMenuManager.Instance.mainMenuPanel.ShowCanvasGroup();
            }
        }
        
        public void FieldOfViewSlide()
        {
            _gameSettings.FieldOfView = FieldOfViewSlider.value;
            FieldOfViewText.text = FieldOfViewSlider.value.ToString("F0");
            _settingsChanged = true;
        }

        public void WindowModeDropdown()
        {
            _gameSettings.WindowMode = windowModeDropdown.value;
            _settingsChanged = true;
        }
        
        public void VSyncToggle()
        {
            _gameSettings.VSync = vsyncToggle.isOn;
            _settingsChanged = true;
        }
        
        public void MsaaDropdown()
        {
            _gameSettings.MsaaSampleCount = msaaDropdown.value;
            _settingsChanged = true;
        }
        
        public void RenderScaleSlider()
        {
            _gameSettings.RenderScale = renderScaleSlider.value;
            renderScaleText.text = renderScaleSlider.value.ToString("F2");
            _settingsChanged = true;
        }
        
        public void ShadowCascadeCountSlider()
        {
            _gameSettings.ShadowCascades = (int) shadowCascadeCountSlider.value;
            shadowCascadeCountText.text = shadowCascadeCountSlider.value.ToString();
            _settingsChanged = true;
        }

        public void ShadowDistanceSlider()
        {
            _gameSettings.ShadowDistance = shadowDistanceSlider.value;
            shadowDistanceText.text = shadowDistanceSlider.value.ToString("F0");
            _settingsChanged = true;
        }
    }
}
