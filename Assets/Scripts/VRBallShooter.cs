using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// VR Ball Shooter - Shoot randomly colored balls with right trigger
/// </summary>
public class VRBallShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private float shootForce = 10f;
    [SerializeField] private Transform shootPoint; // Shoot point (right hand position)
    
    [Header("Ball Settings")]
    [SerializeField] private float ballSize = 0.1f;
    [SerializeField] private float ballLifetime = 20f; // Ball lifetime (seconds)
    
    [Header("Input Settings")]
    [SerializeField] private InputActionProperty shootAction;
    
    [Header("Color Settings")]
    [SerializeField] private Color[] ballColors;
    
    [Header("Audio Settings (Optional)")]
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private float soundVolume = 1.0f;
    
    private AudioSource audioSource;

    private void Awake()
    {
        // Force initialize color array with blue gradient colors (override Inspector values)
        ballColors = new Color[]
        {
            HexToColor("#002145"), // Dark Blue
            HexToColor("#0055B7"), // Blue
            HexToColor("#0077C8"), // Medium Blue
            HexToColor("#00A7E1"), // Bright Blue
            HexToColor("#40B4E5"), // Sky Blue
            HexToColor("#6EC4E8"), // Light Sky Blue
            HexToColor("#A4D2E6"), // Light Blue
            HexToColor("#FFFFFF")  // White
        };
        
        // If shoot point is not set, use current object position
        if (shootPoint == null)
        {
            shootPoint = transform;
        }
        
        // If no ball prefab is set, create one at runtime
        if (ballPrefab == null)
        {
            CreateBallPrefab();
        }
        
        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && shootSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1.0f; // 3D sound
        }
    }

    private void OnEnable()
    {
        shootAction.action?.Enable();
        if (shootAction.action != null)
        {
            shootAction.action.performed += OnShoot;
        }
    }

    private void OnDisable()
    {
        if (shootAction.action != null)
        {
            shootAction.action.performed -= OnShoot;
        }
        shootAction.action?.Disable();
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        ShootBall();
    }

    private void ShootBall()
    {
        if (ballPrefab == null)
        {
            return;
        }
        
        // Play shoot sound
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound, soundVolume);
        }

        // Create ball
        GameObject ball = Instantiate(ballPrefab, shootPoint.position, Quaternion.identity);
        
        // Set random color (choose from blue gradient)
        int colorIndex = Random.Range(0, ballColors.Length);
        Color randomColor = ballColors[colorIndex];
        Renderer renderer = ball.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = randomColor;
        }
        
        // Add shooting force
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 shootDirection = shootPoint.forward;
            rb.linearVelocity = shootDirection * shootForce;
        }
        
        // Set lifetime
        Destroy(ball, ballLifetime);
    }

    private void CreateBallPrefab()
    {
        // Create a default ball prefab
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "Ball";
        ball.transform.localScale = Vector3.one * ballSize;
        
        // Add Rigidbody
        Rigidbody rb = ball.AddComponent<Rigidbody>();
        rb.mass = 0.1f;
        rb.useGravity = true;
        
        // Add grabbable component
        ball.AddComponent<VRGrabbable>();
        
        // Create material
        Material mat = new Material(Shader.Find("Standard"));
        ball.GetComponent<Renderer>().material = mat;
        
        ballPrefab = ball;
        
        // Hide original ball (use as template)
        ball.SetActive(false);
    }

    /// <summary>
    /// Set shooting force
    /// </summary>
    public void SetShootForce(float force)
    {
        shootForce = force;
    }

    /// <summary>
    /// Set ball size
    /// </summary>
    public void SetBallSize(float size)
    {
        ballSize = size;
        if (ballPrefab != null)
        {
            ballPrefab.transform.localScale = Vector3.one * size;
        }
    }

    /// <summary>
    /// Convert HEX color code to Unity Color
    /// </summary>
    private Color HexToColor(string hex)
    {
        // Remove # symbol
        hex = hex.Replace("#", "");
        
        // Parse RGB values
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        
        // Convert to 0-1 range float values
        return new Color(r / 255f, g / 255f, b / 255f);
    }
}
