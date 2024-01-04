using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [SerializeField]
    Transform focus = default;

    [SerializeField, Range(1f, 20f)]
    float distance = 5f;
    
    [SerializeField, Min(0f)]
    float focusRadius = 1f;
    
    [SerializeField, Range(0f, 1f)]
    float focusCentering = 0.5f;

    [SerializeField]
    private Vector3 focusOffset;
    
    [SerializeField, Range(1f, 360f)]
    float rotationSpeed = 90f;
    
    [SerializeField, Range(-89f, 89f)]
    float minVerticalAngle = -30f, maxVerticalAngle = 60f;

    [SerializeField]
    private InputActionReference cameraMovement;

    Vector2 orbitAngles = new Vector2(45f, 0f);
    
    [SerializeField, Min(0f)]
    float alignDelay = 5f;
    
    [SerializeField, Range(0f, 90f)]
    float alignSmoothRange = 45f;
    
    Vector3 focusPoint, previousFocusPoint;
    float lastManualRotationTime;
    
    
    void Awake () {
        focusPoint = focus.position;
        transform.localRotation = Quaternion.Euler(orbitAngles);
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void OnValidate () {
        if (maxVerticalAngle < minVerticalAngle) {
            maxVerticalAngle = minVerticalAngle;
        }
    }

    bool ManualRotation ()
    {
        Vector2 input = cameraMovement.action.ReadValue<Vector2>();
        input = new Vector2(-input.y, input.x);
        const float e = 0.001f;
        if (input.x < -e || input.x > e || input.y < -e || input.y > e) {
            orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
            lastManualRotationTime = Time.unscaledTime;
            return true;
        }

        return false;
    }
    
    void LateUpdate () {
        UpdateFocusPoint();
        Quaternion lookRotation;
        if (ManualRotation() || AutomaticRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(orbitAngles);
        }
        else
        {
            lookRotation = transform.localRotation;
        }
        
        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * distance;
        transform.SetPositionAndRotation(lookPosition, lookRotation);
        ManualRotation();
    }

    void UpdateFocusPoint () {
        previousFocusPoint = focusPoint;
        Vector3 targetPoint = focus.position + focusOffset;
        if (focusRadius > 0f) {
            float distance = Vector3.Distance(targetPoint, focusPoint);
            float t = 1f;
            if (distance > 0.01f && focusCentering > 0f) {
                t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
            }
            if (distance > focusRadius) {
                t = Mathf.Min(t, focusRadius / distance);
            }
            focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
            if (distance > focusRadius) {
                focusPoint = Vector3.Lerp(
                    targetPoint, focusPoint, focusRadius / distance
                );
            }
        }
        else {
            focusPoint = targetPoint;
        }
    }
    
    void ConstrainAngles () {
        orbitAngles.x =
            Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

        if (orbitAngles.y < 0f) {
            orbitAngles.y += 360f;
        }
        else if (orbitAngles.y >= 360f) {
            orbitAngles.y -= 360f;
        }
    }
    
    static float GetAngle (Vector2 direction) {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }
    
    bool AutomaticRotation () {
        if (Time.unscaledTime - lastManualRotationTime < alignDelay) {
            return false;
        }
        
        Vector2 movement = new Vector2(
            focusPoint.x - previousFocusPoint.x,
            focusPoint.z - previousFocusPoint.z
        );
        float movementDeltaSqr = movement.sqrMagnitude;
        if (movementDeltaSqr < 0.0001f) {
            return false;
        }
        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
        float rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
        if (deltaAbs < alignSmoothRange) {
            rotationChange *= deltaAbs / alignSmoothRange;
        }
        else if (180f - deltaAbs < alignSmoothRange) {
            rotationChange *= (180f - deltaAbs) / alignSmoothRange;
        }
        orbitAngles.y =
            Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
        return true;
    }
}
