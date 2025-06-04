using UnityEngine;

public class SpectatorModeController : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float mouseSensitivity = 100f;

    private float currentXRotation = 0f;
    private float currentYRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 initialEulerAngles = transform.eulerAngles;
        currentYRotation = initialEulerAngles.y;
        currentXRotation = initialEulerAngles.x;

        if (currentXRotation > 180f)
        {
            currentXRotation -= 360f;
        }
        currentXRotation = Mathf.Clamp(currentXRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0f);
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        currentYRotation += mouseX;
        currentXRotation -= mouseY;
        currentXRotation = Mathf.Clamp(currentXRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0f);

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        float inputY = 0f;

        if (Input.GetKey(KeyCode.Space))
        {
            inputY = 1f;
        }
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            inputY = -1f;
        }

        Vector3 moveDirectionForward = transform.forward * inputZ;
        Vector3 moveDirectionRight = transform.right * inputX;
        Vector3 moveDirectionUp = Vector3.up * inputY;

        transform.position += (moveDirectionForward + moveDirectionRight + moveDirectionUp) * moveSpeed * Time.deltaTime;
    }
}