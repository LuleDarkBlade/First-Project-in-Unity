using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 100f; // Speed of rotation

    private void Update()
    {
        // Rotate left when A is pressed
        if (Input.GetKey(KeyCode.A))
        {
            RotateLeft();
        }

        // Rotate right when D is pressed
        if (Input.GetKey(KeyCode.D))
        {
            RotateRight();
        }
    }

    private void RotateLeft()
    {
        transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
    }

    private void RotateRight()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
