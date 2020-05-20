using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (!TryGetComponent(out AudioSource source))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.priority = 128;
            audioSource.maxDistance = 30f;
            audioSource.minDistance = 10f;
            audioSource.spatialBlend = 1f;
            audioSource.loop = true;
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }

        audioSource.volume *= AudioManager.Instance.levels[2]; //audiolevel for global sounds
    }

    // Update is called once per frame
    void Update()
    {
        if(AudioManager.Instance.globalMute)
        {
            if (!audioSource.mute)
            {
                audioSource.mute = true;
            }
        }
        else
        {
            if(audioSource.mute)
            {
                audioSource.mute = false;
            }
        }
    }

    public void SetVolume(float volume)
    {
        if(!AudioManager.Instance.globalMute)
        {
            audioSource.volume = volume;
        }
    }
}
