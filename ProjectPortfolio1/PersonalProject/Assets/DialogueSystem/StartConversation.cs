using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartConversation : MonoBehaviour
{
    [SerializeField] Dialogue _dialogue;
    public void ConverstionStart()
    {
        FindObjectOfType<DialogueManager>().BeginDialogue(_dialogue);
    }
}
