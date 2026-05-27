using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [CreateAssetMenu(
        fileName = "New Terrain Shape Settings",
        menuName = "Solar System/Terrain Shape Settings")]
    public class TerrainShapeSettings : ScriptableObject
    {
        [Tooltip("Noise layers used for procedural terrain deformation.")]
        public NoiseLayer[] noiseLayers;
    }
}