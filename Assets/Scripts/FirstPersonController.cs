using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float moveSpeed = 4.5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float mouseSensitivity = 0.12f;
    [SerializeField] private float gravity = -18f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float pitchMin = -80f;
    [SerializeField] private float pitchMax = 80f;

    private CharacterController controller;
    private float verticalVelocity;
    private float pitch;
    private bool skipNextLook = true;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pitch = cameraTransform.localEulerAngles.x;
    }

    private void Update()
    {
        if (skipNextLook)
        {
            skipNextLook = false;
            if (Mouse.current != null)
            {
                Mouse.current.delta.ReadValue();
            }
        }
        else
        {
            HandleLook();
        }

        HandleMovement();

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
            skipNextLook = true;
        }
    }

    private void HandleLook()
    {
        if (Mouse.current == null)
        {
            return;
        }

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        float horizontal = 0f;
        float vertical = 0f;
        bool sprintHeld = false;
        bool jumpPressed = false;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
            sprintHeld = Keyboard.current.leftShiftKey.isPressed;
            jumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
        }

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        float currentSpeed = sprintHeld ? sprintSpeed : moveSpeed;

        if (isGrounded && jumpPressed)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 motion = moveDirection * currentSpeed + Vector3.up * verticalVelocity;
        controller.Move(motion * Time.deltaTime);
    }
}
