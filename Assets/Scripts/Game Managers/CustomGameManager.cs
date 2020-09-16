using UnityEngine;

public class CustomGameManager : MonoBehaviour
{
    public static SaveData saveData;
    public static bool pauseIsWorking;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        pauseIsWorking = true;
    }

    public static void SaveGame()
    {
        SaveSystem.Save(new SaveData(GameObject.FindObjectOfType<TerrainGenerator>()));
    }

    public static void LoadGameFromSave()
    {
        saveData = SaveSystem.Load();
        ManagerScenes.LoadGame();
    }
}