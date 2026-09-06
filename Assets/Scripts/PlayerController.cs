using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //movement
    private float moveSpeed = 5f;
    private float gravity = -9.81f;

    //rotation
    private Transform cameraTarget;
    private float lookSensitivity = 0.1f;
    private float topClamp = 85f;
    private float bottomClamp = -85f;

    //inputs
    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private float _cinemachineTargetPitch;
    private float _verticalVelocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Movement();
    }

    private void LateUpdate()
    {
        Rotation();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    private void Movement()
    {
        Vector3 moveDirection = Vector3.forward * _moveInput.y + Vector3.right * _moveInput.x;

        _verticalVelocity += gravity * Time.deltaTime;
        moveDirection.y = _verticalVelocity;

        _controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void Rotation()
    {
        if (_lookInput.sqrMagnitude >= .01f)
        {
            _cinemachineTargetPitch -= _lookInput.y * lookSensitivity;
            _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, bottomClamp, topClamp);
            cameraTarget.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0f, 0f);

            transform.Rotate(Vector3.up * (_lookInput.x * lookSensitivity));
        }
    }
}
