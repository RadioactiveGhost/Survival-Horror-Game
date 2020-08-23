using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum scenes //NAME SENSITIVE
{
    MainMenu = 0,
    Game = 1,
    Lose = 2
};

public class ManagerScenes : MonoBehaviour
{

    public static void LoadScene(scenes s)
    {
        //Debug.Log("Loading " + s.ToString());
        SceneManager.LoadScene(s.ToString());
    }

    public static void LoadMainMenu()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        //Debug.Log("Loading Main Menu");
        LoadScene(scenes.MainMenu);
    }

    public void bcsUnityLoadMainMenu() //buttons don't take static functions -.-
    {
        //Debug.Log("Directing to load Main Menu");
        LoadMainMenu();
    }

    public static void LoadGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        LoadScene(scenes.Game);
    }

    public void bcsUnityLoadgame() //buttons don't take static functions -.-
    {
        LoadGame();
    }

    public static void Quit()
    {
        //Debug.Log("Quitting");
        Application.Quit();
    }

    public void bcsUnityQuit() //buttons don't take static functions -.-
    {
        Quit();
    }
}