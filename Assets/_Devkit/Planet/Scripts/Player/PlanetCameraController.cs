using UnityEngine;

public class PlanetCameraController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform planet;
    public Transform cameraPivot;
    public Camera cam;
    public PlanetMover planetMover;

    [Header("Settings")]
    public float mouseSensitivity = 2f;
    public float minPitch = -80f;
    public float maxPitch = 80f;
    public float cameraDistance = 4f;
    public float cameraCollisionRadius = 0.3f;

    float pitch = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (planetMover == null && player != null)
            planetMover = player.GetComponent<PlanetMover>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void LateUpdate()
    {
        if (player == null || planet == null || cameraPivot == null || cam == null)
            return;

        Vector3 gravityUp = (player.position - planet.position).normalized;

        // Accumulate pitch from mouse Y
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Set pivot position to player
        cameraPivot.position = player.position;

        // Apply pitch on top of player's world rotation in one clean operation
        cameraPivot.rotation = player.rotation * Quaternion.Euler(pitch, 0f, 0f);

        // Camera position + collision
        Vector3 desiredCamPos = cameraPivot.position - cameraPivot.forward * cameraDistance;

        if (Physics.SphereCast(
            cameraPivot.position,
            cameraCollisionRadius,
            -cameraPivot.forward,
            out RaycastHit hit,
            cameraDistance,
            ~0,
            QueryTriggerInteraction.Ignore))
        {
            desiredCamPos = hit.point + hit.normal * cameraCollisionRadius;
        }

        cam.transform.position = desiredCamPos;
        cam.transform.LookAt(cameraPivot.position, gravityUp);
    }
}