using UnityEngine;

public class SceneInterface : MonoBehaviour
{
    public void LoadMainMenu()
    {
        ManagerScenes.LoadScene(scenes.MainMenu);
    }

    public void LoadGameFromSave()
    {
        CustomGameManager.LoadGameFromSave();
    }

    public void LoadGame()
    {
        ManagerScenes.LoadScene(scenes.Game);
    }

    public void SaveGame()
    {
        CustomGameManager.SaveGame();
    }

    public void Quit()
    {
        ManagerScenes.Quit();
    }
}