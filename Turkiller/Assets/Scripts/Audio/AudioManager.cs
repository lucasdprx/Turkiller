using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[System.Serializable]
public class SerializableSoundArray
{
    public Sound[] sounds;
}

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance;
    public AudioSource _audioSourceMusic;
    private List<AudioSource> _currentSFXSources = new List<AudioSource>();
    private List<Sound> _currentSFX = new List<Sound>();
    public Sound[] _musicSounds;
    public Sound[] _sfxSounds;

    [SerializeField] public SerializableSoundArray[] _painSounds;
    [SerializeField] public SerializableSoundArray[] _deathSounds;

    public GameObject sfxContainer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsSFXPlaying(string name)
    {
        return _currentSFX.Exists(s => s._name == name);
    }

    public float GetSFXLength(string name)
    {
        Sound sound = Array.Find(_sfxSounds, s => s._name == name);
        return sound != null ? sound._clip.length : 0f;
    }

    private void PlaySoundAtPosition(Vector3 position, Sound[] soundList, string name, float pitch = 1f)
    {
        Sound sound = Array.Find(soundList, s => s._name == name);
        if (sound == null)
        {
            Debug.Log($"[AudioManager] Son '{name}' introuvable !");
            return;
        }

        GameObject soundObject = new GameObject("SFX_" + name);
        soundObject.transform.position = position;
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = sound._clip;
        audioSource.pitch = pitch;
        audioSource.spatialBlend = 1f; // Son 3D
        audioSource.Play();
        
        _currentSFXSources.Add(audioSource);
        _currentSFX.Add(sound);
        
        Destroy(soundObject, sound._clip.length);
    }

    public void StopSFX(string name)
    {
        for (int i = _currentSFXSources.Count - 1; i >= 0; i--)
        {
            if (_currentSFX[i]._name == name)
            {
                if (_currentSFXSources[i] != null)
                {
                    _currentSFXSources[i].Stop();
                    Destroy(_currentSFXSources[i].gameObject);
                }
                _currentSFXSources.RemoveAt(i);
                _currentSFX.RemoveAt(i);
            }
        }
    }

    public void PlaySFX(string name, bool isPitched, Vector3 position)
    {
        float pitch = isPitched ? UnityEngine.Random.Range(0.8f, 1.2f) : 1f;
        PlaySoundAtPosition(position, _sfxSounds, name, pitch);
    }

    public void PlaySFXPain(int index, Vector3 position)
    {
        PlayRandomSoundFromArray(_painSounds, index, position);
    }

    public void PlaySFXDeath(int index, Vector3 position)
    {
        PlayRandomSoundFromArray(_deathSounds, index, position);
    }

    private void PlayRandomSoundFromArray(SerializableSoundArray[] soundArray, int index, Vector3 position)
    {
        if (index < 0 || index >= soundArray.Length || soundArray[index].sounds.Length == 0) return;

        int randomIndex = UnityEngine.Random.Range(0, soundArray[index].sounds.Length);
        PlaySoundAtPosition(position, soundArray[index].sounds, soundArray[index].sounds[randomIndex]._name);
    }
}