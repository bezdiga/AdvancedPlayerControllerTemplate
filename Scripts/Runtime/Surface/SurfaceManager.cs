using System.Collections.Generic;
using HatchStudio.Audio;
using HatchStudio.Manager;
using HatchStudios.ToolBox;
using UnityEngine;

namespace HatchStudio.Surface
{
    [CreateAssetMenu(menuName = MANAGERS_MENU_PATH + "Surface Manager", fileName = nameof(SurfaceManager))]
    public class SurfaceManager : Manager<SurfaceManager>
    {
        [SerializeField,NotNull]
        [Tooltip("Default surface definition.")]
        private SurfaceDefinition _defaultSurface;
        
        private readonly Dictionary<PhysicsMaterial, SurfaceDefinition> _materialSurfacePairs = new(12);
        private readonly Dictionary<int, CachedSurfaceEffect> _surfaceEffects = new(32);
        #region Initialization
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#else
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        private static void Init() => LoadOrCreateInstance();

        /*static SurfaceManager()
        {
            ScenePools.ScenePoolHandlerDestroyed += () =>
            {
                if (Instance != null)
                    Instance._surfaceEffects.Clear();
            };
        }*/

        protected override void OnInitialized()
        {
#if UNITY_EDITOR
            _materialSurfacePairs.Clear();
            _surfaceEffects.Clear();
#endif
            CacheSurfaceDefinitions();
        }

        private void CacheSurfaceDefinitions()
        {
            var surfaces = SurfaceDefinition.Definitions;
            foreach (var surface in surfaces)
            {
                foreach (var material in surface.Materials)
                {
                    if (material == null)
                    {
                        Debug.LogError($"One of the physic materials ''{surface}'' is null.", surface);
                        return;
                    }

                    if (!_materialSurfacePairs.TryAdd(material, surface))
                    {
                        Debug.LogError($"The physic material ''{material.name}'' on {surface.name} is already referenced by a different surface definition ''{_materialSurfacePairs[material]}''", material);
                        return;
                    }
                }
            }
        }
        #endregion
        public SurfaceDefinition SpawnEffectFromHit(ref RaycastHit hit, SurfaceEffectType effectType, float audioVolume = 1f, bool parentDecal = false)
        {
            var surface = GetSurfaceFromHit(ref hit);
            if (TryGetEffect(surface, effectType, out var surfaceEffect))
            {
                if (parentDecal)
                    surfaceEffect.Play(hit.point, Quaternion.LookRotation(hit.normal), audioVolume, hit.transform);
                else
                    surfaceEffect.Play(hit.point, Quaternion.LookRotation(hit.normal), audioVolume);
            }

            return surface;
        }
        
        public SurfaceDefinition GetSurfaceFromHit(ref RaycastHit hit)
        {
            var material = hit.collider.sharedMaterial;
            if (material != null && _materialSurfacePairs.TryGetValue(material, out var surface))
                return surface;

            return _defaultSurface;
        }
        private bool TryGetEffect(SurfaceDefinition surface, SurfaceEffectType effectType, out CachedSurfaceEffect surfaceEffect)
        {
            int poolId = surface.Id + (int)effectType;
            if (_surfaceEffects.TryGetValue(poolId, out surfaceEffect))
                return surfaceEffect != null;

            var effectPair = surface.GetEffectPairOfType(effectType);
            surfaceEffect = CreateCachedEffect(effectPair, poolId);
            return surfaceEffect != null;
        }
        
        private CachedSurfaceEffect CreateCachedEffect(SurfaceDefinition.EffectPair effectPair, int poolId)
        {
            var surfaceEffect = new CachedSurfaceEffect(effectPair);
            _surfaceEffects.Add(poolId, surfaceEffect);
            return surfaceEffect;
        }

        #region Internal

        private sealed class CachedSurfaceEffect
        {
            private readonly SurfaceDefinition.EffectPair _effectPair;
            private float _audioPlayTimer;
            
            private const float AUDIO_PLAY_COOLDOWN = 0.3f;

            public CachedSurfaceEffect(SurfaceDefinition.EffectPair effectPair)
            {
                _effectPair = effectPair;
            }
            public void Play(Vector3 position, Quaternion rotation, float volume)
            {
                // Play the audio.
                if (Time.time > _audioPlayTimer && _effectPair.AudioData.IsPlayable)
                {
                    AudioManager.Instance.PlayClipAtPoint(_effectPair.AudioData.Clip, position, _effectPair.AudioData.Volume * volume, _effectPair.AudioData.Pitch);
                    _audioPlayTimer = Time.time + AUDIO_PLAY_COOLDOWN;
                }
            }
            
            public void Play(Vector3 position, Quaternion rotation, float volume, Transform parent)
            {
                // Play the audio.
                if (_effectPair.AudioData.IsPlayable)
                    AudioManager.Instance.PlayClipAtPoint(_effectPair.AudioData.Clip, position, _effectPair.AudioData.Volume * volume, _effectPair.AudioData.Pitch);
                
            }
        }
        #endregion
    }
}