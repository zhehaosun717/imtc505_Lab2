using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public InputActionProperty testActionValue;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float value = testActionValue.action.ReadValue<float>();
        Debug.Log("Value: " + value);
    }
}
