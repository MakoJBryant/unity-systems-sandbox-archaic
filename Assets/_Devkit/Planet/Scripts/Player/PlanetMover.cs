using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanetMover : MonoBehaviour
{
    [Header("References")]
    public Transform planet;

    [Header("Movement")]
    public float moveSpeed = 24f;
    public float acceleration = 20f;
    public float jumpForce = 24f;

    [Header("Gravity")]
    public float gravityStrength = 30f;
    public float gravityAlignSpeed = 10f;

    [Header("Look")]
    public float mouseSensitivity = 2f;

    [Header("Grounding")]
    public LayerMask groundMask;
    public float groundCheckDistance = 1.5f;

    Rigidbody rb;
    bool isGrounded;
    bool jumpRequested;

    // Public so camera can read it
    [HideInInspector] public float currentYaw = 0f;
    [HideInInspector] public Vector3 gravityUp;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.angularDamping = 10f;
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            jumpRequested = true;

        // Yaw in Update — same timestep as camera pitch
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        if (Mathf.Abs(mouseX) < 0.001f) mouseX = 0f;
        currentYaw += mouseX;

        // Update gravityUp every frame so camera can use it too
        if (planet != null)
            gravityUp = (transform.position - planet.position).normalized;
    }

    void FixedUpdate()
    {
        ApplyGravity();
        AlignToGravity();
        CheckGround();
        HandleMovement();
        HandleJump();
    }

    void ApplyGravity()
    {
        Vector3 gravityDir = (planet.position - transform.position).normalized;
        rb.AddForce(gravityDir * gravityStrength, ForceMode.Acceleration);
    }

    void AlignToGravity()
    {
        Vector3 up = (transform.position - planet.position).normalized;
        Quaternion gravityAlignment = Quaternion.FromToRotation(Vector3.up, up);
        Quaternion targetRotation = gravityAlignment * Quaternion.Euler(0f, currentYaw, 0f);
        rb.MoveRotation(targetRotation);
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 up = (transform.position - planet.position).normalized;
        Vector3 input = new Vector3(h, 0f, v).normalized;
        Vector3 desiredVelocity = transform.TransformDirection(input) * moveSpeed;

        Vector3 velocity = rb.linearVelocity;
        Vector3 surfaceVelocity = Vector3.ProjectOnPlane(velocity, up);
        Vector3 verticalVelocity = Vector3.Project(velocity, up);

        Vector3 velocityChange = desiredVelocity - surfaceVelocity;
        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
        rb.linearVelocity = surfaceVelocity + verticalVelocity;
    }

    void CheckGround()
    {
        Vector3 gravityDir = (transform.position - planet.position).normalized;
        isGrounded = Physics.Raycast(
            transform.position,
            -gravityDir,
            groundCheckDistance,
            groundMask
        );
    }

    void HandleJump()
    {
        if (!jumpRequested) return;
        jumpRequested = false;
        if (!isGrounded) return;

        Vector3 up = (transform.position - planet.position).normalized;
        rb.linearVelocity -= Vector3.Project(rb.linearVelocity, up);
        rb.AddForce(up * jumpForce, ForceMode.VelocityChange);
    }
}