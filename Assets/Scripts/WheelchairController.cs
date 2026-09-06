using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Windows;

public class WheelchairController : MonoBehaviour
{
    [Header("Wheelchair Settings")]
    public float acceleration = 2000f;
    public float deceleration = 500f;
    public float maxSpeed = 20f;
    public float steeringRange = 360;
    public float steeringRangeAtMaxSpeed = 240f;
    public float centerOfGravityOffset = -1f;

    private WheelControl[] wheels;
    private Rigidbody rb;

    private WheelchairInputActions controls;
    private Vector2 _inputVector;
    private float moveInput;
    private float steerInput;
    private float currentSpeed;
    private float currentSteerRange;
    private float currentAcceleration;

    private void Awake()
    {
        controls = new WheelchairInputActions();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        Vector3 centerOfMass = rb.centerOfMass;
        centerOfMass.y += centerOfGravityOffset;
        rb.centerOfMass = centerOfMass;

        wheels = GetComponentsInChildren<WheelControl>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _inputVector = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        bool isAccelerating = Mathf.Sign(moveInput) == Mathf.Sign(currentSpeed);

        foreach (var wheel in wheels)
        {
            if (wheel.steerable)
            {
                wheel.wheelCollider.steerAngle = steerInput * currentSteerRange;
            }

            if (isAccelerating)
            {
                if (wheel.motorized)
                {
                    wheel.wheelCollider.motorTorque = moveInput * currentSpeed;
                }

                wheel.wheelCollider.brakeTorque = 0f;
            }
            else
            {
                wheel.wheelCollider.motorTorque = 0f;
                //wheel.wheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
            }
        }
    }

    public void Movement()
    {
        moveInput = _inputVector.y;

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            currentSpeed += moveInput * acceleration * Time.deltaTime;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
        }

        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed * 0.5f, maxSpeed);

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    public void Steering()
    {
        steerInput = _inputVector.x;
    }

    /*private void FixedUpdate()
    {
        float vInput = _inputVector.y;
        float hInput = _inputVector.x;

        
        float forwardSpeed = Vector3.Dot(transform.forward, rb.linearVelocity);
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(forwardSpeed));

        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        foreach (var wheel in wheels)
        {
            if (wheel.steerable)
            {
                wheel.wheelCollider.steerAngle = hInput * currentSteerRange;
            }

            if (isAccelerating)
            {
                if (wheel.motorized)
                {
                    wheel.wheelCollider.motorTorque = vInput * currentMotorTorque;
                }

                wheel.wheelCollider.brakeTorque = 0f;
            }
            else
            {
                wheel.wheelCollider.motorTorque = 0f;
                wheel.wheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
            }
        }
    }*/
}
