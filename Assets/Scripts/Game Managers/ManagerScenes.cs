using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScenes : MonoBehaviour
{
    public enum scenes //NAME SENSITIVE
    {
        MainMenu = 0,
        Game = 1,
        Lose = 2
    };

    public static void LoadScene(scenes s)
    {
        Debug.Log("Loading " + s.ToString());
        SceneManager.LoadScene(s.ToString());
    }

    public static void LoadMainMenu()
    {
        LoadScene(scenes.MainMenu);
    }

    public void bcsUnityLoadMainMenu() //buttons don't take static functions -.-
    {
        LoadMainMenu();
    }

    public static void LoadGame()
    {
        LoadScene(scenes.Game);
    }

    public void bcsUnityLoadgame() //buttons don't take static functions -.-
    {
        LoadGame();
    }

    public static void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    public void bcsUnityQuit() //buttons don't take static functions -.-
    {
        Quit();
    }
}