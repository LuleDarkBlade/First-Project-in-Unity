using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 inputDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Prevent physics from rotating the player unexpectedly
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // 1) Movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // 2) Calculate speed for animation
        float currentSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
        animator.SetFloat("Speed", currentSpeed);

        // 3) Face movement direction
        if (inputDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(inputDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // 4) Shot input
        if (Input.GetButtonDown("Fire1")) // e.g., left Ctrl or mouse button
        {
            animator.SetTrigger("Forehand");
            HitBall(); // We'll define the logic below
        }
        else if (Input.GetButtonDown("Fire2")) // e.g., right Alt or second mouse button
        {
            animator.SetTrigger("Forehand Strafe");
            HitBall();
        }
    }

    private void FixedUpdate()
    {
        // Move the player
        Vector3 velocity = inputDirection * moveSpeed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }

    private void HitBall()
    {
        // Very simple approach: find the ball close to the player and apply force.
        Collider[] hits = Physics.OverlapSphere(transform.position, 1f);
        foreach (var h in hits)
        {
            if (h.CompareTag("TennisBall"))
            {
                Rigidbody ballRb = h.GetComponent<Rigidbody>();
                if (ballRb != null)
                {
                    Vector3 shotDir = (transform.forward + Vector3.up * 0.3f).normalized;
                    float shotPower = 10f;
                    ballRb.velocity = shotDir * shotPower;
                }
            }
        }
    }
}
