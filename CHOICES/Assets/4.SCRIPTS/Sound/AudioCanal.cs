using System;
using UnityEngine;

[System.Serializable]
public class AudioCanal
{
    public string name;
    public AudioSource audio;
    public bool play;

    public AudioCanal(string iName, AudioSource iAudioSource)
    {
        name = iName;
        audio = iAudioSource;
        play = false;
    }
}
