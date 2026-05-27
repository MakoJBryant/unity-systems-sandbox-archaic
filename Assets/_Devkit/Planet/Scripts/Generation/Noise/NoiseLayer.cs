using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    /// <summary>
    /// Defines a reusable procedural noise layer used in terrain generation.
    /// This is a shared asset and should NOT contain runtime overrides.
    /// </summary>

    public enum NoiseType
    {
        Standard,
        Ridge
    }

    [CreateAssetMenu(
        fileName = "NoiseLayer",
        menuName = "Planet Generation/Noise Layer")]
    public class NoiseLayer : ScriptableObject
    {
        [Header("Layer State")]

        [Tooltip("If disabled, this layer will be ignored during terrain generation.")]
        public bool enabled = true;

        [Header("Shape")]

        [Tooltip("Overall amplitude of terrain displacement caused by this layer.")]
        public float strength = 1f;

        [Tooltip("Base frequency of the noise. Higher values create more detail.")]
        public float roughness = 2f;

        [Tooltip("Number of noise octaves blended together for detail complexity.")]
        public int octaves = 4;

        [Header("Noise Behavior")]

        [Tooltip("Amplitude reduction per octave.")]
        [Range(0f, 1f)]
        public float persistence = 0.5f;

        [Tooltip("Frequency multiplier per octave.")]
        public float lacunarity = 2f;

        [Tooltip("World-space offset applied to noise sampling.")]
        public Vector3 offset = Vector3.zero;

        [Header("Shaping")]

        [Tooltip("Baseline offset applied before noise evaluation.")]
        public float minValue = 0f;

        [Tooltip("Noise interpretation mode.")]
        public NoiseType noiseType = NoiseType.Standard;

        [Tooltip("If enabled, this layer can be masked by the first valid layer.")]
        public bool useFirstLayerAsMask = false;
    }
}