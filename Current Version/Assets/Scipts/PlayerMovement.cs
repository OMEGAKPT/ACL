using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] float runSpeed = 6;
    [SerializeField] float turnSmoothTime = 0.2f;
    [SerializeField] float speedSmoothTime = 0.1f;

    float turnSmoothVelocity;
    float speedSmoothVelocity;
    float currentSpeed;

    bool walking;

    Animator ani;
    Transform cameraT;
    
    void Start()
    {
        cameraT = Camera.main.transform;
        ani = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input = input.normalized;

        if (input != Vector2.zero)
        {
            walking = true;
            float targetRotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }
        else
            walking = false;

        ani.SetBool("Walking", walking);

        currentSpeed = Mathf.SmoothDamp(currentSpeed, runSpeed * input.magnitude, ref speedSmoothVelocity, speedSmoothTime);

        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
    }
}
