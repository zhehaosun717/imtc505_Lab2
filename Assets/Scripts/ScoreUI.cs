using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// VR Score UI Display - Display score on Canvas
/// </summary>
public class ScoreUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText; // Use TextMeshPro (recommended)
    [SerializeField] private Text legacyScoreText; // Or use legacy Text component
    
    [Header("Display Settings")]
    [SerializeField] private string scorePrefix = "Score: ";
    [SerializeField] private string scoreSuffix = "";
    [SerializeField] private bool useThousandsSeparator = false; // Use thousands separator
    
    [Header("Combo Display (Optional)")]
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private Text legacyComboText;
    [SerializeField] private GameObject comboPanel; // Combo panel (shown when combo > 1)
    
    [Header("Animation Effects (Optional)")]
    [SerializeField] private bool enableScaleAnimation = true;
    [SerializeField] private float scaleDuration = 0.3f;
    [SerializeField] private float scaleMultiplier = 1.2f;
    
    private int currentDisplayScore = 0;
    private Vector3 originalScale;
    private float scaleTimer = 0f;

    private void Start()
    {
        // Record original scale
        if (scoreText != null)
            originalScale = scoreText.transform.localScale;
        else if (legacyScoreText != null)
            originalScale = legacyScoreText.transform.localScale;
        
        // Subscribe to score manager events
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.onScoreChanged.AddListener(UpdateScoreDisplay);
            ScoreManager.Instance.onComboChanged.AddListener(UpdateComboDisplay);
            
            // Initialize display
            UpdateScoreDisplay(ScoreManager.Instance.GetScore());
            UpdateComboDisplay(ScoreManager.Instance.GetCombo());
        }
        
        // Initialize combo panel
        if (comboPanel != null)
        {
            comboPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // Handle scale animation
        if (enableScaleAnimation && scaleTimer > 0)
        {
            scaleTimer -= Time.deltaTime;
            float progress = 1f - (scaleTimer / scaleDuration);
            float scale = Mathf.Lerp(scaleMultiplier, 1f, progress);
            
            if (scoreText != null)
                scoreText.transform.localScale = originalScale * scale;
            else if (legacyScoreText != null)
                legacyScoreText.transform.localScale = originalScale * scale;
            
            if (scaleTimer <= 0)
            {
                // Restore original size
                if (scoreText != null)
                    scoreText.transform.localScale = originalScale;
                else if (legacyScoreText != null)
                    legacyScoreText.transform.localScale = originalScale;
            }
        }
    }

    /// <summary>
    /// Update score display
    /// </summary>
    private void UpdateScoreDisplay(int newScore)
    {
        currentDisplayScore = newScore;
        
        // Format score string
        string scoreString;
        if (useThousandsSeparator)
        {
            scoreString = newScore.ToString("N0"); // Thousands separator
        }
        else
        {
            scoreString = newScore.ToString();
        }
        
        string fullText = scorePrefix + scoreString + scoreSuffix;
        
        // Update text
        if (scoreText != null)
        {
            scoreText.text = fullText;
        }
        else if (legacyScoreText != null)
        {
            legacyScoreText.text = fullText;
        }
        
        // Trigger scale animation
        if (enableScaleAnimation)
        {
            scaleTimer = scaleDuration;
        }
    }

    /// <summary>
    /// Update combo display
    /// </summary>
    private void UpdateComboDisplay(int combo)
    {
        // Show/hide combo panel
        if (comboPanel != null)
        {
            comboPanel.SetActive(combo > 1);
        }
        
        // Update combo text
        string comboString = combo > 1 ? $"Combo x{combo}" : "";
        
        if (comboText != null)
        {
            comboText.text = comboString;
        }
        else if (legacyComboText != null)
        {
            legacyComboText.text = comboString;
        }
    }

    /// <summary>
    /// Manually set score display (no animation)
    /// </summary>
    public void SetScoreDisplay(int score)
    {
        currentDisplayScore = score;
        string fullText = scorePrefix + score.ToString() + scoreSuffix;
        
        if (scoreText != null)
            scoreText.text = fullText;
        else if (legacyScoreText != null)
            legacyScoreText.text = fullText;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.onScoreChanged.RemoveListener(UpdateScoreDisplay);
            ScoreManager.Instance.onComboChanged.RemoveListener(UpdateComboDisplay);
        }
    }
}
