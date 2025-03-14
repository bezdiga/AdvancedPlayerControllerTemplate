using System.Collections;
using HatchStudio.Manager;
using HatchStudios.ToolBox;
using UnityEngine;
using UnityEngine.Audio;

namespace HatchStudio.Audio
{
    [DefaultExecutionOrder(ExecutionOrderConstants.SCRIPTABLE_SINGLETON)]
    [CreateAssetMenu(menuName = MANAGERS_MENU_PATH + "Audio Manager", fileName = nameof(AudioManager))]
    public class AudioManager : Manager<AudioManager>
    {
        [SerializeField,NotNull] private AudioMixer _mixer;
        
        [SerializeField]
        private AudioMixerSnapshot _defaultSnapshot;

        [SerializeField]
        [Tooltip("The master audio mixer group.")]
        private AudioMixerGroup _masterGroup;

        [SerializeField]
        [Tooltip("The audio mixer group for sound effects.")]
        private AudioMixerGroup _effectsGroup;

        [SerializeField]
        [Tooltip("The audio mixer group for ambience sounds.")]
        private AudioMixerGroup _ambienceGroup;

        [SerializeField]
        [Tooltip("The audio mixer group for music.")]
        private AudioMixerGroup _musicGroup;

        [SerializeField]
        [Tooltip("The audio mixer group for UI sounds.")]
        private AudioMixerGroup _uIGroup;
        
        [SerializeField, Range(1, 50)]
        [Tooltip("The number of 3D audio sources.")]
        private int _3dAudioSourceCount = 9;

        [SerializeField, Range(1, 50)]
        [Tooltip("The number of 2D audio sources.")]
        private int _2dAudioSourceCount = 4;

        [SerializeField, Range(1, 50)]
        [Tooltip("The number of looping audio sources.")]
        private int _loopAudioSourceCount = 4;

        [SerializeField, Range(1, 50)]
        [Tooltip("The number of UI audio sources.")]
        private int _uIAudioSourceCount = 6;

        [SerializeField, Range(0.01f, 1f)]
        [Tooltip("The minimum duration for looping audio easing.")]
        private float _minEaseDuration = 0.25f;

        [SerializeField, Range(0.01f, 1f)]
        [Tooltip("The maximum duration for looping audio easing.")]
        private float _maxEaseDuration = 0.5f;
        
        private int _2dAudioSourceIndex;
        private int _3dAudioSourceIndex;
        private int _loopAudioSourceIndex;
        private int _uiAudioSourceIndex;
        private AudioSource[] _2dAudioSources;
        private AudioSource[] _3dAudioSources;
        private AudioSource[] _loopAudioSources;
        private AudioSource[] _uiAudioSources;
        private RuntimeComponent _runtimeComponent;
        
        private const string MASTER_VOLUME_KEY = "MasterVolume";
        private const string EFFECTS_VOLUME_KEY = "EffectsVolume";
        private const string AMBIENCE_VOLUME_KEY = "AmbienceVolume";
        private const string MUSIC_VOLUME_KEY = "MusicVolume";
        private const string UI_VOLUME_KEY = "UIVolume";

        #region Initialize

        private sealed class RuntimeComponent : MonoBehaviour
        { }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#else
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        private static void Init() => LoadOrCreateInstance();
        
        protected override void OnInitialized()
        {
            _runtimeComponent = CreateRuntimeObject<RuntimeComponent>("AudioRuntimeComponent");
            CreateAudioSources(); 
            SetVolumeCallbacks();
            InitVolume();
        }


        private void CreateAudioSources()
        {
            _3dAudioSourceIndex = -1;
            _2dAudioSourceIndex = -1;
            _loopAudioSourceIndex = -1;
            _uiAudioSourceIndex = -1;
            
            _3dAudioSources = CreateAudioSourceArray(_effectsGroup, _3dAudioSourceCount, true, false, "3D Audio Source");
            _2dAudioSources = CreateAudioSourceArray(_effectsGroup, _2dAudioSourceCount, false, false, "2D Audio Source");
            _loopAudioSources = CreateAudioSourceArray(_effectsGroup, _loopAudioSourceCount, true, true, "Loop Audio Source");
            _uiAudioSources = CreateAudioSourceArray(_uIGroup, _uIAudioSourceCount, false, false, "UI Audio Source");
            
            return;
            
            AudioSource[] CreateAudioSourceArray(AudioMixerGroup mixerGroup, int count, bool spatialize, bool loop, string objectName)
            {
                var sources = new AudioSource[count];
                for (int i = 0; i < count; i++)
                    sources[i] = CreateAudioSource(mixerGroup, spatialize, loop, objectName);

                return sources;
            }
        }
        
        private AudioSource CreateAudioSource(AudioMixerGroup mixerGroup, bool spatialize, bool loop, string objectName)
        {
            AudioSource source;

            if (spatialize)
            {
                GameObject audioObject = new(objectName, typeof(AudioSource));
                audioObject.transform.SetParent(_runtimeComponent.transform);
                source = audioObject.GetComponent<AudioSource>();
            }
            else
                source = _runtimeComponent.gameObject.AddComponent<AudioSource>();

            source.volume = 1f;
            source.outputAudioMixerGroup = mixerGroup;
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialize = spatialize;
            source.spatialBlend = spatialize ? 1f : 0f;
            source.minDistance = 2f;

            return source;
        }
        
        private void SetVolumeCallbacks()
        {
            var settings = AudioOptions.Instance;
            settings.MasterVolume.Changed += volume => _mixer.SetFloat(MASTER_VOLUME_KEY, GetDBForVolume(volume));
            settings.EffectsVolume.Changed += volume => _mixer.SetFloat(EFFECTS_VOLUME_KEY, GetDBForVolume(volume));
            settings.AmbienceVolume.Changed += volume => _mixer.SetFloat(AMBIENCE_VOLUME_KEY, GetDBForVolume(volume));
            settings.MusicVolume.Changed += volume => _mixer.SetFloat(MUSIC_VOLUME_KEY, GetDBForVolume(volume));
            settings.UIVolume.Changed += volume => _mixer.SetFloat(UI_VOLUME_KEY, GetDBForVolume(volume));
        }
        
        private void InitVolume()
        {
            var settings = AudioOptions.Instance;
            _mixer.SetFloat(MASTER_VOLUME_KEY, GetDBForVolume(settings.MasterVolume));
            _mixer.SetFloat(EFFECTS_VOLUME_KEY, GetDBForVolume(settings.EffectsVolume));
            _mixer.SetFloat(AMBIENCE_VOLUME_KEY, GetDBForVolume(settings.AmbienceVolume));
            _mixer.SetFloat(MUSIC_VOLUME_KEY, GetDBForVolume(settings.MusicVolume));
            _mixer.SetFloat(UI_VOLUME_KEY, GetDBForVolume(settings.UIVolume));
        }
        
        private static float GetDBForVolume(float volume) => Mathf.Log(Mathf.Clamp(volume, 0.001f, 1f)) * 20; 
        #endregion
        
        public void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            var source = GetNext3DAudioSource();
            source.transform.position = position;

            source.Stop();
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();
        }
        
        private AudioSource GetNext3DAudioSource()
        {
            _3dAudioSourceIndex = (int)Mathf.Repeat(_3dAudioSourceIndex + 1, _3dAudioSourceCount);

            // Parented sources can become null if their parent gets destroyed. 
            var source = _3dAudioSources[_3dAudioSourceIndex];
            if (source == null)
            {
                source = CreateAudioSource(_effectsGroup, true, false, "Parented Audio Source");
                _3dAudioSources[_3dAudioSourceIndex] = source;
            }

            return source;
        }
        
        public void PlayClipAtTransformDelayed(AudioClip clip, Transform transform, float delay, float volume = 1f, float pitch = 1f)
        {
            var source = GetNext3DAudioSource();
            source.transform.SetParent(transform);

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.PlayDelayed(delay);
        }

        public void PlayClip2D(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            var source = GetNext2DAudioSource();

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();
        }
        
         public int StartLoopAtTransform(AudioClip clip, Transform transform, bool spatialize = true, float volume = 1f, float pitch = 1f, float duration = Mathf.Infinity)
        {
            AudioSource source = GetNextLoopAudioSource();

            if (source.isPlaying)
            {
                Debug.LogWarning("All audio sources used for looping are in use.");
                return -1;
            }

            source.transform.SetParent(transform);
            source.playOnAwake = true;
            source.clip = clip;
            source.pitch = pitch;
            source.spatialize = spatialize;
            source.spatialBlend = spatialize ? 1f : 0f;

            _runtimeComponent.StartCoroutine(C_PlayLoop(source, volume, duration, true));
            return _loopAudioSourceIndex;
        }

        public bool IsLoopPlaying(int loopId) => loopId >= 0 && _loopAudioSources[loopId].playOnAwake;

        public void StopLoop(ref int loopId)
        {
            if (loopId < 0 || loopId >= _loopAudioSourceCount)
            {
                Debug.LogError($"{loopId} is not a valid id.");
                return;
            }

            AudioSource source = _loopAudioSources[loopId];
            source.playOnAwake = false;

            loopId = -1;
        }

        public IEnumerator C_PlayLoop(AudioSource source, float volume, float duration, bool reparent = false)
        {
            source.Play();

            float easeDuration = Mathf.Clamp(duration * 0.25f, _minEaseDuration, _maxEaseDuration);

            //  Ease in volume...
            float from = 0f;
            float to = volume;
            float time = 0f;
            while (time < 1f)
            {
                source.volume = Mathf.Lerp(from, to, time);
                time += Time.deltaTime / easeDuration;
                yield return null;
            }

            // Loop audio...
            while (source.playOnAwake)
            {
                if (duration <= 0f)
                    source.playOnAwake = false;
                duration -= Time.deltaTime;

                yield return null;
            }

            // Ease out volume...
            from = source.volume;
            to = 0f;
            time = 0f;
            while (time < 1f)
            {
                source.volume = Mathf.Lerp(from, to, time);
                time += Time.deltaTime / easeDuration;
                yield return null;
            }

            source.Stop();

            if (reparent)
                source.transform.SetParent(_runtimeComponent.transform);
        }
        
        public void PlayUIClip(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            var source = GetNextUIAudioSource();
            source.pitch = pitch;
            source.PlayOneShot(clip, volume);
        }
        
        private AudioSource GetNextLoopAudioSource()
        {
            _loopAudioSourceIndex = (int)Mathf.Repeat(_loopAudioSourceIndex + 1, _loopAudioSourceCount);

            // Parented sources can become null if their parent gets destroyed. 
            var source = _loopAudioSources[_loopAudioSourceIndex];
            if (source == null)
            {
                source = CreateAudioSource(_effectsGroup, true, true, "Parented Audio Source");
                _loopAudioSources[_loopAudioSourceIndex] = source;
            }

            return source;
        }

        private AudioSource GetNext2DAudioSource()
        {
            _2dAudioSourceIndex = (int)Mathf.Repeat(_2dAudioSourceIndex + 1, _2dAudioSourceCount);
            return _2dAudioSources[_2dAudioSourceIndex];
        }

        private AudioSource GetNextUIAudioSource()
        {
            _uiAudioSourceIndex = (int)Mathf.Repeat(_uiAudioSourceIndex + 1, _uIAudioSourceCount);
            return _uiAudioSources[_uiAudioSourceIndex];
        }
    }
}
