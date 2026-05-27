using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    /// <summary>
    /// Runtime override wrapper for a NoiseLayer.
    /// Allows per-planet modification of shared NoiseLayer assets.
    /// </summary>
    [System.Serializable]
    public class NoiseLayerInstance
    {
        public NoiseLayer layer;

        [Header("Runtime Overrides")]

        /// <summary>
        /// If false, this layer is completely ignored at runtime.
        /// </summary>
        public bool enabledOverride = true;

        public bool overrideStrength = false;
        public float strength = 0f;

        public bool overrideRoughness = false;
        public float roughness = 0f;

        public bool overridePersistence = false;
        public float persistence = 0f;

        public bool overrideLacunarity = false;
        public float lacunarity = 0f;

        public bool overrideOctaves = false;
        public int octaves = 0;

        public bool overrideOffset = false;
        public Vector3 offset = Vector3.zero;
    }
}