using UnityEngine;

public class lookCamera : MonoBehaviour
{
    public float rotationSpeed = 1f;

    private bool isRotating = false;
    private Vector3 lastMousePosition;
    private Vector3 prevCameraPosition;

    private Vector3 cameraPosition;

    private void Start()
    {
        cameraPosition = transform.position;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button down
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(1)) // Right mouse button up
        {
            isRotating = false;
        }

        if (isRotating)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotationX = delta.x * rotationSpeed;
            float rotationY = -delta.y * rotationSpeed; // Invert Y-axis rotation for natural movement

            transform.RotateAround(Vector3.zero, Vector3.up, rotationX);
            transform.RotateAround(Vector3.zero, transform.right, rotationY);

            lastMousePosition = Input.mousePosition;
        }
    }
}
