using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class ObserverController : MonoBehaviour
{
    private const float SPEED_CONSTNANT = 0.01f;
    private const float SENSITIVITY_CONSTANT = 5f;

    public Camera camera;

    public float speed = 1.0f;
    public float sensitivity = 1.0f;

    private float currentXRotation = 0f;
    public float maxYAngle = 10f;

    float finalSpeed { get => speed * SPEED_CONSTNANT; }
    float finalSensitivity { get => sensitivity * SENSITIVITY_CONSTANT * Time.deltaTime; }

    void Awake()
    {
        camera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        UpdateMovement();
        UpdateRotation();
    }

    private void UpdateMovement()
    {
        Vector3 moveDelta = UpdateXZMovment() + UpdateYMovement();
        Vector3 targetPosition = transform.position + moveDelta;
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);
    }

    private Vector3 UpdateXZMovment()
    {
        Vector3 keyAxis = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        if (keyAxis.sqrMagnitude < 0.01f)
        {
            return Vector3.zero;
        }

        Vector3 forwardVector = camera.transform.forward;
        Quaternion fowardQuaternion = Quaternion.FromToRotation(Vector3.forward, forwardVector);

        Vector3 moveVector = fowardQuaternion * keyAxis;

        return moveVector * finalSpeed;
    }

    private Vector3 UpdateYMovement()
    {
        float delta = 0.0f;
        if (Input.GetKey(KeyCode.Q))
        {
            delta -= 1.0f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            delta += 1.0f;
        }
        if (Mathf.Approximately(delta, 0.0f))
        {
            return Vector3.zero;
        }

        return Vector3.up * delta * finalSpeed;
    }

    private void UpdateRotation()
    {
        Vector3 mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).normalized * finalSensitivity;

        transform.Rotate(Vector3.up, mouseDelta.x);

        currentXRotation += mouseDelta.y;
        currentXRotation = Mathf.Clamp(currentXRotation, -maxYAngle, maxYAngle);
        camera.transform.localEulerAngles = new Vector3(-currentXRotation, 0f, 0f);
    }
}
