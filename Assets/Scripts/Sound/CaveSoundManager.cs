using System.Collections;
using UnityEngine;

public class CaveSoundManager : MonoBehaviour
{
    IEnumerator DelayPLayMusic()
    {
        SoundManager.StopAll();

        yield return new WaitForSeconds(5);

        SoundManager.Play("Cave soundtrack");
    }

    private void Start()
    {
        StartCoroutine(DelayPLayMusic());
    }

    void Update()
    {
        
    }
}