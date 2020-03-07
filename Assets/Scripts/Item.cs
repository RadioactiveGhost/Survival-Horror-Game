using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { STONE, STICKS, METAL };
public class Item : MonoBehaviour
{
    public ItemType type;

    public Sprite spriteNeutral;

    public Sprite spriteHighlight;

    public int maxSize; // sets stackable size, if 0 means non stackable

    public void Use()
    {
        switch (type)
        {
            case ItemType.STONE:
                Debug.Log("I just used stone");
                break;
            case ItemType.STICKS:
                Debug.Log("I just used sticks");
                break;
            case ItemType.METAL:
                Debug.Log("I just used metal");
                break;
        }
    }
}
