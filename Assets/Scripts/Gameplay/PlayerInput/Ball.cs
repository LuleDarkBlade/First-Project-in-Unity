using UnityEngine;

public class Ball : MonoBehaviour
{
	public TrailRenderer trail;
	private Rigidbody rb;

	private bool served = false;

	public PlayerController Player { get; set; }

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
            }
            else if (collision.gameObject.CompareTag("FloorInRight"))
            {
                ResetBall();
                Message.ShowMessage("ACE!");
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
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, transform.forward);
	}
}
