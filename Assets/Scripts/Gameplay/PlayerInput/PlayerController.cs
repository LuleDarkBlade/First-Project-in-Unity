using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 360f;

    [Header("Ball / Left Hand Setup")]
    [Tooltip("Your tennis ball prefab (Rigidbody+SphereCollider).")]
    public GameObject tennisBallPrefab;

    [Tooltip("Child transform on the left hand bone for ball spawn/holding.")]
    public Transform leftHandTip;

    // references
    private Animator animator;
    private Rigidbody rb;
    private Vector3 inputDir;

    // We'll store the currently spawned ball + rigidbody
    private GameObject currentBall;
    private Rigidbody currentBallRb;

    // Serve phase logic:
    private int servePhase = 0; // 0 = ready, 1 = ball tap, 2 = prep, 3 = swing

    // Animator params/triggers
    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    private static readonly int ServiceBallTapTrigger = Animator.StringToHash("ServiceBallTapTrigger");
    private static readonly int ServicePrepTrigger = Animator.StringToHash("ServicePrepTrigger");
    private static readonly int ServiceSwingTrigger = Animator.StringToHash("ServiceSwingTrigger");

    private static readonly int IsForehandPrepBool = Animator.StringToHash("IsForehandPrep");
    private static readonly int ForehandSwingTrigger = Animator.StringToHash("ForehandSwingTrigger");
    private static readonly int IsBackhandPrepBool = Animator.StringToHash("IsBackhandPrep");
    private static readonly int BackhandSwingTrigger = Animator.StringToHash("BackhandSwingTrigger");

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Freeze rotation so player won't topple over with physics
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // ============== MOVEMENT ==============
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        inputDir = new Vector3(h, 0f, v).normalized;

        float currentSpeed = inputDir.magnitude * moveSpeed;
        animator.SetFloat(SpeedParam, currentSpeed);

        // Rotate toward input direction
        if (inputDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(inputDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        // ============ SERVICE SEQUENCE ============

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (servePhase == 0)
            {
                servePhase = 1;
                Debug.Log("F Press #1 => ServiceBallTap");
                animator.SetTrigger(ServiceBallTapTrigger);
            }
            else if (servePhase == 1)
            {
                servePhase = 2;
                Debug.Log("F Press #2 => ServicePrep");
                animator.SetTrigger(ServicePrepTrigger);
            }
            else if (servePhase == 2)
            {
                servePhase = 3;
                Debug.Log("F Press #3 => ServiceSwing");
                animator.SetTrigger(ServiceSwingTrigger);

                // Reset movement input after serving
                inputDir = Vector3.zero;
            }
            else
            {
                Debug.Log("Serve complete. Resetting for next serve.");
                servePhase = 0;
            }
        }

        // ============ FOREHAND (Fire1) ============
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Fire1 pressed => ForehandPrep ON");
            animator.SetBool(IsForehandPrepBool, true);
        }
        if (Input.GetButtonUp("Fire1"))
        {
            Debug.Log("Fire1 released => ForehandSwing triggered");
            animator.SetBool(IsForehandPrepBool, false);
            animator.SetTrigger(ForehandSwingTrigger);
        }

        // ============ BACKHAND (Fire2) ============
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Fire2 pressed => BackhandPrep ON");
            animator.SetBool(IsBackhandPrepBool, true);
        }
        if (Input.GetButtonUp("Fire2"))
        {
            Debug.Log("Fire2 released => BackhandSwing triggered");
            animator.SetBool(IsBackhandPrepBool, false);
            animator.SetTrigger(BackhandSwingTrigger);
        }
    }

    private void FixedUpdate()
    {
        Vector3 velocity = inputDir * moveSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    // =======================================================================================
    // ServiceBallTap clip event => spawn the ball in left hand (only once)
    // =======================================================================================
    public void SpawnBallAtLeftHand()
    {
        if (currentBall != null)
        {
            Debug.LogWarning("Ball already spawned. Skipping spawn.");
            return;
        }

        if (tennisBallPrefab == null || leftHandTip == null)
        {
            Debug.LogWarning("Missing tennisBallPrefab or leftHandTip!");
            return;
        }

        currentBall = Instantiate(tennisBallPrefab, leftHandTip.position, leftHandTip.rotation);
        currentBallRb = currentBall.GetComponent<Rigidbody>();

        currentBall.transform.SetParent(leftHandTip);
        currentBallRb.isKinematic = true;

        Debug.Log("Spawned ball in left hand (ServiceBallTap).");
    }

    // =======================================================================================
    // ServicePrep clip event => throw the ball up
    // =======================================================================================
    public void ThrowBallUp()
    {
        if (currentBall == null || currentBallRb == null)
        {
            Debug.LogWarning("No ball to throw up!");
            return;
        }

        currentBall.transform.SetParent(null);
        currentBallRb.isKinematic = false;

        Vector3 throwDir = Vector3.up;
        float throwForce = 8f;
        currentBallRb.linearVelocity = throwDir * throwForce;

        Debug.Log("Ball thrown upward (ServicePrep).");
    }

    // =======================================================================================
    // ServiceSwing clip event => final forward strike
    // =======================================================================================
    public void ApplyServiceHit()
    {
        if (currentBall == null || currentBallRb == null)
        {
            Debug.LogWarning("No ball to service-hit!");
            return;
        }

        currentBallRb.isKinematic = false;

        Vector3 serviceDir = (transform.forward + Vector3.up * 0.3f).normalized;
        float serviceForce = 12f;
        currentBallRb.linearVelocity = serviceDir * serviceForce;

        Debug.Log("Service swing applied => ball launched forward!");
    }

    // =======================================================================================
    // ForehandSwing clip event => normal forehand contact
    // =======================================================================================
    public void ApplyForehandHit()
    {
        if (currentBall == null || currentBallRb == null)
        {
            Debug.LogWarning("No ball for ForehandHit!");
            return;
        }

        Vector3 forehandDir = transform.forward;
        float forehandForce = 10f;
        currentBallRb.linearVelocity = forehandDir * forehandForce;

        Debug.Log("Forehand contact => launched ball forward!");
    }

    // =======================================================================================
    // BackhandSwing clip event => normal backhand contact
    // =======================================================================================
    public void ApplyBackhandHit()
    {
        if (currentBall == null || currentBallRb == null)
        {
            Debug.LogWarning("No ball for BackhandHit!");
            return;
        }

        Vector3 backhandDir = transform.forward;
        float backhandForce = 9f;
        currentBallRb.linearVelocity = backhandDir * backhandForce;

        Debug.Log("Backhand contact => launched ball forward!");
    }
}
