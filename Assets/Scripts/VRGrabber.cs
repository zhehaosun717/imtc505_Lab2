using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// VR Grabber Controller - Grab objects with left trigger
/// </summary>
public class VRGrabber : MonoBehaviour
{
    [Header("Grab Settings")]
    [SerializeField] private float grabRadius = 0.3f; // Grab radius
    [SerializeField] private LayerMask grabbableLayer = -1; // Layer of grabbable objects
    [SerializeField] private Transform handTransform; // Hand Transform
    
    [Header("Input Settings")]
    [SerializeField] private InputActionProperty grabAction;
    
    [Header("Throw Settings")]
    [SerializeField] private float throwMultiplier = 1.5f; // Throw force multiplier
    
    private VRGrabbable currentGrabbedObject;
    private Vector3 lastHandPosition;
    private List<Vector3> velocitySamples = new List<Vector3>();
    private int maxVelocitySamples = 5;

    private void Awake()
    {
        if (handTransform == null)
        {
            handTransform = transform;
        }
    }

    private void OnEnable()
    {
        grabAction.action?.Enable();
        if (grabAction.action != null)
        {
            grabAction.action.started += OnGrabStarted;
            grabAction.action.canceled += OnGrabCanceled;
        }
    }

    private void OnDisable()
    {
        if (grabAction.action != null)
        {
            grabAction.action.started -= OnGrabStarted;
            grabAction.action.canceled -= OnGrabCanceled;
        }
        grabAction.action?.Disable();
    }

    private void Update()
    {
        // Record hand velocity (for throwing objects)
        if (currentGrabbedObject != null)
        {
            Vector3 velocity = (handTransform.position - lastHandPosition) / Time.deltaTime;
            velocitySamples.Add(velocity);
            
            if (velocitySamples.Count > maxVelocitySamples)
            {
                velocitySamples.RemoveAt(0);
            }
        }
        
        lastHandPosition = handTransform.position;
    }

    private void OnGrabStarted(InputAction.CallbackContext context)
    {
        if (currentGrabbedObject == null)
        {
            TryGrab();
        }
    }

    private void OnGrabCanceled(InputAction.CallbackContext context)
    {
        if (currentGrabbedObject != null)
        {
            ReleaseObject();
        }
    }

    private void TryGrab()
    {
        // Find grabbable objects near the hand
        Collider[] colliders = Physics.OverlapSphere(handTransform.position, grabRadius, grabbableLayer);
        
        VRGrabbable closestGrabbable = null;
        float closestDistance = float.MaxValue;
        
        foreach (Collider col in colliders)
        {
            VRGrabbable grabbable = col.GetComponent<VRGrabbable>();
            
            if (grabbable != null && grabbable.CanGrab(handTransform.position))
            {
                float distance = Vector3.Distance(handTransform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestGrabbable = grabbable;
                }
            }
        }
        
        if (closestGrabbable != null)
        {
            currentGrabbedObject = closestGrabbable;
            currentGrabbedObject.Grab(handTransform);
            velocitySamples.Clear();
        }
    }

    private void ReleaseObject()
    {
        if (currentGrabbedObject == null) return;
        
        // Calculate average throw velocity
        Vector3 throwVelocity = Vector3.zero;
        if (velocitySamples.Count > 0)
        {
            foreach (Vector3 vel in velocitySamples)
            {
                throwVelocity += vel;
            }
            throwVelocity /= velocitySamples.Count;
            throwVelocity *= throwMultiplier;
        }
        
        currentGrabbedObject.Release(throwVelocity);
        currentGrabbedObject = null;
        velocitySamples.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        // Show grab range
        if (handTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(handTransform.position, grabRadius);
        }
    }

    /// <summary>
    /// Set grab radius
    /// </summary>
    public void SetGrabRadius(float radius)
    {
        grabRadius = radius;
    }

    /// <summary>
    /// Get currently grabbed object
    /// </summary>
    public VRGrabbable GetGrabbedObject()
    {
        return currentGrabbedObject;
    }
}
