using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class AtmosphereGenerator : MonoBehaviour
    {
        [Header("Settings")]
        public AtmosphereSettings settings;
        public Light sunLight;

        [Header("Size Override")]
        [Range(0f, 2f)]
        [Tooltip("0 = use AtmosphereSettings thickness value")]
        public float thicknessOverride = 0f;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Mesh mesh;

        public void Generate(int resolution, float planetRadius, float maxElevation)
        {
            if (settings == null)
            {
                Debug.LogWarning("[AtmosphereGenerator] No AtmosphereSettings assigned!");
                return;
            }

            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            if (mesh == null)
            {
                mesh = new Mesh { name = "Generated Atmosphere Mesh" };
                meshFilter.sharedMesh = mesh;
            }

            mesh.Clear();

            // Use override if set, otherwise fall back to settings value
            float thickness = thicknessOverride > 0f
                ? thicknessOverride
                : settings.thicknessMultiplier;

            float atmosphereRadius = planetRadius * (1f + thickness);

            Debug.Log($"[AtmosphereGenerator] Radius: {atmosphereRadius} | PlanetRadius: {planetRadius} | Thickness: {thickness}");

            SphereCreator.CreateSphereMesh(
                resolution, atmosphereRadius,
                out Vector3[] vertices,
                out int[] triangles,
                out Vector2[] uv);

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            if (settings.atmosphereMaterial != null)
            {
                meshRenderer.sharedMaterial = settings.atmosphereMaterial;
                meshRenderer.sharedMaterial.SetColor("_Color", settings.atmosphereColor);
            }
            else
            {
                Debug.LogWarning("[AtmosphereGenerator] No atmosphere material assigned!");
            }

            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            AtmosphereSunController controller = GetComponent<AtmosphereSunController>();
            if (controller == null)
                controller = gameObject.AddComponent<AtmosphereSunController>();

            controller.sunLight = sunLight;
            controller.atmosphereRenderer = meshRenderer;
        }

        void Update()
        {
            if (Application.isPlaying && meshRenderer != null && sunLight != null)
            {
                meshRenderer.sharedMaterial.SetVector(
                    "_SunDirection", sunLight.transform.forward);
            }
        }
    }
}