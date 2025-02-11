using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance;
    public AudioSource _audioSourceMusic;
    private List<AudioSource> _currentSFXSources = new List<AudioSource>();
    private List<Sound> _currentSFX = new List<Sound>();
    public Sound[] _musicSounds;
    public Sound[] _sfxSounds;
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

    private void PlaySound(AudioSource audioSource, Sound[] soundList, string name, bool isSFX = false)
    {
        Sound sound = Array.Find(soundList, s => s._name == name);

        if (sound == null)
        {
            Debug.Log($"[AudioManager] Son '{name}' introuvable !");
            return;
        }

        if (isSFX)
        {
            if (sfxContainer == null)
            {
                Debug.LogError("[AudioManager] Aucun conteneur SFX assigné !");
                return;
            }
            
            // Créer un nouvel AudioSource attaché au conteneur SFX
            AudioSource sfxSource = sfxContainer.AddComponent<AudioSource>();
            sfxSource.clip = sound._clip;
            sfxSource.Play();
            
            _currentSFXSources.Add(sfxSource);
            _currentSFX.Add(sound);

            // Détruire l'AudioSource quand il a fini de jouer
            Destroy(sfxSource, sound._clip.length);
        }
        else
        {
            audioSource.clip = sound._clip;
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        _audioSourceMusic.Stop();
    }

    public void StopSFX(string name)
    {
        for (int i = _currentSFXSources.Count - 1; i >= 0; i--)
        {
            if (_currentSFX[i]._name == name)
            {
                _currentSFXSources[i].Stop();
                Destroy(_currentSFXSources[i]);
                _currentSFXSources.RemoveAt(i);
                _currentSFX.RemoveAt(i);
            }
        }
    }

    public void PlayMusic(string name)
    {
        PlaySound(_audioSourceMusic, _musicSounds, name);
        PlayMusicServerRpc(name);
    }

    public void PlaySFX(string name)
    {
        PlaySound(null, _sfxSounds, name, true);
        PlaySFXServerRpc(name);
    }

    public bool IsSoundInList(Sound[] soundList, string name)
    {
        return Array.Exists(soundList, s => s._name == name);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayMusicServerRpc(string name)
    {
        PlayMusicClientRpc(name);
    }

    [ClientRpc]
    private void PlayMusicClientRpc(string name)
    {
        if (!IsOwner)
        {
            PlaySound(_audioSourceMusic, _musicSounds, name);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlaySFXServerRpc(string name)
    {
        PlaySFXClientRpc(name);
    }

    [ClientRpc]
    private void PlaySFXClientRpc(string name)
    {
        if (!IsOwner)
        {
            PlaySound(null, _sfxSounds, name, true);
        }
    }
}