using JDialogue_System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionAsset InputActionAsset;
    
    private InputAction NextDialogueAction;

    void Awake()
    {
        if (InputActionAsset != null)
        {
            var actionMap = InputActionAsset.FindActionMap("UI", true);
            if (actionMap != null)
            {
                NextDialogueAction = actionMap.FindAction("OnNextDialogue", true);
            }
            else
            {
                Debug.LogError($"[{name}]: UI Action Map not found!");
            }
        }
        else
        {
            Debug.LogError($"[{name}]: InputActionAsset is not assigned!");
        }
    }

    void OnEnable()
    {
        if (NextDialogueAction != null)
        {
            NextDialogueAction.performed += OnNextDialogue;
            NextDialogueAction.Enable();
        }
    }

    void OnDisable()
    {
        if (NextDialogueAction != null)
        {
            NextDialogueAction.performed -= OnNextDialogue;
            NextDialogueAction.Disable();
        }
    }
    
    private void OnNextDialogue(InputAction.CallbackContext context)
    {
        JDialogueRunner.TriggerNextDialogue();
    }
}
