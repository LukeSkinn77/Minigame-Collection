using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AudioPlay
{
    private AudioSource aud;
    private int id = -1;
    public List<Audio> soundEffects;

    public AudioPlay(AudioSource _aud)
    {
        aud = _aud;
    }

    public void AssignAudioSource(AudioSource _aud)
    {
        aud = _aud;
        id = -1;
    }

    public void PlaySoundEffect(int audId)
    {
        if (audId != id)
        {
            id = audId;
            aud.clip = soundEffects[audId].audioClip;
        }
        aud.Play();
    }
}

[System.Serializable]
public class Audio
{
    public AudioClip audioClip;
}