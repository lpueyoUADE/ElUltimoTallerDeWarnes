using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HighScoresController : MonoBehaviour
{
    public static string filePath = Path.Combine(Application.dataPath, "highscores.json");

    [System.Serializable]
    public class HighScoreRegister : IComparable
    {
        public int puntos;
        public string fecha;
        public string auto;
        public string mapa;

        public int CompareTo(object obj)
        {

            return puntos - ((HighScoreRegister)obj).puntos;
        }
    }

    [System.Serializable]
    public class HighScoreDatabase
    {
        public List<HighScoreRegister> highScores;
    }

    public static HighScoreDatabase ReadData()
    {
        if (!File.Exists(filePath))
        {
            string newJson = JsonUtility.ToJson(new HighScoreDatabase(), true);
            File.WriteAllText(filePath, newJson);
        }

        string json = File.ReadAllText(filePath);
        HighScoreDatabase highScoreDatabase = JsonUtility.FromJson<HighScoreDatabase>(json);
        
        Debug.Log("Datos leídos de: " + filePath);
        return highScoreDatabase;
    }

    public static void WriteData(HighScoreRegister newRegsiter)
    {
        HighScoreDatabase highScoreDatabase = ReadData();

        highScoreDatabase.highScores.Add(newRegsiter);

        string json = JsonUtility.ToJson(highScoreDatabase, true);
        File.WriteAllText(filePath, json);
    }
}
