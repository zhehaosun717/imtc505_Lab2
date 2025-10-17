using UnityEngine;

/// <summary>
/// VR Grabbable Object - Can be picked up with left trigger
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class VRGrabbable : MonoBehaviour
{
    [Header("Grab Settings")]
    [SerializeField] private float grabDistance = 0.5f; // Grab distance
    [SerializeField] private bool usePhysics = true; // Use physics for grabbing
    
    private Rigidbody rb;
    private bool isGrabbed = false;
    private Transform grabber; // The hand that grabbed it
    private Vector3 grabOffset;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Get grabbed
    /// </summary>
    public void Grab(Transform grabberTransform)
    {
        if (isGrabbed) return;
        
        isGrabbed = true;
        grabber = grabberTransform;
        
        // Calculate grab offset
        grabOffset = transform.position - grabber.position;
        
        if (usePhysics)
        {
            // Disable gravity but keep physics
            rb.useGravity = false;
            rb.linearDamping = 10f; // Increase drag for more stable following
        }
        else
        {
            // Completely disable physics
            rb.isKinematic = true;
        }
    }

    /// <summary>
    /// Get released
    /// </summary>
    public void Release(Vector3 throwVelocity)
    {
        if (!isGrabbed) return;
        
        isGrabbed = false;
        grabber = null;
        
        // Restore physics
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearDamping = 0.5f;
        
        // Apply throw velocity
        rb.linearVelocity = throwVelocity;
    }

    private void FixedUpdate()
    {
        if (isGrabbed && grabber != null)
        {
            // Make object follow hand position
            Vector3 targetPosition = grabber.position + grabOffset;
            
            if (usePhysics)
            {
                // Move using physics
                Vector3 direction = targetPosition - transform.position;
                rb.linearVelocity = direction * 10f; // Use velocity to move it
            }
            else
            {
                // Directly set position
                transform.position = targetPosition;
            }
        }
    }

    /// <summary>
    /// Check if can be grabbed
    /// </summary>
    public bool CanGrab(Vector3 handPosition)
    {
        float distance = Vector3.Distance(transform.position, handPosition);
        return !isGrabbed && distance <= grabDistance;
    }

    /// <summary>
    /// Get whether it's grabbed
    /// </summary>
    public bool IsGrabbed()
    {
        return isGrabbed;
    }

    private void OnDrawGizmosSelected()
    {
        // Show grab range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabDistance);
    }
}
