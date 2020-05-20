using UnityEngine.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
    public Sound[] musicList;
    public Sound[] combatSounds;
    public Sound[] messageSounds;
    public Sound[] atmoSounds;
    public Sound[] workerSounds;
    public Sound[] ambientSounds;

    private int soundLimit;
    public bool globalMute = false;
    public bool musicMute = false;
    public bool messageMute = false;
    public float[] levels;

    public int trackList;
    public int trackDump;
    private bool focus = true;

    private void Start()
    {
        levels = new float[3];

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i] = 1f;
        }

        InitializeAudioSources(musicList, 0);
        InitializeAudioSources(messageSounds, 0);
        InitializeAudioSources(ambientSounds, 10);

        soundLimit = 0;

        StartCoroutine(CleanLimit());
    }

    void Update()
    {
        if(GameManager.Instance.mapIndex > 1)
        {
            if(musicList[0].source.isPlaying)
            {
                StopMusicWithEffect(musicList[0].name);
                trackList = 1;
                trackDump = trackList - 1;
            }

            if(!musicList[trackList].source.isPlaying && !musicList[trackDump].source.isPlaying && focus)
            {
                musicList[trackList].source.Play();
                if (trackList < musicList.Length - 1)
                {
                    trackList++;
                    trackDump = trackList - 1;
                }
                else
                {
                    trackList = 1;
                    trackDump = musicList.Length - 1;
                }
            }
        }
        else
        {
            for (int i = 1; i < musicList.Length; i++)
            {
                if(musicList[i].source.isPlaying)
                {
                    musicList[i].source.Stop();
                }
            }
        }
    }

    public void PlayMusic(string name, bool isLoop)
    {
        if (!musicMute)
        {
            Sound s = Array.Find(musicList, sound => sound.name == name);
            if (s == null)
                return;

            if (s.source != null)
            {
                if (!s.source.isPlaying)
                {
                    s.source.loop = isLoop;
                    s.source.Play();
                }
            }
        }
    }

    public void StopMusicWithEffect(string name)
    {
        Sound s = Array.Find(musicList, sound => sound.name == name);
        if (s == null)
            return;

        if (s.source != null)
        {
            if (s.source.isPlaying)
            {
                s.source.volume -= 0.05f;
                if(s.source.volume < 0.1f)
                {
                    s.source.Stop();
                    s.source.volume = s.volume * levels[0];
                }
            }
        }
    }

    public void PlayMessageSound(string name)
    {
        if (!messageMute)
        {
            Sound s = Array.Find(messageSounds, sound => sound.name == name);
            if (s == null)
                return;

            if (s.source != null)
            {
                if (!s.source.isPlaying)
                {
                    s.source.Play();
                }
            }
        }
    }

    public void PlayAtmosphereSound(string name)
    {
        if (!globalMute)
        {
            Sound s = Array.Find(ambientSounds, sound => sound.name == name);
            if (s == null)
                return;

            if (s.source != null)
            {
                if (!s.source.isPlaying)
                {
                    s.source.loop = true;
                    s.source.Play();
                }
            }
        }
    }

    public void StopAtmosphereSounds()
    {
        if(ambientSounds.Length > 0)
        {
            foreach(Sound sound in ambientSounds)
            {
                if (sound.source != null)
                {
                    if (sound.source.isPlaying)
                    {
                        sound.source.Stop();
                    }
                }
            }
        }
    }

    public void PlayCombatSound(string name, Vector3 position)
    {
        Sound s = Array.Find(combatSounds, sound => sound.name == name);
        if (s == null)
            return;

        PlayAtPoint(s, position, true, 11);
    }

    public void PlayWorkerSound(string name, Vector3 position)
    {
        Sound s = Array.Find(workerSounds, sound => sound.name == name);
        if (s == null)
            return;

        PlayAtPoint(s, position, false, 0);
    }

    public void PlayBuildingSound(ObjectList name, Vector3 position)
    {
        Sound s = Array.Find(atmoSounds, sound => sound.name == "" + name);
        if (s == null)
            return;

        PlayAtPoint(s, position, false, 0);
    }

    public Sound GetSound(string soundName, int soundList)
    {
        if (soundList == 0)
        {
            Sound s = Array.Find(combatSounds, sound => sound.name == soundName);
            if (s != null)
            {
                return s;
            }
        }
        else if (soundList == 1)
        {
            Sound s = Array.Find(atmoSounds, sound => sound.name == soundName);
            if (s != null)
            {
                return s;
            }
        }
        else
        {
            // Work in progress
        }

        return null;
    }

    public void PlayRandomCombatSound(Vector3 position)
    {
        PlayAtPoint(combatSounds[UnityEngine.Random.Range(0, 4)], position, true, 8);
    }

    public void PlayRandomWorkerSound(Vector3 position)
    {
        PlayAtPoint(workerSounds[UnityEngine.Random.Range(3, 7)], position, false, 0);
    }

    public void PlayFootstepSound(Vector3 position)
    {
        PlayAtPoint(atmoSounds[UnityEngine.Random.Range(6, 10)], position, true, 5);
    }

    public void ApplyAudioLevels(int listNum, bool muted)
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length > 0)
        {
            if (listNum > -1 && listNum < 3)
            {
                if (listNum == 0)
                {
                    if(muted)
                    {
                        musicMute = true;
                        for (int i = 0; i < musicList.Length; i++)
                        {
                            musicList[i].source.mute = true;
                        }
                    }
                    else
                    {
                        musicMute = false;
                        for (int i = 0; i < musicList.Length; i++)
                        {
                            musicList[i].source.mute = false;
                            musicList[i].source.volume = musicList[i].volume * levels[0];
                        }
                    }
                }
                else if(listNum == 1)
                {
                    if (muted)
                    {
                        messageMute = true;
                        for (int i = 0; i < messageSounds.Length; i++)
                        {
                            messageSounds[i].source.mute = true;
                        }
                    }
                    else
                    {
                        messageMute = false;
                        for (int i = 0; i < messageSounds.Length; i++)
                        {
                            messageSounds[i].source.mute = false;
                            messageSounds[i].source.volume = messageSounds[i].volume * levels[1];
                        }
                    }
                }
                else
                {
                    if (muted)
                    {
                        globalMute = true;
                        for (int i = 0; i < ambientSounds.Length; i++)
                        {
                            ambientSounds[i].source.mute = true;
                        }
                    }
                    else
                    {
                        globalMute = false;
                        for (int i = 0; i < ambientSounds.Length; i++)
                        {
                            ambientSounds[i].source.mute = false;
                            ambientSounds[i].source.volume = ambientSounds[i].volume * levels[2];
                        }
                    }
                }
            }
        }
    }

    private void PlayAtPoint(Sound s, Vector3 position, bool isLimited, int limit)
    {
        if (!globalMute)
        {
            if (isLimited)
            {
                if (soundLimit > limit)
                {
                    return;
                }
                else
                {
                    soundLimit++;
                }
            }

            GameObject temp = new GameObject();
            AudioSource source = temp.AddComponent<AudioSource>();
            temp.transform.position = position;
            float clipLength;
            if (s != null)
            {
                source.clip = s.clip;
                source.volume = s.volume * levels[2];
                source.pitch = s.pitch;
                source.spatialBlend = 1f;
                source.priority = 128;
                source.dopplerLevel = 0f;
                source.minDistance = s.minDistance;
                source.maxDistance = s.maxDistance;
                source.Play();
                clipLength = source.clip.length;
            }
            else
            {
                clipLength = 0f;
            }

            Destroy(temp, clipLength);
        }
    }

    private void InitializeAudioSources(Sound[] soundList, int soundPriority)
    {
        if (soundList.Length > 0)
        {
            foreach (Sound sound in soundList)
            {
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.priority = soundPriority;
                sound.source.minDistance = sound.minDistance;
                sound.source.maxDistance = sound.maxDistance;
            }
        }
    }

    private IEnumerator CleanLimit()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.8f);

            soundLimit = 0;
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        focus = hasFocus;
    }
}
