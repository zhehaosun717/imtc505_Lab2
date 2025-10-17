using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// VR Button - Can be pushed by balls using physics
/// </summary>
public class VRButton : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private float pushDistance = 0.1f; // Push distance
    [SerializeField] private float returnSpeed = 5f; // Return speed
    [SerializeField] private float triggerThreshold = 0.08f; // Trigger threshold (how much push is needed)
    
    [Header("Visual Feedback")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color pressedColor = Color.red;
    [SerializeField] private Renderer buttonRenderer;
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip pressSound;
    private AudioSource audioSource;
    
    [Header("Events")]
    public UnityEvent onButtonPressed; // Triggered when button is pressed
    
    private Vector3 initialPosition;
    private bool isPressed = false;
    private float currentPushAmount = 0f;
    private Material buttonMaterial;

    private void Awake()
    {
        // Record initial position
        initialPosition = transform.localPosition;
        
        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && pressSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Setup material
        if (buttonRenderer == null)
        {
            buttonRenderer = GetComponent<Renderer>();
        }
        
        if (buttonRenderer != null)
        {
            buttonMaterial = buttonRenderer.material;
            buttonMaterial.color = normalColor;
        }
        
        // Ensure Rigidbody component exists (for physics interaction)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        // Set Rigidbody properties: fixed position but allows collision detection
        rb.isKinematic = false; // Set to dynamic so it can detect collisions
        rb.useGravity = false;  // Not affected by gravity
        rb.mass = 100f;         // Large mass to prevent being pushed away
        
        // Freeze all position and rotation (completely fixed, only controlled by script animation)
        rb.constraints = RigidbodyConstraints.FreezeAll;
        
        // Ensure collider exists
        if (GetComponent<Collider>() == null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.2f, 0.1f, 0.2f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if it's a ball collision (case-insensitive or check for VRGrabbable component)
        string objectName = collision.gameObject.name.ToLower();
        bool isBall = objectName.Contains("ball") || 
                      collision.gameObject.GetComponent<VRGrabbable>() != null;
        
        if (isBall)
        {
            // Calculate impact force
            float impactForce = collision.relativeVelocity.magnitude;
            
            if (impactForce > 0.2f)
            {
                PushButton(impactForce);
            }
        }
    }

    private void Update()
    {
        // Button auto-return
        if (currentPushAmount > 0)
        {
            currentPushAmount -= returnSpeed * Time.deltaTime;
            currentPushAmount = Mathf.Max(0, currentPushAmount);
            
            // Update position
            Vector3 targetPos = initialPosition - transform.forward * (currentPushAmount * pushDistance);
            transform.localPosition = targetPos;
            
            // Update color
            if (buttonMaterial != null)
            {
                float t = currentPushAmount;
                buttonMaterial.color = Color.Lerp(normalColor, pressedColor, t);
            }
            
            // Check if return is complete
            if (currentPushAmount <= 0 && isPressed)
            {
                isPressed = false;
            }
        }
    }

    private void PushButton(float force)
    {
        // Calculate push amount (based on force)
        float pushAmount = Mathf.Min(force * 0.3f, 1f);
        currentPushAmount = Mathf.Max(currentPushAmount, pushAmount);
        
        // Check if trigger threshold is reached and not already pressed
        if (currentPushAmount >= triggerThreshold && !isPressed)
        {
            isPressed = true;
            TriggerButton();
        }
    }

    private void TriggerButton()
    {
        // Play sound effect
        if (audioSource != null && pressSound != null)
        {
            audioSource.PlayOneShot(pressSound);
        }
        
        // Trigger event (notify score system)
        onButtonPressed?.Invoke();
        
        // Additional visual feedback (particle effects can be added here)
    }

    /// <summary>
    /// Manually trigger button (for testing)
    /// </summary>
    public void ManualTrigger()
    {
        if (!isPressed)
        {
            currentPushAmount = 1f;
            TriggerButton();
        }
    }

    /// <summary>
    /// Reset button
    /// </summary>
    public void ResetButton()
    {
        currentPushAmount = 0;
        isPressed = false;
        transform.localPosition = initialPosition;
        if (buttonMaterial != null)
        {
            buttonMaterial.color = normalColor;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Show button push range in editor
        Gizmos.color = Color.yellow;
        Vector3 pos = Application.isPlaying ? initialPosition : transform.localPosition;
        Gizmos.DrawLine(transform.position, transform.position - transform.forward * pushDistance);
    }
}
