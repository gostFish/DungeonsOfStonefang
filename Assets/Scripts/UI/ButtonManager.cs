using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{

    public void OpenSettinga()
    {
        SceneManager.LoadScene("Settings");
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void SpiritPoints()
    {
        SceneManager.LoadScene("SpiritPoints");
    }
    public void CharacterCreation()
    {
        SceneManager.LoadScene("CharacterCreation");
    }

    public void StartGame()
    {
        PlayerPrefs.SetFloat("Health", 100);
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.SetInt("NewestLevel", 1);
        PlayerPrefs.SetFloat("Hours", 8);
        PlayerPrefs.SetFloat("Minutes", 30);
        PlayerPrefs.SetInt("Day", 0);
        SceneManager.LoadScene("Level");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Level");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
