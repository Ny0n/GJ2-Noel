using System;
using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    public bool IsMusicActive = true;
    public bool IsSFXActive = true;

    public float FadeTime = 1f;
    
    // Sounds
    public Sound[] Sounds;
    
    // Unity functions ///////////////////////////////////////////////////

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        // we create the audio source for every sound
        foreach (Sound s in Sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.loop = s.Loop;
            s.Source.volume = s.Volume;
            s.Source.spatialBlend = (s.Blend == Sound.TypeBlend.None_2D) ? 0.0f : 1.0f;
            s.Source.maxDistance = 10.0f;
        }
    }

    private void Update()
    {
        if (!IsPlaying("theme"))
            Play("theme"); // we play the theme song

        if (Input.GetKeyDown(KeyCode.A))
        {
            Play("taunt");
        }
    }

    [ClientRpc]
    private void LaunchTauntSoundClientRpc()
    {
        // Play sound
        Play("taunt");
    }
    
    [ServerRpc]
    private void LaunchTauntSoundServerRpc()
    {
        LaunchTauntSoundClientRpc();
    }

    // Class functions ///////////////////////////////////////////////////

    public void SetActiveMusic(bool b)
    {
        foreach (Sound s in Sounds)
        {
            if (s.Type == Sound.TypeSound.Music) s.Source.volume = (b) ? s.Volume : 0f;
        }
        IsMusicActive = b;
    }

    public void SetActiveSFX(bool b)
    {
        foreach (Sound s in Sounds)
        {
            if (s.Type == Sound.TypeSound.SFX) s.Source.volume = (b) ? s.Volume : 0f;
        }
        IsSFXActive = b;
    }

    public void StopPlayingAll()
    {
        foreach (Sound s in Sounds)
        {
            s.Source.Stop();
        }
    }

    public void SetPauseAll(bool b)
    {
        AudioListener.pause = b;
    }

    public void Play(string sound)
    {
        Sound s = Array.Find(Sounds, item => item.Name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }

        s.Source.Play();
    }

    public void StopPlaying(string sound)
    {
        Sound s = Array.Find(Sounds, item => item.Name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }

        s.Source.Stop();
    }

    public bool IsPlaying(string sound)
    {
        Sound s = Array.Find(Sounds, item => item.Name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return false;
        }

        return s.Source.isPlaying;
    }

    public void FadeOutSound(string sound)
    {
        Sound s = Array.Find(Sounds, item => item.Name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }

        StartCoroutine(FadeOut(s));
    }

    private IEnumerator FadeOut(Sound s)
    {
        float startVolume = s.Source.volume;

        while (s.Source.volume > 0)
        {
            s.Source.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }

        s.Source.Stop();
        s.Source.volume = startVolume;
    }
    
}
