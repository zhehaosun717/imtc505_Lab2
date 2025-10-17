using UnityEngine;
using UnityEditor;

/// <summary>
/// VR Locomotion Setup Wizard
/// Quickly configure VR movement system
/// </summary>
public class VRLocomotionSetupWizard : EditorWindow
{
    private GameObject xrOrigin;
    private GameObject leftController;
    private GameObject rightController;
    private GameObject cameraObject;
    
    private bool setupContinuousMove = true;
    private bool setupSnapTurn = true;
    
    [MenuItem("Tools/VR Locomotion Setup Wizard")]
    public static void ShowWindow()
    {
        GetWindow<VRLocomotionSetupWizard>("VR Locomotion Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("VR Locomotion System Quick Setup", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Object references
        EditorGUILayout.LabelField("Scene Object References", EditorStyles.boldLabel);
        xrOrigin = (GameObject)EditorGUILayout.ObjectField("XR Origin", xrOrigin, typeof(GameObject), true);
        leftController = (GameObject)EditorGUILayout.ObjectField("Left Controller", leftController, typeof(GameObject), true);
        rightController = (GameObject)EditorGUILayout.ObjectField("Right Controller", rightController, typeof(GameObject), true);
        cameraObject = (GameObject)EditorGUILayout.ObjectField("Camera", cameraObject, typeof(GameObject), true);
        
        EditorGUILayout.Space();
        
        // Auto find button
        if (GUILayout.Button("Auto Find Objects"))
        {
            AutoFindObjects();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Setup Options", EditorStyles.boldLabel);
        
        setupContinuousMove = EditorGUILayout.Toggle("Setup Continuous Move", setupContinuousMove);
        setupSnapTurn = EditorGUILayout.Toggle("Setup Snap Turn", setupSnapTurn);
        
        EditorGUILayout.Space();
        
        // Setup button
        GUI.enabled = xrOrigin != null;
        if (GUILayout.Button("Start Setup", GUILayout.Height(40)))
        {
            SetupVRLocomotion();
        }
        GUI.enabled = true;
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "This wizard will help you quickly setup VR movement and rotation system.\n" +
            "1. Click 'Auto Find Objects' to find VR objects in scene\n" +
            "2. Select features to setup\n" +
            "3. Click 'Start Setup'", 
            MessageType.Info);
    }

    private void AutoFindObjects()
    {
        // Find XR Origin - use name search (compatible with all versions)
        xrOrigin = GameObject.Find("XR Origin");
        if (xrOrigin == null)
        {
            xrOrigin = GameObject.Find("XROrigin");
        }
        
        // If still not found, try finding objects containing XR Origin
        if (xrOrigin == null)
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (var obj in allObjects)
            {
                if (obj.name.Contains("XR") && obj.name.Contains("Origin"))
                {
                    xrOrigin = obj;
                    break;
                }
            }
        }
        
        // Find camera
        if (Camera.main != null)
        {
            cameraObject = Camera.main.gameObject;
        }
        
        // Try finding controllers by name
        GameObject[] allSceneObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allSceneObjects)
        {
            string lowerName = obj.name.ToLower();
            
            if ((lowerName.Contains("left") && lowerName.Contains("hand")) || 
                (lowerName.Contains("left") && lowerName.Contains("controller")) ||
                lowerName.Contains("lefthand"))
            {
                leftController = obj;
            }
            
            if ((lowerName.Contains("right") && lowerName.Contains("hand")) || 
                (lowerName.Contains("right") && lowerName.Contains("controller")) ||
                lowerName.Contains("righthand"))
            {
                rightController = obj;
            }
        }
    }

    private void SetupVRLocomotion()
    {
        if (xrOrigin == null)
        {
            EditorUtility.DisplayDialog("Error", "Please specify XR Origin object first!", "OK");
            return;
        }
        
        Undo.RegisterCompleteObjectUndo(xrOrigin, "Setup VR Locomotion");
        
        // Setup continuous movement
        if (setupContinuousMove)
        {
            SetupContinuousMoveComponent();
        }
        
        // Setup rotation
        if (setupSnapTurn)
        {
            SetupSnapTurnComponent();
        }
        
        // Setup locomotion manager
        SetupLocomotionManager();
        
        EditorUtility.DisplayDialog("Complete", "VR locomotion system setup complete!\nPlease check Inspector to verify configuration.", "OK");
    }

    private void SetupContinuousMoveComponent()
    {
        VRContinuousMove continuousMove = xrOrigin.GetComponent<VRContinuousMove>();
        if (continuousMove == null)
        {
            continuousMove = xrOrigin.AddComponent<VRContinuousMove>();
        }
        
        // Setup references
        SerializedObject so = new SerializedObject(continuousMove);
        
        if (xrOrigin != null)
            so.FindProperty("xrOrigin").objectReferenceValue = xrOrigin.transform;
        
        if (cameraObject != null)
            so.FindProperty("headTransform").objectReferenceValue = cameraObject.transform;
        
        so.FindProperty("useHeadDirection").boolValue = true;
        so.FindProperty("moveSpeed").floatValue = 2.0f;
        so.FindProperty("sprintMultiplier").floatValue = 1.5f;
        
        so.ApplyModifiedProperties();
    }

    private void SetupSnapTurnComponent()
    {
        VRSnapTurn snapTurn = xrOrigin.GetComponent<VRSnapTurn>();
        if (snapTurn == null)
        {
            snapTurn = xrOrigin.AddComponent<VRSnapTurn>();
        }
        
        SerializedObject so = new SerializedObject(snapTurn);
        
        if (xrOrigin != null)
            so.FindProperty("xrOrigin").objectReferenceValue = xrOrigin.transform;
        
        so.FindProperty("turnAngle").floatValue = 45f;
        so.FindProperty("useContinuousTurn").boolValue = false;
        so.FindProperty("continuousTurnSpeed").floatValue = 60f;
        
        so.ApplyModifiedProperties();
    }

    private void SetupLocomotionManager()
    {
        VRLocomotionManager manager = xrOrigin.GetComponent<VRLocomotionManager>();
        if (manager == null)
        {
            manager = xrOrigin.AddComponent<VRLocomotionManager>();
        }
        
        SerializedObject so = new SerializedObject(manager);
        
        // Setup references
        VRContinuousMove continuousMove = xrOrigin.GetComponent<VRContinuousMove>();
        if (continuousMove != null)
        {
            so.FindProperty("continuousMove").objectReferenceValue = continuousMove;
        }
        
        VRSnapTurn snapTurn = xrOrigin.GetComponent<VRSnapTurn>();
        if (snapTurn != null)
        {
            so.FindProperty("snapTurn").objectReferenceValue = snapTurn;
        }
        
        // Set default enabled
        so.FindProperty("enableMoveOnStart").boolValue = true;
        so.FindProperty("enableTurnOnStart").boolValue = true;
        
        so.ApplyModifiedProperties();
    }
}
