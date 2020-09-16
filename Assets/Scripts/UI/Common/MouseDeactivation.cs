using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDeactivation : MonoBehaviour
{
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
