using EstructurasDeDatos.Lista;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static HighScoresController;

public class HighScoreButtons : MonoBehaviour
{
    [SerializeField] TMP_Text highscores_text;
    [SerializeField] TMP_Text highscores_points_text;

    public void GotToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void Start()
    {
        HighScoreDatabase data = HighScoresController.ReadData();

        ListaSimplementeEnlazada<HighScoreRegister> puntajes = new();

        foreach (HighScoreRegister row in data.highScores)
        {
            puntajes.Agregar(row);
        }

        puntajes.Ordenar();

        highscores_text.text = "";
        highscores_points_text.text = "";

        for (int i = 0; i < puntajes.Cantidad() && i <= 9; i++)
        {
            highscores_text.text += $"{i+1}. {puntajes[i].auto}       {puntajes[i].mapa}       {puntajes[i].fecha}\n";
            highscores_points_text.text += $"{puntajes[i].puntos}\n";
        }
    }
}
