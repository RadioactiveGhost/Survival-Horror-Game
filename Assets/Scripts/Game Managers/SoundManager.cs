using UnityEngine;
using UnityEngine.Audio;
using System;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;

    Animator playerAnimator;

    public static SoundManager mysoundcontroller;
    [HideInInspector]
    public bool onceFootsteps = true;

    public static Sound[] soundinstance;

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
    }

    void Start()
    {

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
}