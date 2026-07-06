using TankAssault.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TankAssault.UI
{
    public class SettingsUI : UIScreen
    {
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider joystickSensitivitySlider;
        [SerializeField] private Toggle invertAimToggle;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button resetSaveButton;

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(() => Hide());
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            joystickSensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            invertAimToggle.onValueChanged.AddListener(OnInvertAimChanged);
            resetSaveButton.onClick.AddListener(OnResetSaveClicked);
        }

        public override void Show(bool instant = false)
        {
            base.Show(instant);
            var save = SaveSystem.Instance.Current;
            musicVolumeSlider.SetValueWithoutNotify(save.MusicVolume);
            sfxVolumeSlider.SetValueWithoutNotify(save.SfxVolume);
            joystickSensitivitySlider.SetValueWithoutNotify(save.JoystickSensitivity);
            invertAimToggle.SetIsOnWithoutNotify(save.InvertAim);
        }

        private void OnMusicVolumeChanged(float value)
        {
            SaveSystem.Instance.Current.MusicVolume = value;
            Audio.AudioManager.Instance?.SetMusicVolume(value);
        }

        private void OnSfxVolumeChanged(float value)
        {
            SaveSystem.Instance.Current.SfxVolume = value;
            Audio.AudioManager.Instance?.SetSfxVolume(value);
        }

        private void OnSensitivityChanged(float value)
        {
            Input.InputManager.Instance?.SetMoveSensitivity(value);
        }

        private void OnInvertAimChanged(bool value)
        {
            SaveSystem.Instance.Current.InvertAim = value;
            SaveSystem.Instance.Save();
        }

        private void OnResetSaveClicked()
        {
            SaveSystem.Instance.ResetSave();
        }
    }
}
