/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using Runtime.Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.UI
{
    [DefaultExecutionOrder(6)]
    public class DebugMenu : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement _mainContainer;
        
        private VisualElement _aiManagerContainer;
        private Label _menaceStateValue;
        private Label _menaceGaugeValue;
        
        public void Show()
        {
            _mainContainer.style.display = DisplayStyle.Flex;
        }
        
        public void Hide()
        {
            _mainContainer.style.display = DisplayStyle.None;
        }

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            
            _mainContainer = rootVisualElement.Q<VisualElement>("debug-container");
            
            SetupAIManager(rootVisualElement);
            StartCoroutine(UpdateStates());
        }
        
        private void SetupAIManager(VisualElement root)
        {
            var aiManager = root.Q<VisualElement>("ai-manager");
            _menaceStateValue = aiManager.Q<Label>("menace-state-value");
            _menaceGaugeValue = aiManager.Q<Label>("menace-gauge-value");
        }

        private IEnumerator UpdateStates()
        {
            while (true)
            {
                UpdateAIManagerValues();
                yield return new WaitForSeconds(0.2f);
            }
        } 

        private void UpdateAIManagerValues()
        {
            _menaceStateValue.text = "Menace State: " + MenaceStateString();
            _menaceGaugeValue.text = "Menace Gauge: " + GameManager.Instance.AIManager.menaceState;
        }

        private static string MenaceStateString()
        {
            var state = GameManager.Instance.AIManager.menaceState;
            return state ? "Active" : "Inactive";
        }

        // private void Start()
        // {
        //     RegisterPlayerPrefs();
        //     RegisterVolumeSettings();
        //
        //     RegisterDisplayModeSetting();
        //     RegisterVsyncSetting();
        //     RegisterBrightnessSetting();
        //     RegisterResolutionSetting();
        //     
        //     RegisterNoteSetting();
        //     RegisterTapeSetting();
        // }
        //
        // private void RegisterDisplayModeSetting()
        // {
        //     var rootVisualElement = _uiDocument.rootVisualElement;
        //     var displayModeSetting = rootVisualElement.Q<VisualElement>("display-mode");
        //     displayModeSetting.RegisterCallback<MouseEnterEvent>(_ =>
        //     {
        //         _settingDescription.text = "Change the active display mode";
        //     });
        //     
        //     var activeDisplayModeText = displayModeSetting.Q<Label>("setting-value");
        //     activeDisplayModeText.text = _displayModeNames[Array.IndexOf(_displayModes, _activeDisplayMode)];
        //     
        //     // register buttons for changing display mode
        //     var leftButton = displayModeSetting.Q<Button>("left-button");
        //     SetButtonState(leftButton, Array.IndexOf(_displayModes, _activeDisplayMode) != 0);
        //
        //     var rightButton = displayModeSetting.Q<Button>("right-button");
        //     SetButtonState(rightButton, Array.IndexOf(_displayModes, _activeDisplayMode) != _displayModes.Length - 1);
        //
        //     leftButton.RegisterCallback<ClickEvent>(_ =>
        //     {
        //         var newIndex = Array.IndexOf(_displayModes, _activeDisplayMode) - 1;
        //         Screen.fullScreenMode = _displayModes[newIndex];
        //         activeDisplayModeText.text = _displayModeNames[newIndex];
        //         _activeDisplayMode = _displayModes[newIndex];
        //         
        //         SetButtonState(leftButton, newIndex != 0);
        //         SetButtonState(rightButton, newIndex != _displayModes.Length - 1);
        //         GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
        //         GameManager.Instance.SaveSystem.UpdatePlayerDisplayMode(newIndex);
        //     });
        //     
        //     rightButton.RegisterCallback<ClickEvent>(_ =>
        //     {
        //         var newIndex = Array.IndexOf(_displayModes, _activeDisplayMode) + 1;
        //         Screen.fullScreenMode = _displayModes[newIndex];
        //         activeDisplayModeText.text = _displayModeNames[newIndex];
        //         _activeDisplayMode = _displayModes[newIndex];
        //         
        //         SetButtonState(leftButton, newIndex != 0);
        //         SetButtonState(rightButton, newIndex != _displayModes.Length - 1);
        //         GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
        //         GameManager.Instance.SaveSystem.UpdatePlayerDisplayMode(newIndex);
        //     });
        // }
        //
        // private void RegisterResolutionSetting()
        // {
        //     Screen.SetResolution(1920, 1080, _activeDisplayMode);
        // }
        //
        // private void RegisterVsyncSetting()
        // {
        //     var rootVisualElement = _uiDocument.rootVisualElement;
        //     var vsyncSetting = rootVisualElement.Q<VisualElement>("vertical-sync");
        //     vsyncSetting.RegisterCallback<MouseEnterEvent>(_ =>
        //     {
        //         _settingDescription.text = "Toggle vertical sync";
        //     });
        //
        //     var activeVsyncText = vsyncSetting.Q<Label>("setting-value");
        //     activeVsyncText.text = _vsyncNames[Array.IndexOf(_vsync, _activeVsync)];
        //     
        //     // register buttons for changing display mode
        //     var leftButton = vsyncSetting.Q<Button>("left-button");
        //     SetButtonState(leftButton, Array.IndexOf(_vsync, _activeVsync) != 0);
        //     
        //     var rightButton = vsyncSetting.Q<Button>("right-button");
        //     SetButtonState(rightButton, Array.IndexOf(_vsync, _activeVsync) != _vsync.Length - 1);
        //     
        //     leftButton.RegisterCallback<ClickEvent>(_ =>
        //     {
        //         var newIndex = Array.IndexOf(_vsync, _activeVsync) - 1;
        //         QualitySettings.vSyncCount = _vsync[newIndex];
        //         activeVsyncText.text = _vsyncNames[newIndex];
        //         _activeVsync = _vsync[newIndex];
        //         
        //         SetButtonState(leftButton, newIndex != 0);
        //         SetButtonState(rightButton, newIndex != _vsync.Length - 1);
        //         GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
        //         GameManager.Instance.SaveSystem.UpdatePlayerVSync(newIndex > 0);
        //     });
        //     
        //     rightButton.RegisterCallback<ClickEvent>(_ =>
        //     {
        //         var newIndex = Array.IndexOf(_vsync, _activeVsync) + 1;
        //         QualitySettings.vSyncCount = _vsync[newIndex];
        //         activeVsyncText.text = _vsyncNames[newIndex];
        //         _activeVsync = _vsync[newIndex];
        //         
        //         SetButtonState(leftButton, newIndex != 0);
        //         SetButtonState(rightButton, newIndex != _vsync.Length - 1);
        //         GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
        //         GameManager.Instance.SaveSystem.UpdatePlayerVSync(newIndex > 0);
        //     });
        // }
        //
        // private void RegisterNoteSetting()
        // {
        //     var rootVisualElement = _uiDocument.rootVisualElement;
        //     var noteSetting = rootVisualElement.Q<VisualElement>("read-mode");
        //     noteSetting.RegisterCallback<MouseEnterEvent>(_ =>
        //     {
        //         _settingDescription.text = "Change the read mode for picked up notes";
        //     });
        //
        //     var activeNote = noteSetting.Q<Toggle>("Toggle");
        //     activeNote.value = _activeAutoNotes;
        //     
        //     // register buttons for changing display mode
        //     activeNote.RegisterValueChangedCallback(evt =>
        //     {
        //         _activeAutoNotes = evt.newValue;
        //         GameManager.Instance.SaveSystem.UpdatePlayerNotesValue(_activeAutoNotes);
        //         GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
        //     });
        // }
        //
        // private void RegisterTapeSetting()
        // {
        //     var rootVisualElement = _uiDocument.rootVisualElement;
        //     var tapeSetting = rootVisualElement.Q<VisualElement>("tape-mode");
        //     tapeSetting.RegisterCallback<MouseEnterEvent>(_ =>
        //     {
        //         _settingDescription.text = "Change the read mode for picked up tapes";
        //     });
        //
        //     var activeTape = tapeSetting.Q<Toggle>("Toggle");
        //     activeTape.value = _activeAutoTapes;
        //     
        //     // register buttons for changing display mode
        //     activeTape.RegisterValueChangedCallback(evt =>
        //     {
        //         _activeAutoTapes = evt.newValue;
        //         GameManager.Instance.SaveSystem.UpdatePlayerTapesValue(_activeAutoTapes);
        //         GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
        //     });
        // }
        //
        // private void RegisterVolumeSettings()
        // {
        //     var rootVisualElement = _uiDocument.rootVisualElement;
        //     var masterSlider = rootVisualElement.Q<VisualElement>("master-slider");
        //     masterSlider.RegisterCallback<MouseEnterEvent>(_ =>
        //     {
        //         _settingDescription.text = "Change the global volume";
        //     });
        //     
        //     var soundSlider = rootVisualElement.Q<VisualElement>("sound-slider");
        //     soundSlider.RegisterCallback<MouseEnterEvent>(_ =>
        //     {
        //         _settingDescription.text = "Change the volume of sound effects";
        //     });
        //     
        //     var musicSlider = rootVisualElement.Q<VisualElement>("music-slider");
        //     musicSlider.RegisterCallback<MouseEnterEvent>(_ =>
        //     {
        //         _settingDescription.text = "Change the volume of music";
        //     });
        //     
        //     var voicesSlider = rootVisualElement.Q<VisualElement>("voices-slider");
        //     voicesSlider.RegisterCallback<MouseEnterEvent>(_ =>
        //     {
        //         _settingDescription.text = "Change the volume of voices";
        //     });
        //     
        //     var masterVolume = masterSlider.Q<Slider>("slider");
        //     masterVolume.value = _activeMasterVolume;
        //     masterVolume.RegisterValueChangedCallback(evt =>
        //     {
        //         _activeMasterVolume = evt.newValue;
        //         GameManager.Instance.SaveSystem.UpdatePlayerMasterVolume(_activeMasterVolume);
        //         GameManager.Instance.SoundSystem.SetMasterVolume(_activeMasterVolume);
        //     });
        //     
        //     var soundVolume = soundSlider.Q<Slider>("slider");
        //     soundVolume.value = _activeSfxVolume;
        //     soundVolume.RegisterValueChangedCallback(evt =>
        //     {
        //         _activeSfxVolume = evt.newValue;
        //         GameManager.Instance.SaveSystem.UpdatePlayerSoundsVolume(_activeSfxVolume);
        //         GameManager.Instance.SoundSystem.SetSfxVolume(_activeSfxVolume);
        //     });
        //     
        //     var musicVolume = musicSlider.Q<Slider>("slider");
        //     musicVolume.value = _activeMusicVolume;
        //     musicVolume.RegisterValueChangedCallback(evt =>
        //     {
        //         _activeMusicVolume = evt.newValue;
        //         GameManager.Instance.SaveSystem.UpdatePlayerMusicVolume(_activeMusicVolume);
        //         GameManager.Instance.SoundSystem.SetMusicVolume(_activeMusicVolume);
        //     });
        //     
        //     var voicesVolume = voicesSlider.Q<Slider>("slider");
        //     voicesVolume.value = _activeVoicesVolume;
        //     voicesVolume.RegisterValueChangedCallback(evt =>
        //     {
        //         _activeVoicesVolume = evt.newValue;
        //         GameManager.Instance.SaveSystem.UpdatePlayerVoicesVolume(_activeVoicesVolume);
        //         GameManager.Instance.SoundSystem.SetVoicesVolume(_activeVoicesVolume);
        //     });
        // }
        //
        // private void RegisterBrightnessSetting()
        // {
        //     //slider
        //     var rootVisualElement = _uiDocument.rootVisualElement;
        //     var brightnessSlider = rootVisualElement.Q<VisualElement>("brightness-slider");
        //     brightnessSlider.RegisterCallback<MouseEnterEvent>(_ =>
        //     {
        //         _settingDescription.text = "Change the brightness of the game";
        //     });
        //     
        //     var brightness = brightnessSlider.Q<Slider>("slider");
        //     brightness.value = _activeBrightnessGain;
        //     var gammaGain = _activeVolume.sharedProfile.components[3] as LiftGammaGain;
        //     if (gammaGain != null)
        //     {
        //         gammaGain.gamma.value= new Vector4(1, 1, 1, _activeBrightnessGain);
        //         gammaGain.lift.value = new Vector4(1, 1, 1, _activeBrightnessGain);
        //     }
        //     
        //     brightness.RegisterValueChangedCallback(evt =>
        //     {
        //         _activeBrightnessGain = evt.newValue;
        //         GameManager.Instance.SaveSystem.UpdatePlayerBrightness(_activeBrightnessGain);
        //         if (gammaGain != null)
        //         {
        //             gammaGain.gamma.value = new Vector4(1, 1, 1, _activeBrightnessGain);
        //             gammaGain.lift.value = new Vector4(1, 1, 1, _activeBrightnessGain);
        //         }
        //     });
        // }
        //
        // private void RegisterPlayerPrefs()
        // {
        //     var playerData = GameManager.Instance.SaveSystem.GetPlayerData();
        //     
        //     _activeMasterVolume = playerData.masterVolume;
        //     _activeMusicVolume = playerData.musicVolume;
        //     _activeSfxVolume = playerData.sfxVolume;
        //     _activeVoicesVolume = playerData.voicesVolume;
        //     
        //     GameManager.Instance.SoundSystem.LoadMasterVolume(_activeMasterVolume);
        //     GameManager.Instance.SoundSystem.SetSfxVolume(_activeSfxVolume);
        //     GameManager.Instance.SoundSystem.SetMusicVolume(_activeMusicVolume);
        //     GameManager.Instance.SoundSystem.SetVoicesVolume(_activeVoicesVolume);
        //     
        //     _activeVsync = _vsync[playerData.vSync ? 1 : 0];
        //     _activeDisplayMode = _displayModes[playerData.displayMode];
        //     _activeBrightnessGain = playerData.brightnessGain;
        //     
        //     _activeAutoNotes = playerData.autoNotes;
        //     _activeAutoTapes = playerData.autoTapes;
        // }
        //
        // private static void SetButtonState(Button button, bool state)
        // {
        //     if (button == null) throw new ArgumentNullException(nameof(button));
        //     button.SetEnabled(state);
        //     button.style.opacity = state ? 1 : 0.5f;
        // }
    }
}