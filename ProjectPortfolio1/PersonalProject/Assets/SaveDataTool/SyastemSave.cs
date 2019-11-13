using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SyastemSave 
{
    public static void SaveScore(IncreaseScore score)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/score.encription";
        FileStream stream = new FileStream(path, FileMode.Create);


        ScoreData data = new ScoreData(score);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static ScoreData LoadScore()
    {
        string path = Application.persistentDataPath + "/score.encription";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);


            ScoreData data = formatter.Deserialize(stream) as ScoreData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in: " + path);
            return null;
        }
    }
}
