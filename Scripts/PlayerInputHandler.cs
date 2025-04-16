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
            NextDialogueAction = actionMap.FindAction("OnNextDialogue", true);
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
        DialogueManager.TriggerNextDialogue();
    }
}
