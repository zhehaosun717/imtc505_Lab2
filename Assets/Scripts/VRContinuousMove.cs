using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// VR Continuous Movement Controller - Use joystick to control movement
/// Suitable for Meta Quest 3
/// </summary>
public class VRContinuousMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private bool enableSprint = true;
    
    [Header("Input Settings")]
    [SerializeField] private InputActionProperty moveInputAction;
    [SerializeField] private InputActionProperty sprintInputAction;
    
    [Header("Reference Objects")]
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private Transform headTransform;
    
    [Header("Advanced Settings")]
    [SerializeField] private bool useHeadDirection = true; // Move based on head direction
    [SerializeField] private bool useControllerDirection = false; // Move based on controller direction
    [SerializeField] private Transform controllerTransform; // If using controller direction
    
    private CharacterController characterController;
    private bool isSprinting = false;

    private void Awake()
    {
        // If XR Origin is not set, try to find it automatically
        if (xrOrigin == null)
        {
            xrOrigin = transform;
        }
        
        // Get or add CharacterController
        characterController = xrOrigin.GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = xrOrigin.gameObject.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.4f;
            characterController.center = new Vector3(0, 0.9f, 0);
        }
    }

    private void OnEnable()
    {
        moveInputAction.action?.Enable();
        if (enableSprint && sprintInputAction.action != null)
        {
            sprintInputAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        moveInputAction.action?.Disable();
        sprintInputAction.action?.Disable();
    }

    private void Update()
    {
        PerformMove();
    }

    private void PerformMove()
    {
        // Read movement input
        Vector2 inputMove = moveInputAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
        
        if (inputMove.magnitude < 0.1f)
            return;

        // Check if sprinting
        if (enableSprint && sprintInputAction.action != null)
        {
            isSprinting = sprintInputAction.action.ReadValue<float>() > 0.5f;
        }

        // Calculate movement speed
        float currentSpeed = moveSpeed;
        if (isSprinting)
        {
            currentSpeed *= sprintMultiplier;
        }

        // Determine movement direction
        Vector3 moveDirection = GetMoveDirection(inputMove);
        
        // Apply movement
        Vector3 movement = moveDirection * currentSpeed * Time.deltaTime;
        movement.y = 0; // Ensure no vertical movement
        
        characterController.Move(movement);
    }

    private Vector3 GetMoveDirection(Vector2 input)
    {
        Vector3 forward;
        Vector3 right;

        if (useHeadDirection && headTransform != null)
        {
            // Based on head direction
            forward = headTransform.forward;
            right = headTransform.right;
        }
        else if (useControllerDirection && controllerTransform != null)
        {
            // Based on controller direction
            forward = controllerTransform.forward;
            right = controllerTransform.right;
        }
        else
        {
            // Based on XR Origin direction
            forward = xrOrigin.forward;
            right = xrOrigin.right;
        }

        // Project onto horizontal plane
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Calculate final movement direction
        return forward * input.y + right * input.x;
    }

    // Public methods: Allow external modification of movement speed
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetSprintMultiplier(float multiplier)
    {
        sprintMultiplier = multiplier;
    }
}
