using UnityEngine;
using UnityEngine.InputSystem;

public class WheelchairController : MonoBehaviour
{
    [Header("Wheelchair Settings")]
    public float motorTorque = 2000f;
    public float brakeTorque = 500f;
    public float maxSpeed = 20f;
    public float steeringRange = 90;
    public float steeringRangeAtMaxSpeed = 45;
    public float centerOfGravityOffset = -1f;

    private WheelControl[] wheels;
    private Rigidbody rb;

    private WheelchairInputActions controls;
    private Vector2 _inputVector;

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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
        
    }

    private void FixedUpdate()
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
    }
}
