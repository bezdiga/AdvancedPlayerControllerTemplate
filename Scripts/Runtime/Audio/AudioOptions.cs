using HatchStudio.Manager;
using UnityEngine;

namespace HatchStudio.Audio
{
    [CreateAssetMenu(menuName = OPTIONS_MENU_PATH + "Audio Options", fileName = nameof(AudioOptions))]
    public sealed class AudioOptions : UserOptions<AudioOptions>
    {
        [SerializeField]
        private Option<float> _masterVolume = new(1f);

        [SerializeField]
        private Option<float> _effectsVolume = new(1f);

        [SerializeField]
        private Option<float> _ambienceVolume = new(1f);
        
        [SerializeField]
        private Option<float> _musicVolume = new(1f);

        [SerializeField]
        private Option<float> _uiVolume = new(1f);

        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() => CreateInstance();
        
        public Option<float> MasterVolume => _masterVolume;
        public Option<float> EffectsVolume => _effectsVolume;
        public Option<float> AmbienceVolume => _ambienceVolume;
        public Option<float> MusicVolume => _musicVolume;
        public Option<float> UIVolume => _uiVolume;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _masterVolume.Value = Mathf.Clamp01(_masterVolume.Value);
            _effectsVolume.Value = Mathf.Clamp01(_effectsVolume.Value);
            _ambienceVolume.Value = Mathf.Clamp01(_ambienceVolume.Value);
            _musicVolume.Value = Mathf.Clamp01(_musicVolume.Value);
            _uiVolume.Value = Mathf.Clamp01(_uiVolume.Value);
        }
#endif
    }
}