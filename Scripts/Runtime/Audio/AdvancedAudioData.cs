using System;
using HatchStudio.Utilities;
using UnityEngine;

namespace HatchStudio.Audio
{
    [Serializable]
    public struct AdvancedAudioData
    {
        [SerializeField, Range(0f, 1f)]
        [Tooltip("The volume of the audio clip in the range of 0 to 1.")]
        private float _volume;

        [SerializeField, Range(0f, 1f)]
        [Tooltip("The amount of volume jitter to apply to the audio clip.")]
        private float _volumeJitter;

        [SerializeField, Range(0f, 2f)]
        [Tooltip("The pitch of the audio clip in the range of 0 to 2.")]
        private float _pitch;

        [SerializeField, Range(0f, 1f)]
        [Tooltip("The amount of pitch jitter to apply to the audio clip.")]
        private float _pitchJitter;
        
        [SerializeField]
        [Tooltip("The array of audio clips to choose from.")]
        private AudioClip[] _clips;


        /// <summary>
        /// The default values for an instance of AdvancedAudioData.
        /// </summary>
        public static readonly AdvancedAudioData Default = new(Array.Empty<AudioClip>(), 1f, 0.1f, 1f, 0.05f);

        public AdvancedAudioData(AudioClip[] clips, float volume, float volumeJitter, float pitch, float pitchJitter)
        {
            _clips = clips;
            _volume = volume;
            _volumeJitter = volumeJitter;
            _pitch = pitch;
            _pitchJitter = pitchJitter;
        }

        /// <summary>
        /// The audio clip to play.
        /// </summary>
        public readonly AudioClip Clip => _clips.SelectRandom();

        /// <summary>
        /// The volume of the audio clip with jitter applied.
        /// </summary>
        public readonly float Volume => _volume.Jitter(_volumeJitter);

        /// <summary>
        /// The pitch of the audio clip with jitter applied.
        /// </summary>
        public readonly float Pitch => _pitch.Jitter(_pitchJitter);

        /// <summary>
        /// Returns true if the audio is worth playing (can be heard).
        /// </summary>
        public readonly bool IsPlayable => _volume > 0.01f && _clips.Length > 0;
    }
}