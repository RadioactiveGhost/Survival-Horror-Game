using System.Collections;
using UnityEngine;

public class CaveSoundManager : MonoBehaviour
{
    Player p;
    string s, temp;
    public int duration;

    IEnumerator DelayPLayMusic(string s, int delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);

        SoundManager.Play(s);
    }

    private void Start()
    {
        p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        SoundManager.Play("Cave soundtrack");
        SoundManager.SetVolume("Cave soundtrack", 1);
        SoundManager.Play("Cave soundtrack 2");
        SoundManager.SetVolume("Cave soundtrack 2", 0);
        SoundManager.Play("Cave soundtrack 3");
        SoundManager.SetVolume("Cave soundtrack 3", 0);

        temp = ChooseSoundtrack();
        ChangeSoundtrackVolumes();

        s = temp;
    }

    private void Update()
    {
        temp = ChooseSoundtrack();
        if (s != temp)
        {
            ChangeSoundtrackVolumes();
        }
        s = temp;
    }

    private string ChooseSoundtrack()
    {
        if(p.health >= p.maxHealth * 0.8)
        {
            return "Cave soundtrack";
        }
        else if(p.health >= p.maxHealth * 0.4)
        {
            return "Cave soundtrack 2";
        }
        else
        {
            return "Cave soundtrack 3";
        }
    }

    void ChangeSoundtrackVolumes()
    {
        //fade in
        if(temp == "Cave soundtrack")
        {
            SoundManager.FadeInOut(duration, temp, true, false);
        }
        else if(temp == "Cave soundtrack 2")
        {
            SoundManager.FadeInOut(duration, temp, true, false);
        }
        else if (temp == "Cave soundtrack 3")
        {
            SoundManager.FadeInOut(duration, temp, true, false);
        }

        //fade out
        if (s == "Cave soundtrack")
        {
            SoundManager.FadeInOut(duration, s, false, false);
        }
        else if (s == "Cave soundtrack 2")
        {
            SoundManager.FadeInOut(duration, s, false, false);
        }
        else if(s == "Cave soundtrack 3")
        {
            SoundManager.FadeInOut(duration, s, false, false);
        }
    }
}