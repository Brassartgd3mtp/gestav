using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundsDatabase", menuName = "SoundsScriptableObjects/AudioDatabase")]
public class AudioScriptable : ScriptableObject
{
    [SerializeField] private AudioClip[] sounds;

    public AudioClip GetAudioClip(int index)
    {
        if (index >= 0 && index < sounds.Length)
        {
            return sounds[index];
        }
        return null;
    }

    public int GetClipCount()
    {
        return sounds.Length;
    }
}