using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Score Manager - Singleton pattern to manage game score
/// </summary>
public class ScoreManager : MonoBehaviour
{
    // Singleton instance
    public static ScoreManager Instance { get; private set; }

    [Header("Score Settings")]
    [SerializeField] private int score = 0;
    [SerializeField] private int scorePerHit = 1; // Points per hit
    [SerializeField] private int maxScore = 9999; // Maximum score
    
    [Header("Combo System (Optional)")]
    [SerializeField] private bool enableCombo = false;
    [SerializeField] private float comboTimeWindow = 2f; // Combo time window (seconds)
    [SerializeField] private int comboMultiplier = 2; // Combo multiplier
    
    [Header("Events")]
    public UnityEvent<int> onScoreChanged; // Triggered when score changes (passes new score)
    public UnityEvent<int> onComboChanged; // Triggered when combo changes
    
    private int currentCombo = 0;
    private float lastHitTime = 0f;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        // Check if combo timeout
        if (enableCombo && currentCombo > 0)
        {
            if (Time.time - lastHitTime > comboTimeWindow)
            {
                ResetCombo();
            }
        }
    }

    /// <summary>
    /// Add score (called by button)
    /// </summary>
    public void AddScore()
    {
        int pointsToAdd = scorePerHit;
        
        // Handle combo system
        if (enableCombo)
        {
            float timeSinceLastHit = Time.time - lastHitTime;
            
            if (timeSinceLastHit <= comboTimeWindow)
            {
                // Within combo time window
                currentCombo++;
                pointsToAdd *= comboMultiplier;
                onComboChanged?.Invoke(currentCombo);
            }
            else
            {
                // Combo broken
                currentCombo = 1;
                onComboChanged?.Invoke(currentCombo);
            }
            
            lastHitTime = Time.time;
        }
        
        // Add score
        score += pointsToAdd;
        score = Mathf.Min(score, maxScore); // Limit maximum score
        
        // Trigger event to notify UI update
        onScoreChanged?.Invoke(score);
    }

    /// <summary>
    /// Remove score
    /// </summary>
    public void RemoveScore(int amount)
    {
        score -= amount;
        score = Mathf.Max(0, score); // Cannot be less than 0
        
        onScoreChanged?.Invoke(score);
    }

    /// <summary>
    /// Reset score
    /// </summary>
    public void ResetScore()
    {
        score = 0;
        currentCombo = 0;
        onScoreChanged?.Invoke(score);
        onComboChanged?.Invoke(currentCombo);
    }

    /// <summary>
    /// Get current score
    /// </summary>
    public int GetScore()
    {
        return score;
    }

    /// <summary>
    /// Get current combo
    /// </summary>
    public int GetCombo()
    {
        return currentCombo;
    }

    /// <summary>
    /// Reset combo
    /// </summary>
    private void ResetCombo()
    {
        if (currentCombo > 0)
        {
            currentCombo = 0;
            onComboChanged?.Invoke(currentCombo);
        }
    }

    /// <summary>
    /// Set points per hit
    /// </summary>
    public void SetScorePerHit(int value)
    {
        scorePerHit = value;
    }
}
