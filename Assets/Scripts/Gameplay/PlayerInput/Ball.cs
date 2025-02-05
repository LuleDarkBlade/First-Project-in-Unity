using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Sound to play when the ball hits the courtLayer.")]
    public AudioClip VictoryClip;
    [Header("Audio Settings")]
    [Tooltip("Sound to play when the ball hits the courtLayer.")]
    public AudioClip FailureClip;
    public TrailRenderer trail;
	private Rigidbody rb;

	private bool served = false;

	public PlayerController Player { get; set; }
    private AudioSource audioSource;
    private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("courtLayer"))
		{
			if (!served)
			{
				OnBallDropped();
			}
			else if (collision.gameObject.CompareTag("FloorOut"))
			{
				ResetBall();
				Message.ShowMessage("Out!");
				PlayFailureSound();
                audioSource.PlayOneShot(FailureClip);
            }
			else if (collision.gameObject.CompareTag("Net"))
			{
				ResetBall(); 
				Message.ShowMessage("Net!");
			}
            else if (collision.gameObject.CompareTag("FloorInLeft"))
			{
                ResetBall();
                Message.ShowMessage("ACE!");
				PlayVictorySound();
                audioSource.PlayOneShot(VictoryClip);
            }
            else if (collision.gameObject.CompareTag("FloorInRight"))
            {
                ResetBall();
                Message.ShowMessage("ACE!");
                PlayVictorySound();
                audioSource.PlayOneShot(VictoryClip);
            }
        }
	}

	public void ApplyServiceHit(float force)
	{
		rb.isKinematic = false;
		rb.useGravity = true;
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("courtLayer")))
		{
			var hp = hit.point;
			hp.y = transform.position.y;
			Vector3 dir = hp - transform.position;
			rb.linearVelocity = Vector3.zero;
			rb.AddForce(dir.normalized * force, ForceMode.Impulse);
		}
		else
			rb.AddForce(Vector3.forward * force, ForceMode.Impulse);
		served = true;
		trail.enabled = true;
	}
    public void PlayFailureSound()
    {
        if (audioSource != null && FailureClip != null)
        {
            audioSource.PlayOneShot(FailureClip);
            Debug.Log("Failure sound played.");
        }
        else
        {
            Debug.LogWarning("Missing AudioSource or FailureClip!");
        }
    }
    public void PlayVictorySound()
    {
        if (audioSource != null && VictoryClip != null)
        {
            audioSource.PlayOneShot(VictoryClip);
            Debug.Log("Victory sound played.");
        }
        else
        {
            Debug.LogWarning("Missing AudioSource or VictoryClip!");
        }
    }
    private void OnBallDropped()
	{
		ResetBall();
		Message.ShowMessage("Ball Dropped!");
	}

	private void ResetBall()
	{
		
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		trail.enabled = false;
		audioSource = GetComponent<AudioSource>(); // Get AudioSource component
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, transform.forward);
	}
}
