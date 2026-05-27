using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    /// <summary>
    /// Runtime wrapper for a NoiseLayer.
    /// Used for future per-planet overrides (currently minimal implementation).
    /// </summary>
    [System.Serializable]
    public class NoiseLayerInstance
    {
        public NoiseLayer layer;

        [Header("Runtime State")]
        public bool enabled = true;
    }
}