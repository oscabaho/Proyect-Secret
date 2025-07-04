using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Conversation : MonoBehaviour
{
    [SerializeField]private GameObject panel;
    [SerializeField]private string[] conversationLines;
    DialogueScript dialogueScript;
    void Awake()
    {
        dialogueScript = panel.GetComponent<DialogueScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerStay(Collider collider)
    {
        panel.SetActive(true);
        dialogueScript.dialogueLines = conversationLines;
        dialogueScript.StartDialogue();
    }
}
