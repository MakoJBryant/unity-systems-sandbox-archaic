using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [CreateAssetMenu(
        fileName = "New Terrain Shape Settings",
        menuName = "Solar System/Terrain Shape Settings")]
    public class TerrainShapeSettings : ScriptableObject
    {
        [Header("Global Shape Controls")]
        [Range(-1f, 1f)]
        public float globalHeightOffset = 0f;

        [Header("Noise Layers")]
        public NoiseLayer[] noiseLayers;
    }
}