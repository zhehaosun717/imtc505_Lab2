using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// VR Snap Turn Controller - Use right joystick to rotate view
/// Suitable for Meta Quest 3
/// </summary>
public class VRSnapTurn : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float turnAngle = 45f; // Rotation angle per turn
    [SerializeField] private bool useContinuousTurn = false; // Use continuous rotation
    [SerializeField] private float continuousTurnSpeed = 60f; // Continuous rotation speed
    
    [Header("Input Settings")]
    [SerializeField] private InputActionProperty turnInputAction;
    
    [Header("Reference Objects")]
    [SerializeField] private Transform xrOrigin;
    
    private bool canTurn = true;
    private float turnCooldown = 0.3f; // Snap turn cooldown time
    private float lastTurnTime;

    private void Awake()
    {
        // Automatically find XR Origin
        if (xrOrigin == null)
        {
            GameObject xrOriginObj = GameObject.Find("XR Origin") ?? GameObject.Find("XROrigin");
            if (xrOriginObj != null)
            {
                xrOrigin = xrOriginObj.transform;
            }
        }
    }

    private void OnEnable()
    {
        turnInputAction.action?.Enable();
    }

    private void OnDisable()
    {
        turnInputAction.action?.Disable();
    }

    private void Update()
    {
        if (xrOrigin == null)
            return;

        Vector2 turnInput = turnInputAction.action?.ReadValue<Vector2>() ?? Vector2.zero;

        if (useContinuousTurn)
        {
            // Continuous rotation mode
            PerformContinuousTurn(turnInput.x);
        }
        else
        {
            // Snap turn mode (recommended, reduces motion sickness)
            PerformSnapTurn(turnInput.x);
        }
    }

    private void PerformSnapTurn(float input)
    {
        // Check cooldown time
        if (Time.time - lastTurnTime < turnCooldown)
            return;

        // Check input threshold
        if (Mathf.Abs(input) < 0.5f)
        {
            canTurn = true;
            return;
        }

        if (!canTurn)
            return;

        // Execute rotation
        float angle = input > 0 ? turnAngle : -turnAngle;
        xrOrigin.Rotate(0, angle, 0);

        // Set cooldown
        canTurn = false;
        lastTurnTime = Time.time;
    }

    private void PerformContinuousTurn(float input)
    {
        if (Mathf.Abs(input) < 0.1f)
            return;

        float rotation = input * continuousTurnSpeed * Time.deltaTime;
        xrOrigin.Rotate(0, rotation, 0);
    }

    /// <summary>
    /// Toggle rotation mode
    /// </summary>
    public void ToggleTurnMode()
    {
        useContinuousTurn = !useContinuousTurn;
    }

    /// <summary>
    /// Set rotation angle
    /// </summary>
    public void SetTurnAngle(float angle)
    {
        turnAngle = angle;
    }

    /// <summary>
    /// Set continuous rotation speed
    /// </summary>
    public void SetContinuousTurnSpeed(float speed)
    {
        continuousTurnSpeed = speed;
    }
}
