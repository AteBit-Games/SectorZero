/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.Managers
{
    [DefaultExecutionOrder(1)]
    public class SettingsMenu : MonoBehaviour
    {
        private Label _settingDescription;
        private UIDocument _uiDocument;
        
        private Button _backButton;

        private readonly Resolution[] _resolutions = new Resolution[3];
        private Resolution _activeResolution;
        private readonly string[] _resolutionNames = { "1280x720", "1366x768", "1920x1080" };
        
        private readonly int[] _vsync = { 0, 1 };
        private int _activeVsync;
        private readonly string[] _vsyncNames = { "Off", "On" };
        
        private readonly FullScreenMode[] _displayModes = { FullScreenMode.Windowed, FullScreenMode.MaximizedWindow, FullScreenMode.FullScreenWindow };
        private FullScreenMode _activeDisplayMode;
        private readonly string[] _displayModeNames = { "Windowed", "Borderless", "Fullscreen" };
        
        private const string MasterVolumeKey = "master-volume";
        private float _activeMasterVolume;
        
        private const string MusicVolumeKey = "music-volume";
        private float _activeMusicVolume;
        
        private const string SfxVolumeKey = "sfx-volume";
        private float _activeSfxVolume;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            _settingDescription = rootVisualElement.Q<Label>("setting-description");
            
            _backButton = rootVisualElement.Q<Button>("back-button");
            _backButton.RegisterCallback<MouseEnterEvent>(_ =>
            {
                _settingDescription.text = "Exit the settings menu";
            });
            _backButton.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.HandleEscape();
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
            });
            
            _activeDisplayMode = Screen.fullScreenMode;
            RegisterDisplayModeSetting();

            SetupResolutions();
            _activeResolution = Screen.currentResolution;
            RegisterResolutionSetting();
            
            _activeVsync = QualitySettings.vSyncCount;
            RegisterVsyncSetting();
            
            RegisterPlayerPrefs();
            RegisterVolumeSettings();
        }
        
        private void RegisterDisplayModeSetting()
        {
            var rootVisualElement = _uiDocument.rootVisualElement;
            var displayModeSetting = rootVisualElement.Q<VisualElement>("display-mode");
            displayModeSetting.RegisterCallback<MouseEnterEvent>(_ =>
            {
                _settingDescription.text = "Change the active display mode";
            });
            
            var activeDisplayModeText = displayModeSetting.Q<Label>("setting-value");
            activeDisplayModeText.text = _displayModeNames[Array.IndexOf(_displayModes, _activeDisplayMode)];
            
            // register buttons for changing display mode
            var leftButton = displayModeSetting.Q<Button>("left-button");
            SetButtonState(leftButton, Array.IndexOf(_displayModes, _activeDisplayMode) != 0);

            var rightButton = displayModeSetting.Q<Button>("right-button");
            SetButtonState(rightButton, Array.IndexOf(_displayModes, _activeDisplayMode) != _displayModes.Length - 1);

            leftButton.RegisterCallback<ClickEvent>(_ =>
            {
                var newIndex = Array.IndexOf(_displayModes, _activeDisplayMode) - 1;
                Screen.fullScreenMode = _displayModes[newIndex];
                activeDisplayModeText.text = _displayModeNames[newIndex];
                _activeDisplayMode = _displayModes[newIndex];
                
                SetButtonState(leftButton, newIndex != 0);
                SetButtonState(rightButton, newIndex != _displayModes.Length - 1);
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
            });
            
            rightButton.RegisterCallback<ClickEvent>(_ =>
            {
                var newIndex = Array.IndexOf(_displayModes, _activeDisplayMode) + 1;
                Screen.fullScreenMode = _displayModes[newIndex];
                activeDisplayModeText.text = _displayModeNames[newIndex];
                _activeDisplayMode = _displayModes[newIndex];
                
                SetButtonState(leftButton, newIndex != 0);
                SetButtonState(rightButton, newIndex != _displayModes.Length - 1);
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
            });
        }

        private void RegisterResolutionSetting()
        {
            var rootVisualElement = _uiDocument.rootVisualElement;
            var resolutionSetting = rootVisualElement.Q<VisualElement>("screen-resolution");
            resolutionSetting.RegisterCallback<MouseEnterEvent>(_ =>
            {
                _settingDescription.text = "Change the active screen resolution";
            });

            var activeResolutionText = resolutionSetting.Q<Label>("setting-value");
            activeResolutionText.text = _resolutionNames[GetResolutionIndex()];
            
            // register buttons for changing display mode
            var leftButton = resolutionSetting.Q<Button>("left-button");
            SetButtonState(leftButton, GetResolutionIndex() != 0);
            
            var rightButton = resolutionSetting.Q<Button>("right-button");
            SetButtonState(rightButton, GetResolutionIndex() != _resolutions.Length - 1);
            
            leftButton.RegisterCallback<ClickEvent>(_ =>
            {
                var newIndex = GetResolutionIndex() - 1;
                Screen.SetResolution(_resolutions[newIndex].width, _resolutions[newIndex].height, Screen.fullScreenMode);
                activeResolutionText.text = _resolutionNames[newIndex];
                _activeResolution = _resolutions[newIndex];
                
                SetButtonState(leftButton, newIndex != 0);
                SetButtonState(rightButton, newIndex != _resolutions.Length - 1);
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
            });
            
            rightButton.RegisterCallback<ClickEvent>(_ =>
            {
                var newIndex = GetResolutionIndex() + 1;
                Screen.SetResolution(_resolutions[newIndex].width, _resolutions[newIndex].height, Screen.fullScreenMode);
                activeResolutionText.text = _resolutionNames[newIndex];
                _activeResolution = _resolutions[newIndex];
                
                SetButtonState(leftButton, newIndex != 0);
                SetButtonState(rightButton, newIndex != _resolutions.Length - 1);
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
            });
        }

        private void RegisterVsyncSetting()
        {
            var rootVisualElement = _uiDocument.rootVisualElement;
            var vsyncSetting = rootVisualElement.Q<VisualElement>("vertical-sync");
            vsyncSetting.RegisterCallback<MouseEnterEvent>(_ =>
            {
                _settingDescription.text = "Toggle vertical sync";
            });

            var activeVsyncText = vsyncSetting.Q<Label>("setting-value");
            activeVsyncText.text = _vsyncNames[Array.IndexOf(_vsync, _activeVsync)];
            
            // register buttons for changing display mode
            var leftButton = vsyncSetting.Q<Button>("left-button");
            SetButtonState(leftButton, Array.IndexOf(_vsync, _activeVsync) != 0);
            
            var rightButton = vsyncSetting.Q<Button>("right-button");
            SetButtonState(rightButton, Array.IndexOf(_vsync, _activeVsync) != _vsync.Length - 1);
            
            leftButton.RegisterCallback<ClickEvent>(_ =>
            {
                var newIndex = Array.IndexOf(_vsync, _activeVsync) - 1;
                QualitySettings.vSyncCount = _vsync[newIndex];
                activeVsyncText.text = _vsyncNames[newIndex];
                _activeVsync = _vsync[newIndex];
                
                SetButtonState(leftButton, newIndex != 0);
                SetButtonState(rightButton, newIndex != _vsync.Length - 1);
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
            });
            
            rightButton.RegisterCallback<ClickEvent>(_ =>
            {
                var newIndex = Array.IndexOf(_vsync, _activeVsync) + 1;
                QualitySettings.vSyncCount = _vsync[newIndex];
                activeVsyncText.text = _vsyncNames[newIndex];
                _activeVsync = _vsync[newIndex];
                
                SetButtonState(leftButton, newIndex != 0);
                SetButtonState(rightButton, newIndex != _vsync.Length - 1);
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
            });
        }

        private void RegisterVolumeSettings()
        {
            var rootVisualElement = _uiDocument.rootVisualElement;
            var masterSlider = rootVisualElement.Q<VisualElement>("master-slider");
            masterSlider.RegisterCallback<MouseEnterEvent>(_ =>
            {
                _settingDescription.text = "Change the global volume";
            });
            
            var soundSlider = rootVisualElement.Q<VisualElement>("sound-slider");
            soundSlider.RegisterCallback<MouseEnterEvent>(_ =>
            {
                _settingDescription.text = "Change the volume of sound effects";
            });
            
            var musicSlider = rootVisualElement.Q<VisualElement>("music-slider");
            musicSlider.RegisterCallback<MouseEnterEvent>(_ =>
            {
                _settingDescription.text = "Change the volume of music";
            });
            
            var masterVolume = masterSlider.Q<Slider>("slider");
            masterVolume.value = _activeMasterVolume;
            masterVolume.RegisterValueChangedCallback(evt =>
            {
                _activeMasterVolume = evt.newValue;
                SetPlayerPref(MasterVolumeKey, _activeMasterVolume);
                GameManager.Instance.SoundSystem.SetMasterVolume(_activeMasterVolume);
            });
            
            var soundVolume = soundSlider.Q<Slider>("slider");
            soundVolume.value = _activeSfxVolume;
            soundVolume.RegisterValueChangedCallback(evt =>
            {
                _activeSfxVolume = evt.newValue;
                SetPlayerPref(SfxVolumeKey, _activeSfxVolume);
                GameManager.Instance.SoundSystem.SetSfxVolume(_activeSfxVolume);
            });
            
            var musicVolume = musicSlider.Q<Slider>("slider");
            musicVolume.value = _activeMusicVolume;
            musicVolume.RegisterValueChangedCallback(evt =>
            {
                _activeMusicVolume = evt.newValue;
                SetPlayerPref(MusicVolumeKey, _activeMusicVolume);
                GameManager.Instance.SoundSystem.SetMusicVolume(_activeMusicVolume);
            });
        }
        
        private void RegisterPlayerPrefs()
        {
            _activeMasterVolume = LoadPlayerPref(MasterVolumeKey, 1f);
            _activeSfxVolume = LoadPlayerPref(SfxVolumeKey, 1f);
            _activeMusicVolume = LoadPlayerPref(MusicVolumeKey, 1f);
            
            GameManager.Instance.SoundSystem.SetMasterVolume(_activeMasterVolume);
            GameManager.Instance.SoundSystem.SetSfxVolume(_activeSfxVolume);
            GameManager.Instance.SoundSystem.SetMusicVolume(_activeMusicVolume);
        }
        
        private static void SetPlayerPref(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }
        
        private static float LoadPlayerPref(string key, float defaultValue)
        {
            if (PlayerPrefs.HasKey(key)) return PlayerPrefs.GetFloat(key);
            PlayerPrefs.SetFloat(key, defaultValue);
            return defaultValue;
        }

        private void SetupResolutions()
        {
            var resolutions = Screen.resolutions;
            foreach (var resolution in resolutions)
            {
                switch (resolution)
                {
                    case { width: 1920, height: 1080 }:
                        _resolutions[2] = resolution;
                        break;
                    case { width: 1366, height: 768 }:
                        _resolutions[1] = resolution;
                        break;
                    case { width: 1280, height: 720 }:
                        _resolutions[0] = resolution;
                        break;
                }
            }
        }
        
        private static void SetButtonState([NotNull] Button button, bool state)
        {
            if (button == null) throw new ArgumentNullException(nameof(button));
            button.SetEnabled(state);
            button.style.opacity = state ? 1 : 0.5f;
        }
        
        private int GetResolutionIndex()
        {
            for (var i = 0; i < _resolutions.Length; i++)
            {
                if (_resolutions[i].width == _activeResolution.width && _resolutions[i].height == _activeResolution.height)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}