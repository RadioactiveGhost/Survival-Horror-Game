using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;

struct Fades
{
    public int duration;
    public string name;
    public bool starting;
    public float passedTime;
    public bool remove;
}

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;

    public static SoundManager mysoundcontroller;
    [HideInInspector]
    public bool onceFootsteps = true;

    public static Sound[] soundinstance;

    static List<Fades> fades;
    static List<Fades> fadesToRemove;

    void Awake()
    {
        soundinstance = new Sound[sounds.Length];

        if (mysoundcontroller == null)
        {
            mysoundcontroller = this;
            this.tag = "SoundManager";
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        soundinstance = sounds;

        fades = new List<Fades>();
        fadesToRemove = new List<Fades>();
    }

    private void Update()
    {
        for (int i = 0; i < fades.Count; i++)
        {
            Sound s = Array.Find<Sound>(soundinstance, soundinstance => soundinstance.name == fades[i].name);

            if (fades[i].starting)
            {
                s.source.volume = (fades[i].passedTime * ((s.volume / 100) * (UserSettings.volume * 100))) / fades[i].duration;
                //Debug.Log(s.source.volume + " at time " + fades[i].passedTime);
            }
            else
            {
                s.source.volume = ((fades[i].duration - fades[i].passedTime) * ((s.volume / 100) * (UserSettings.volume * 100))) / fades[i].duration;
                //Debug.Log(s.source.volume + " at time " + fades[i].passedTime);
            }

            //bcs c#
            Fades f = fades[i];
            f.passedTime += Time.deltaTime;
            fades[i] = f;

            if(fades[i].passedTime >= fades[i].duration)
            {
                fadesToRemove.Add(fades[i]);
                if (f.remove)
                {
                    Stop(f.name);
                    s.source.volume = (s.volume / 100) * (UserSettings.volume * 100);
                }
                else
                {
                    if (!fades[i].starting)
                    {
                        s.source.volume = 0;
                    }
                    else
                    {
                        s.source.volume = (s.volume / 100) * (UserSettings.volume * 100);
                    }
                }
            }
        }

        for(int i = 0; i < fadesToRemove.Count; i++)
        {
            fades.Remove(fadesToRemove[i]);
        }
        fadesToRemove.Clear();
    }

    public static void Play(string name)
    {
        Sound s = Array.Find<Sound>(soundinstance, soundinstance => soundinstance.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " is null");
            return;
        }
        s.source.Play();
    }

    public static void Stop(string name)
    {
        Sound s = Array.Find<Sound>(soundinstance, soundinstance => soundinstance.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " is null");
            return;
        }
        s.source.Stop();
    }

    public static void StopAll()
    {
        AudioSource[] audioSources = GameObject.FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource a in audioSources)
        {
            a.Stop();
        }
    }

    public static void UpdateVolume()
    {
        foreach(Sound s in soundinstance)
        {
            s.source.volume = (s.volume / 100) * (UserSettings.volume * 100); //convertions (3 simple rule)
            //Debug.Log(s.source.volume);
        }
    }

    public static void FadeInOut(int duration, string name, bool starting, bool stopWhenFadeDone)
    {
        //Debug.Log("fading in?" + starting + " " + name);

        Fades f = new Fades
        {
            duration = duration,
            name = name,
            starting = starting,
            passedTime = 0,
            remove = stopWhenFadeDone
        };
        fades.Add(f);
    }

    public static void SetVolume(string name, float volume)
    {
        Sound s = Array.Find<Sound>(soundinstance, soundinstance => soundinstance.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " is null");
            return;
        }
        s.source.volume = (volume / 100) * (UserSettings.volume * 100);
    }
}