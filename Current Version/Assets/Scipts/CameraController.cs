using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraController : MonoBehaviour
{
    [SerializeField] bool lockCursor = true;
    [SerializeField] float mouseSensitivity = 10;
    [SerializeField] Transform target;
    [SerializeField] float distanceFromTarget = 2;
    [SerializeField] Vector2 pitchMinMax = new Vector2(-40,85);
    [SerializeField] float rotationSmoothTime = .1f;
    GameObject focused;

    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    float yaw, pitch;
    public void SetFocus(GameObject f)
    {
        focused = f;
    }
    private void Start()
    {
        GetComponent<ColorCorrectionCurves>().enabled = false;
        focused = null;
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x,pitchMinMax.y);

       

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        if (focused != null)
        {
            transform.LookAt(focused.transform);
        }

        transform.position = target.position - transform.forward * distanceFromTarget;
    }
}
