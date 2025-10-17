using UnityEngine;
using TMPro;

/// <summary>
/// VR Locomotion Debugger
/// Display current movement state and input information in VR
/// </summary>
public class VRLocomotionDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private bool showInConsole = true;
    [SerializeField] private bool showInWorld = false;
    
    [Header("World Space UI")]
    [SerializeField] private Canvas debugCanvas;
    [SerializeField] private TextMeshProUGUI debugText;
    
    [Header("Component References")]
    [SerializeField] private VRContinuousMove continuousMove;
    [SerializeField] private VRSnapTurn snapTurn;
    [SerializeField] private VRLocomotionManager locomotionManager;
    
    private float updateInterval = 0.5f;
    private float nextUpdateTime;

    private void Start()
    {
        // Automatically find components
        if (continuousMove == null)
            continuousMove = FindObjectOfType<VRContinuousMove>();
        
        if (snapTurn == null)
            snapTurn = FindObjectOfType<VRSnapTurn>();
        
        if (locomotionManager == null)
            locomotionManager = FindObjectOfType<VRLocomotionManager>();
        
        // Create debug canvas
        if (showInWorld && debugCanvas == null)
        {
            CreateDebugCanvas();
        }
    }

    private void Update()
    {
        if (!showDebugInfo)
            return;
        
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;
            UpdateDebugInfo();
        }
    }

    private void UpdateDebugInfo()
    {
        string info = BuildDebugInfo();
        
        if (showInWorld && debugText != null)
        {
            debugText.text = info;
        }
    }

    private string BuildDebugInfo()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("=== VR Locomotion Debug Info ===");
        sb.AppendLine();
        
        // Continuous movement info
        if (continuousMove != null)
        {
            sb.AppendLine("Continuous Move:");
            sb.AppendLine($"  Status: {(continuousMove.enabled ? "Enabled" : "Disabled")}");
        }
        
        // Rotation info
        if (snapTurn != null)
        {
            sb.AppendLine("Snap Turn:");
            sb.AppendLine($"  Status: {(snapTurn.enabled ? "Enabled" : "Disabled")}");
        }
        
        // Position info
        sb.AppendLine($"Current Position: {transform.position}");
        
        return sb.ToString();
    }

    private void CreateDebugCanvas()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("DebugCanvas");
        canvasObj.transform.SetParent(Camera.main.transform);
        canvasObj.transform.localPosition = new Vector3(0, 0, 2f);
        canvasObj.transform.localRotation = Quaternion.identity;
        
        debugCanvas = canvasObj.AddComponent<Canvas>();
        debugCanvas.renderMode = RenderMode.WorldSpace;
        
        RectTransform canvasRect = debugCanvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(600, 400);
        canvasRect.localScale = Vector3.one * 0.001f;
        
        // Create Text
        GameObject textObj = new GameObject("DebugText");
        textObj.transform.SetParent(canvasObj.transform);
        
        debugText = textObj.AddComponent<TextMeshProUGUI>();
        debugText.fontSize = 24;
        debugText.color = Color.white;
        debugText.alignment = TextAlignmentOptions.TopLeft;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        // Add background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform);
        bgObj.transform.SetAsFirstSibling();
        
        UnityEngine.UI.Image bg = bgObj.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0, 0, 0, 0.7f);
        
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
    }

    /// <summary>
    /// Toggle debug info display
    /// </summary>
    public void ToggleDebugInfo()
    {
        showDebugInfo = !showDebugInfo;
        
        if (debugCanvas != null)
        {
            debugCanvas.gameObject.SetActive(showDebugInfo);
        }
    }

    /// <summary>
    /// Log position information
    /// </summary>
    public void LogPosition()
    {
        // Position logging functionality
    }
}
