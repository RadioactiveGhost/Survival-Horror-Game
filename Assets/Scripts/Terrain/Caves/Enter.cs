﻿using UnityEngine;

public class Enter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<Player>().HasGrapplingHook)
        {
            CustomGameManager.SaveGame();
            ManagerScenes.LoadScene(scenes.Cave);
        }
    }
}