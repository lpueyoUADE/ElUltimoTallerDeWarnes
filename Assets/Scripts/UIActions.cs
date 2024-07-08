using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIActions : MonoBehaviour
{
    public void PlayGameButton()
    {
        SceneManager.LoadScene("Game");
    }
    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void CityButton()
    {
        SceneManager.LoadScene("City");
    }

    public void ExitButton()
    {
         Application.Quit();
    }
}
