using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [CreateAssetMenu(
        fileName = "New Terrain Color Settings",
        menuName = "Solar System/Terrain Color Settings")]
    public class TerrainColorSettings : ScriptableObject
    {
        public ColorRegion[] regions;

        [System.Serializable]
        public struct ColorRegion
        {
            public string name;
            public Color color;

            [Range(0f, 1f)]
            public float startHeight;

            [Range(0.01f, 1f)]
            public float blendAmount;

            public int priority; // override (future proofing)
        }
    }
}