using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    [Header("Aiming Settings")]
    [Tooltip("The plane object used for aiming (its collider must be active).")]
    public GameObject aimPlane;
    [Tooltip("Max raycast distance for mouse aiming.")]
    public float maxAimDistance = 50f;

    [Header("Timing Settings")]
    [Tooltip("Distance threshold for perfect timing (ball very close to racquet).")]
    public float perfectDistanceThreshold = 0.3f;
    [Tooltip("Distance threshold for normal timing.")]
    public float normalDistanceThreshold = 0.8f;
    [Tooltip("Force adjustment for perfect timing.")]
    public float perfectTimingMultiplier = 1.2f;
    [Tooltip("Force adjustment for normal timing.")]
    public float normalTimingMultiplier = 1f;
    [Tooltip("Force adjustment for bad timing.")]
    public float badTimingMultiplier = 0.8f;

    [Header("UI Feedback")]
    [Tooltip("UI Text to display timing feedback.")]
    public Text timingFeedbackText;
    [Tooltip("Color for perfect timing feedback.")]
    public Color perfectTimingColor = Color.green;
    [Tooltip("Color for normal timing feedback.")]
    public Color normalTimingColor = Color.yellow;
    [Tooltip("Color for bad timing feedback.")]
    public Color badTimingColor = Color.red;

    // Internal references
    private Animator animator;
    private Rigidbody rb;
    private Vector3 inputDir;

    private GameObject currentBall;
    private Rigidbody currentBallRb;

    private int servePhase = 0; // 0 = ready, 1 = ball tap, 2 = prep, 3 = swing
    private bool ballHasSpawned = false; // Ensure ball spawns only once
    private Vector3 aimDirection = Vector3.forward; // Default hit direction
    private string currentTimingFeedback = "Normal"; // Default timing feedback

    // Animator parameters (do not change these names)
    private static readonly int SpeedParamHash = Animator.StringToHash("Speed");
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
        rb.freezeRotation = true; // Prevent physics wobble

        if (timingFeedbackText)
            timingFeedbackText.text = "Timing";
    }

    private void Update()
    {
        HandleMovement();
        HandleAiming();
if (Camera.main != null)
    Debug.Log("Main Camera is: " + Camera.main.name);
else
    Debug.LogWarning("No camera tagged MainCamera!");

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
				animator.ResetTrigger(ServiceBallTapTrigger);
				animator.SetTrigger(ServicePrepTrigger);
            }
            else if (servePhase == 2)
            {
                servePhase = 3;
                Debug.Log("F Press #3 => ServiceSwing");
				animator.ResetTrigger(ServicePrepTrigger);
				animator.SetTrigger(ServiceSwingTrigger);
                inputDir = Vector3.zero; // Reset movement input after serve
                //StartCoroutine(CheckShotHit());
            }
            else
            {
                Debug.Log("Serve complete. Resetting for next serve.");
                servePhase = 0;
                ballHasSpawned = false; // Reset ball spawn logic
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
            ApplyForehandHit(CalculateTiming());
            StartCoroutine(CheckShotHit());
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
            ApplyBackhandHit(CalculateTiming());
            StartCoroutine(CheckShotHit());
        }
    }

    private void FixedUpdate()
    {
        Vector3 velocity = inputDir * moveSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    // ================= MOVEMENT =================
    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        inputDir = new Vector3(h, 0f, v).normalized;

        float currentSpeed = inputDir.magnitude * moveSpeed;
        animator.SetFloat(SpeedParamHash, currentSpeed);

        if (inputDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(inputDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    // ================= AIMING =================
    private void HandleAiming()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        Debug.DrawRay(ray.origin, ray.direction * maxAimDistance, Color.yellow, 5f);
        // Use the aim plane instead of a layer mask:
        if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance))
        {
            if (hit.collider.gameObject == aimPlane)
            {
                Vector3 targetPoint = hit.point;
                aimDirection = (targetPoint - transform.position).normalized;
                Debug.DrawLine(transform.position, targetPoint, Color.blue);
            }
            else
            {
                // If the raycast hits something else, use a default aim direction
                aimDirection = (transform.forward + Vector3.up * 0.3f).normalized;
                Debug.LogWarning("Raycast hit an unexpected object; using default aim direction.");
            }
        }
        else
        {
            // If the raycast misses entirely, default the aim direction
            aimDirection = (transform.forward + Vector3.up * 0.3f).normalized;
            Debug.LogWarning("Raycast missed; using default aim direction.");
        }
    }

    // ================= TIMING =================
    private float CalculateTiming()
    {
        if (currentBall == null)
        {
            SetTimingFeedback("No Ball", Color.white);
            return normalTimingMultiplier;
        }

        // Use the distance between the ball and the left hand tip (racquet position)
        float distance = Vector3.Distance(currentBall.transform.position, leftHandTip.position);

        if (distance <= perfectDistanceThreshold)
        {
            SetTimingFeedback("Perfect Timing", perfectTimingColor);
            return perfectTimingMultiplier;
        }
        else if (distance <= normalDistanceThreshold)
        {
            SetTimingFeedback("Normal Timing", normalTimingColor);
            return normalTimingMultiplier;
        }
        else
        {
            SetTimingFeedback("Bad Timing", badTimingColor);
            return badTimingMultiplier;
            
        }
    }

    private void SetTimingFeedback(string feedbackText, Color feedbackColor)
    {
        currentTimingFeedback = feedbackText;  // Now it's used to store the current feedback
        if (timingFeedbackText)
        {
            timingFeedbackText.text = feedbackText;
            timingFeedbackText.color = feedbackColor;
        }
    }

    // ================= SERVICE EVENTS =================
    public void SpawnBallAtLeftHand()
    {
        if (ballHasSpawned) return;

        if (tennisBallPrefab == null || leftHandTip == null)
        {
            Debug.LogWarning("Missing tennisBallPrefab or leftHandTip!");
            return;
        }

        currentBall = Instantiate(tennisBallPrefab, leftHandTip.position, leftHandTip.rotation);
        currentBallRb = currentBall.GetComponent<Rigidbody>();

        currentBall.transform.SetParent(leftHandTip);
        currentBallRb.isKinematic = true;

        ballHasSpawned = true;

        Debug.Log("Spawned ball in left hand (ServiceBallTap).");
    }

    public void DropAndBounceBall()
    {
        if (currentBall == null || currentBallRb == null) return;

        currentBall.transform.SetParent(null);
        currentBallRb.isKinematic = false;

        Vector3 dropDir = Vector3.down;
        float dropForce = 0.5f; // Force to bounce
        currentBallRb.linearVelocity = dropDir * dropForce;

        Debug.Log("Ball dropped and bounced (ServiceBallTap).");
    }

    public void ResetService()
    {
        ResetBallToLeftHand();
		servePhase = 0;
        rb.linearVelocity = Vector3.zero;
	}

    public void ResetBallToLeftHand()
    {
        if (currentBall == null || currentBallRb == null) return;

        currentBallRb.linearVelocity = Vector3.zero;

        currentBall.transform.SetParent(leftHandTip);
        currentBall.transform.localPosition = Vector3.zero;
        currentBall.transform.localRotation = Quaternion.identity;

        currentBallRb.isKinematic = true;

        Debug.Log("Ball reset to left hand.");
    }

    public void ThrowBallUp()
    {
        if (currentBall == null || currentBallRb == null)
        {
            Debug.LogWarning("No ball to throw up!");
            return;
        }

        currentBall.transform.SetParent(null);
        currentBallRb.isKinematic = false;
		currentBallRb.linearVelocity = Vector3.zero;
		var ball = currentBall.gameObject.GetComponent<Ball>();
        ball.Player = this;

		Vector3 throwDir = Vector3.up;
        float throwForce = 12f;
        currentBallRb.linearVelocity = throwDir * throwForce;

        Debug.Log("Ball thrown upward (ServicePrep).");
    }

    public void ApplyServiceHit()
    {
        if (currentBall == null || currentBallRb == null)
        {
            Debug.LogWarning("No ball to service-hit!");
            return;
        }

        var ball = currentBall.gameObject.GetComponent<Ball>();
        ball.Player = this;
        ball.ApplyServiceHit(35f); // Force value is arbitrary
		animator.ResetTrigger(ServiceSwingTrigger);

		//currentBallRb.isKinematic = false;

		//Vector3 serviceDir = aimDirection + Vector3.up * 0.5f; // Ensure proper arc
		//float serviceForce = 20f * CalculateTiming();
		//currentBallRb.linearVelocity = serviceDir.normalized * serviceForce;

		//Debug.Log("Service hit applied.");
		//StartCoroutine(CheckShotHit());
	}

    public void ApplyForehandHit(float timingMultiplier)
    {
        if (currentBall == null || currentBallRb == null)
        {
            Debug.LogWarning("No ball for ForehandHit!");
            return;
        }

        Vector3 forehandDir = aimDirection + Vector3.up * 0.3f; // Arc adjustment
        float forehandForce = 10f * timingMultiplier;
        currentBallRb.linearVelocity = forehandDir.normalized * forehandForce;

        Debug.Log("Forehand hit applied.");
        StartCoroutine(CheckShotHit());
    }

    public void ApplyBackhandHit(float timingMultiplier)
    {
        if (currentBall == null || currentBallRb == null)
        {
            Debug.LogWarning("No ball for BackhandHit!");
            return;
        }

        Vector3 backhandDir = aimDirection + Vector3.up * 0.3f; // Arc adjustment
        float backhandForce = 9f * timingMultiplier;
        currentBallRb.linearVelocity = backhandDir.normalized * backhandForce;

        Debug.Log("Backhand hit applied.");
        StartCoroutine(CheckShotHit());
    }

    // ================= COLLISION & SHOT CHECK =================
    private bool shotHitRegistered = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (currentBall != null && collision.gameObject == currentBall)
        {
            Debug.Log("Collision detected between racquet and ball.");
            shotHitRegistered = true;
        }
    }

    private IEnumerator CheckShotHit()
    {
        shotHitRegistered = false;
        yield return new WaitForSeconds(1f);
        if (!shotHitRegistered)
        {
            Debug.Log("Shot failed: no collision between racquet and ball. Resetting ball.");
            ResetBallToLeftHand();
        }
    }
    // ================= RESET SERVE CYCLE =================
    // Call this method via an Animation Event at the end of the ServiceSwing animation.
    public void ResetServeCycle()
    {
        if (currentBall != null)
        {
            Destroy(currentBall, 5f);
            currentBall = null;
            currentBallRb = null;
        }
        servePhase = 0;
        ballHasSpawned = false;
        Debug.Log("Serve cycle reset. Ready for next serve.");
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
