using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ScoreData
{
    public int _score;

    public ScoreData(IncreaseScore scoreData)
    {
        _score = scoreData.score;
    }
}
