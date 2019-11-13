using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Queue<string> _sentenceHolder;
    
    [SerializeField] Text _name;
    [SerializeField] Text _dialogueText;
    [SerializeField] Animator _anim;
    // Start is called before the first frame update
    void Start()
    {
        _sentenceHolder = new Queue<string>();
    }
     public void BeginDialogue(Dialogue d)
    {
        _anim.SetBool("Open",true);
        //  Debug.Log("Converstion started");
        _name.text = d.name;
        _sentenceHolder.Clear();
        foreach(string sentence in d.sentences)
        {
            _sentenceHolder.Enqueue(sentence);
        }
        ShowNextSentence();
    }
    

    public void ShowNextSentence()
    {
        if (_sentenceHolder.Count == 0)
        {
            FinishDialogue();
            return;
        }
        string s = _sentenceHolder.Dequeue();
        //Debug.Log(s);
        _dialogueText.text = s;
    }


    private void FinishDialogue()
    {
        _anim.SetBool("Open", false);
    }
}
