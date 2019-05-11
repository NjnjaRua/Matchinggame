using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SoundId
{
    NONE = 0,
    TOUCH,
    PLAYING,
    SWAP,
    SCORE, 
    CREATE,
    QUIT,
    LOADING,
    FLY
}


[System.Serializable]
public struct SoundRes
{
    public string name;
    public AudioClip audoClip;
};

public class SoundManager : MonoBehaviour {
    
    [SerializeField]
    private AudioSource audioSourceSound = null;

    public SoundRes[] soundList;


    private static SoundManager instance;
    private List<AudioSource> sourceList;

    private void Awake()
    {
        sourceList = new List<AudioSource>();
        instance = this;
    }

    
    public static SoundManager getInstance()
    {
        return instance;
    }

    public void AddSource(AudioSource src)
    {
        if (!sourceList.Contains(src))
        {
            sourceList.Add(src);
        }
    }

    public void RemoveSource(AudioSource src)
    {
        sourceList.Remove(src);
    }


    public void PlaySound(SoundId id, bool isLoop = false, long delay = 0)
    {
        int soundId = (int)id;
        if (soundId < 0 || soundId >= soundList.Length)
            return;
        if (audioSourceSound != null)
        {
            audioSourceSound.clip = soundList[soundId].audoClip;
            audioSourceSound.loop = isLoop;
            audioSourceSound.Play((ulong)delay);
        }
    }

    public void StopSound()
    {
        if (audioSourceSound != null)
            audioSourceSound.Stop();
    }
}
