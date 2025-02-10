using System;
using UnityEngine;
using Unity.Netcode;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance;

    public AudioSource _audioSourceMusic;
    public AudioSource _audioSourceSFX;

    public Sound[] _musicSounds;
    public Sound[] _sfxSounds;

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

    private void PlaySound(AudioSource audioSource, Sound[] soundList, string name)
    {
        Sound sound = Array.Find(soundList, s => s._name == name);

        if (sound == null)
        {
            Debug.Log($"[AudioManager] Son '{name}' introuvable !");
        }
        else
        {
            audioSource.clip = sound._clip;
            audioSource.Play();
        }
    }

    private void StopSound(AudioSource audioSource)
    {
        audioSource.Stop();
    }

    public void StopMusic()
    {
        StopSound(_audioSourceMusic);
    }

    public void StopSFX()
    {
        StopSound(_audioSourceSFX);
    }

    public void PlayMusic(string name)
    {
        PlaySound(_audioSourceMusic, _musicSounds, name);
        PlayMusicServerRpc(name);
    }

    public void PlaySFX(string name)
    {
        PlaySound(_audioSourceSFX, _sfxSounds, name);
        PlaySFXServerRpc(name);
    }

    public bool IsSoundInList(Sound[] soundList, string name)
    {
        return Array.Exists(soundList, s => s._name == name);
    }
    
    /// Demande au serveur de jouer la musique sur tous les clients.
    [ServerRpc(RequireOwnership = false)]
    private void PlayMusicServerRpc(string name)
    {
        PlayMusicClientRpc(name);
    }
    
    /// Joue la musique sur tous les clients connectés.
    [ClientRpc]
    private void PlayMusicClientRpc(string name)
    {
        if (!IsOwner)
        {
            PlaySound(_audioSourceMusic, _musicSounds, name);
        }
    }
    
    /// Demande au serveur de jouer un effet sonore sur tous les clients.
    [ServerRpc(RequireOwnership = false)]
    private void PlaySFXServerRpc(string name)
    {
        PlaySFXClientRpc(name);
    }
    
    /// Joue l'effet sonore sur tous les clients connectés.
    [ClientRpc]
    private void PlaySFXClientRpc(string name)
    {
        if (!IsOwner)
        {
            PlaySound(_audioSourceSFX, _sfxSounds, name);
        }
    }
}
