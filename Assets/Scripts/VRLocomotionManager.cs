using UnityEngine;

/// <summary>
/// VR Locomotion Manager - Unified management of movement and rotation functions
/// </summary>
public class VRLocomotionManager : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private VRContinuousMove continuousMove;
    [SerializeField] private VRSnapTurn snapTurn;
    
    [Header("Settings")]
    [SerializeField] private bool enableMoveOnStart = true;
    [SerializeField] private bool enableTurnOnStart = true;

    private void Awake()
    {
        // Automatically find components
        if (continuousMove == null)
        {
            continuousMove = GetComponent<VRContinuousMove>();
        }
        
        if (snapTurn == null)
        {
            snapTurn = GetComponent<VRSnapTurn>();
        }
    }

    private void Start()
    {
        // Set initial state
        if (continuousMove != null)
        {
            continuousMove.enabled = enableMoveOnStart;
        }
        
        if (snapTurn != null)
        {
            snapTurn.enabled = enableTurnOnStart;
        }
    }

    /// <summary>
    /// Enable/Disable movement function
    /// </summary>
    public void SetMoveEnabled(bool enabled)
    {
        if (continuousMove != null)
        {
            continuousMove.enabled = enabled;
        }
    }

    /// <summary>
    /// Enable/Disable rotation function
    /// </summary>
    public void SetTurnEnabled(bool enabled)
    {
        if (snapTurn != null)
        {
            snapTurn.enabled = enabled;
        }
    }

    /// <summary>
    /// Set movement speed
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        if (continuousMove != null)
        {
            continuousMove.SetMoveSpeed(speed);
        }
    }

    /// <summary>
    /// Toggle rotation mode
    /// </summary>
    public void ToggleTurnMode()
    {
        if (snapTurn != null)
        {
            snapTurn.ToggleTurnMode();
        }
    }
}
