using ProyectSecret.Core;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProyectSecret.UI.Dialogue
{
    public class Conversation : MonoBehaviour
    {
        [SerializeField]private GameObject panel;
        [SerializeField]private string[] conversationLines;
        private bool dialogueActive = false;
        DialogueScript dialogueScript;
        void Awake()
        {
            dialogueScript = panel.GetComponent<DialogueScript>();
        }

        void Update()
        {
            
        }

        public void OnTriggerStay(Collider collider)
        {
            if (dialogueActive == false)
            {
                panel.SetActive(true);
                dialogueScript.dialogueLines = conversationLines;
                dialogueScript.index = 0;
                dialogueScript.StartDialogue();
                dialogueActive = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            dialogueActive = false;

        }
    }
}
