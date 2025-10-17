using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

/// <summary>
/// Score System Setup Wizard - One-click setup of complete scoring system
/// </summary>
public class ScoreSystemSetupWizard : EditorWindow
{
    private bool createScoreManager = true;
    private bool createUI = true;
    private bool createButton = true;
    private bool enableCombo = false;
    
    private Vector3 canvasPosition = new Vector3(0, 2, 3);
    private Vector3 canvasScale = new Vector3(0.01f, 0.01f, 0.01f);
    private Vector3 buttonPosition = new Vector3(0, 1.5f, 3);
    
    [MenuItem("Tools/VR Score System Setup Wizard")]
    public static void ShowWindow()
    {
        ScoreSystemSetupWizard window = GetWindow<ScoreSystemSetupWizard>("VR Score System Setup");
        window.minSize = new Vector2(400, 500);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("VR Score System Quick Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox("This wizard will help you quickly create a complete VR scoring system, including:\n" +
            "• Score Manager (ScoreManager)\n" +
            "• VR UI Display (Canvas + ScoreUI)\n" +
            "• Button pushable by balls (VRButton)\n" +
            "• Automatically connect all components", MessageType.Info);

        GUILayout.Space(10);

        // Options
        createScoreManager = EditorGUILayout.Toggle("Create Score Manager", createScoreManager);
        createUI = EditorGUILayout.Toggle("Create UI Display", createUI);
        createButton = EditorGUILayout.Toggle("Create Button", createButton);
        
        GUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Space(10);
        
        // Advanced options
        GUILayout.Label("Advanced Options", EditorStyles.boldLabel);
        enableCombo = EditorGUILayout.Toggle("Enable Combo System", enableCombo);
        
        if (createUI)
        {
            GUILayout.Space(5);
            GUILayout.Label("UI Canvas Position", EditorStyles.miniBoldLabel);
            canvasPosition = EditorGUILayout.Vector3Field("Position", canvasPosition);
            canvasScale = EditorGUILayout.Vector3Field("Scale", canvasScale);
        }
        
        if (createButton)
        {
            GUILayout.Space(5);
            GUILayout.Label("Button Position", EditorStyles.miniBoldLabel);
            buttonPosition = EditorGUILayout.Vector3Field("Position", buttonPosition);
        }

        GUILayout.Space(20);

        // Start button
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Start Setup", GUILayout.Height(40)))
        {
            SetupScoreSystem();
        }
        GUI.backgroundColor = Color.white;
    }

    private void SetupScoreSystem()
    {
        try
        {
            GameObject scoreManagerObj = null;
            GameObject canvasObj = null;
            GameObject buttonObj = null;

            // 1. Create score manager
            if (createScoreManager)
            {
                scoreManagerObj = SetupScoreManager();
            }
            else
            {
                // Find existing
                scoreManagerObj = GameObject.Find("ScoreManager");
                if (scoreManagerObj == null)
                {
                    ScoreManager existing = FindObjectOfType<ScoreManager>();
                    if (existing != null)
                        scoreManagerObj = existing.gameObject;
                }
            }

            // 2. Create UI
            if (createUI)
            {
                canvasObj = SetupUICanvas();
            }

            // 3. Create button
            if (createButton)
            {
                buttonObj = SetupButton(scoreManagerObj);
            }

            // Complete message
            string message = "VR Score System setup complete!\n\n";
            if (scoreManagerObj != null) message += "• ScoreManager created\n";
            if (canvasObj != null) message += "• UI Canvas created\n";
            if (buttonObj != null) message += "• Button created and connected\n";
            message += "\nYou can now test the game!\nShoot balls to hit the button, score will display on UI.";

            EditorUtility.DisplayDialog("Setup Complete", message, "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Error during setup:\n{e.Message}", "OK");
        }
    }

    private GameObject SetupScoreManager()
    {
        GameObject obj = GameObject.Find("ScoreManager");
        if (obj == null)
        {
            obj = new GameObject("ScoreManager");
        }

        ScoreManager manager = obj.GetComponent<ScoreManager>();
        if (manager == null)
        {
            manager = obj.AddComponent<ScoreManager>();
        }

        // Use SerializedObject to set private fields
        SerializedObject so = new SerializedObject(manager);
        so.FindProperty("scorePerHit").intValue = 1;
        so.FindProperty("maxScore").intValue = 9999;
        so.FindProperty("enableCombo").boolValue = enableCombo;
        so.FindProperty("comboTimeWindow").floatValue = 2f;
        so.FindProperty("comboMultiplier").intValue = 2;
        so.ApplyModifiedProperties();

        return obj;
    }

    private GameObject SetupUICanvas()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("ScoreCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;
        
        canvasObj.AddComponent<GraphicRaycaster>();

        // Set transform
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.position = canvasPosition;
        canvasRect.localScale = canvasScale;
        canvasRect.sizeDelta = new Vector2(400, 200);

        // Create ScoreText
        GameObject textObj = new GameObject("ScoreText");
        textObj.transform.SetParent(canvasObj.transform);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "Score: 0";
        text.fontSize = 48;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(400, 100);
        textRect.anchoredPosition = Vector2.zero;

        // Add ScoreUI component
        ScoreUI scoreUI = canvasObj.AddComponent<ScoreUI>();
        SerializedObject so = new SerializedObject(scoreUI);
        so.FindProperty("scoreText").objectReferenceValue = text;
        so.FindProperty("scorePrefix").stringValue = "Score: ";
        so.FindProperty("enableScaleAnimation").boolValue = true;
        so.ApplyModifiedProperties();

        return canvasObj;
    }

    private GameObject SetupButton(GameObject scoreManager)
    {
        // Create button
        GameObject buttonObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        buttonObj.name = "Button";
        buttonObj.transform.position = buttonPosition;
        buttonObj.transform.localScale = new Vector3(0.3f, 0.1f, 0.3f);

        // Create material
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.green;
        buttonObj.GetComponent<Renderer>().material = mat;

        // Add VRButton component
        VRButton button = buttonObj.AddComponent<VRButton>();
        
        SerializedObject so = new SerializedObject(button);
        so.FindProperty("pushDistance").floatValue = 0.1f;
        so.FindProperty("returnSpeed").floatValue = 5f;
        so.FindProperty("triggerThreshold").floatValue = 0.08f;
        so.FindProperty("normalColor").colorValue = Color.green;
        so.FindProperty("pressedColor").colorValue = Color.red;
        so.FindProperty("buttonRenderer").objectReferenceValue = buttonObj.GetComponent<Renderer>();
        so.ApplyModifiedProperties();

        // Connect to ScoreManager
        if (scoreManager != null)
        {
            ScoreManager manager = scoreManager.GetComponent<ScoreManager>();
            if (manager != null)
            {
                // Add event listener
                UnityEditor.Events.UnityEventTools.AddPersistentListener(
                    button.onButtonPressed,
                    manager.AddScore
                );
                EditorUtility.SetDirty(button);
            }
        }

        return buttonObj;
    }
}
