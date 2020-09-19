using UnityEngine;

public class CustomGameManager : MonoBehaviour
{
    public static SaveData saveData;
    public static bool pauseIsWorking;
    public static bool hasGrapplingHook;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        pauseIsWorking = true;
        hasGrapplingHook = false;
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