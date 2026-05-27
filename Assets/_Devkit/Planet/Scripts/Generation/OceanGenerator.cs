using UnityEngine;

namespace MakoJBryant.SolarSystem.Generation
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class OceanGenerator : MonoBehaviour
    {
        [Header("Settings")]
        public OceanSettings settings;

        [Header("Sea Level")]
        [Range(0f, 1f)]
        public float seaLevel = 0.5f;
        public float manualOceanRadius = 0f;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Mesh mesh;

        public void Generate(int resolution, float minElevation, float maxElevation)
        {
            if (settings == null)
            {
                Debug.LogWarning("[OceanGenerator] No OceanSettings assigned!");
                return;
            }

            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            if (mesh == null)
            {
                mesh = new Mesh { name = "Generated Ocean Mesh" };
                meshFilter.sharedMesh = mesh;
            }

            mesh.Clear();

            float oceanRadius = manualOceanRadius > 0f
                ? manualOceanRadius
                : Mathf.Lerp(minElevation, maxElevation, seaLevel);

            Debug.Log($"[OceanGenerator] OceanRadius: {oceanRadius} | MinElev: {minElevation} | MaxElev: {maxElevation} | SeaLevel: {seaLevel}");

            SphereCreator.CreateSphereMesh(
                resolution, oceanRadius,
                out Vector3[] vertices,
                out int[] triangles,
                out Vector2[] uv);

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            if (settings.oceanMaterial != null)
            {
                meshRenderer.sharedMaterial = settings.oceanMaterial;
                meshRenderer.sharedMaterial.SetFloat("_Radius", oceanRadius);
                meshRenderer.sharedMaterial.SetColor("_Color", settings.oceanColor);
                meshRenderer.sharedMaterial.SetVector("_PlanetCenter",
                    transform.parent ? transform.parent.position : Vector3.zero);
            }
            else
            {
                Debug.LogWarning("[OceanGenerator] OceanSettings has no material assigned!");
            }

            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            Debug.Log($"[OceanGenerator] Mesh has {mesh.vertexCount} vertices. Material: {meshRenderer.sharedMaterial?.name}");
        }
    }
}