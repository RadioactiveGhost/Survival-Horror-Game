using UnityEngine;

public class UserSettings : MonoBehaviour
{
    public static float volume = 1;

    public void SetNewVolume(float volume)
    {
        UserSettings.volume = volume;
        SoundManager.UpdateVolume();
    }
}