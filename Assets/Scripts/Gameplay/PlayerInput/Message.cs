using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
	public TMP_Text messageText;
	public float displayTime = 3.0f;

	private static Message instance;
	private float timer;

	public static void ShowMessage(string message)
	{
		if (instance == null)
		{
			Debug.LogError("No Message instance found in the scene.");
			return;
		}
		instance.messageText.text = message;
		instance.timer = instance.displayTime;
	}

	void Awake()
	{
		instance = this;
	}

	void Update()
	{
		if (timer > 0)
		{
			timer -= Time.deltaTime;
			if (timer <= 0)
			{
				messageText.text = "";
			}
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}
}
