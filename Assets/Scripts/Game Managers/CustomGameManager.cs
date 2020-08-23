using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGameManager : MonoBehaviour
{
    public static SaveData saveData;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
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