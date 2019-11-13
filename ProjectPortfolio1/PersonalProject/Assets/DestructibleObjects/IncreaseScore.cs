using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncreaseScore : MonoBehaviour
{
   [SerializeField] private Text scoreTxt;
    public int score;
    
    public void SaveScore()
    {
        SyastemSave.SaveScore(this);
    }


    public void LoadScore()
    {
        ScoreData scoreData= SyastemSave.LoadScore();
        score = scoreData._score;
    }
    // Update is called once per frame
    void Update()
    {
        scoreTxt.text = "Score: " + score;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            score++;
        }
    }
}
