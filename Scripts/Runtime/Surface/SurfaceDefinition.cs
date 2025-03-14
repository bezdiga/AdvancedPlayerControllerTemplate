using System;
using HatchStudio.Audio;
using HatchStudio.Inventory;
using UnityEngine;

namespace HatchStudio.Surface
{
    [CreateAssetMenu(menuName = "Hatch Studio/Surfaces/Surface Definition", fileName = "Surface_")]
    public class SurfaceDefinition : DataDefinition<SurfaceDefinition>
    {
        [Serializable]
        public sealed class EffectPair
        {
            [Tooltip("Audio Effect")]
            public AdvancedAudioData AudioData;
            
            /*[Tooltip("Visual Effect")]
            public SurfaceEffect VisualEffect;

            [Tooltip("Decal Effect")]
            public SurfaceEffect DecalEffect;*/
        }
        
        [Tooltip("The physical materials assigned to this surface.")]
        public PhysicsMaterial[] Materials = Array.Empty<PhysicsMaterial>();
        
        [Range(0.01f, 2f)]
        [Tooltip("Multiplier applied to character velocity when stepping on this surface.")]
        public float VelocityModifier = 1f;

        [Range(0.01f, 1f)]
        [Tooltip("Determines how rough the surface is, ranging from slippery to rough.")]
        public float SurfaceFriction = 1f;
        
        [Tooltip("The soft footstep effect for the surface. Used by characters when moving slow, such as walking.")]
        public EffectPair SoftFootstepEffect;
        
        [Tooltip("The hard footstep effect for the surface. Used by characters when moving fast, such as running.")]
        public EffectPair HardFootstepEffect;
        
        public override string Name
        {
            get => this != null ? name.Replace("Surface_", "") : string.Empty;
            protected set { }
        }
        
        public EffectPair GetEffectPairOfType(SurfaceEffectType effectType)
        {
            return effectType switch
            {
                SurfaceEffectType.Impact => null,
                SurfaceEffectType.SoftFootstep => SoftFootstepEffect,
                SurfaceEffectType.HardFootstep => HardFootstepEffect,
                _ => throw new ArgumentOutOfRangeException(nameof(effectType), effectType, null)
            };
        }
    }
}