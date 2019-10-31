using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    PlayerInputs pi;

    public int RunSpeed { get; set; }
    public int JumpSpeed { get; set; }

    float turnSmoothTime;
    float speedSmoothTime;
   

    float turnSmoothVelocity;
    float speedSmoothVelocity;
    float currentSpeed;

    public Vector2 InputC { get; set; }
    Transform cameraT;

    void Start()
    {
        pi = GetComponent<PlayerInputs>();
        RunSpeed = (int)Speed.Walking;
        JumpSpeed = (int)Jump.Running;

        turnSmoothTime = 0.2f;
        speedSmoothTime = 0.1f;
        

        cameraT = Camera.main.transform;
        
        InputC = new Vector2();
    }

    void Update()
    {
        if (pi.Attacking)
        {
            InputC = Vector2.zero;
        }
        InputMove();
        Movement();
    }

    public void InputMove()
    {
        if (!pi.Jumping && !pi.Attacking)
        {
            InputC = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            InputC = InputC.normalized;
        }
    }


    public void Movement()
    {
        if (!pi.Jumping)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, RunSpeed * InputC.magnitude, ref speedSmoothVelocity, speedSmoothTime);
            transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
        }

        if (InputC != Vector2.zero && !pi.Jumping && !pi.Attacking)
        {
            RunSpeed = (int)Speed.Walking;
            float targetRotation = Mathf.Atan2(InputC.x, InputC.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }
        else
        {
            RunSpeed = (int)Speed.Idle;
            JumpSpeed = (int)Jump.Idle;
        }
    }

    public enum Speed
    {
        Idle = 0,
        Praying = 2,
        Walking = 4,
        Running = 8
    }

    public enum Jump
    {
        Idle = 3,
        Running = 6
    }
}
