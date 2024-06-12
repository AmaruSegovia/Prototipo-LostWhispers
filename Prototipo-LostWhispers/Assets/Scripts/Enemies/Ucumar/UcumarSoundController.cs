using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UcumarSoundController : MonoBehaviour
{
    public AudioClip[] soundClips; // Array de clips de audio
    public AudioSource loopAudioSource; // Asignado desde el inspector
    public AudioSource effectAudioSource; // Asignado desde el inspector

    private Dictionary<string, AudioClip> audioClipDictionary;

    void Start()
    {
        // Inicializar el diccionario
        audioClipDictionary = new Dictionary<string, AudioClip>();
        foreach (var clip in soundClips)
        {
            audioClipDictionary[clip.name] = clip;
        }

        // Configuraciones por defecto
        loopAudioSource.volume = 1f;
        loopAudioSource.pitch = 1f;
        loopAudioSource.loop = true;

        effectAudioSource.volume = 1f;
        effectAudioSource.pitch = 1f;
        effectAudioSource.loop = false;
    }

    // Método para reproducir un sonido
    public void PlaySound(string name, bool loop = false, float volume = 1f, float pitch = 1f)
    {
        if (audioClipDictionary.TryGetValue(name, out var clip))
        {
            AudioSource audioSource = loop ? loopAudioSource : effectAudioSource;

            if (!audioSource.isPlaying || audioSource.clip != clip)
            {
                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.pitch = pitch;
                audioSource.loop = loop;
                audioSource.Play();
                Debug.Log($"Playing sound: {name} (Loop: {loop})");
            }
        }
        else
        {
            Debug.LogWarning($"Sound clip with name {name} not found!");
        }
    }

    // Método para detener un sonido específico (osea detiene los sonidos en loop o los que no estan en loop pero siempre usalo para los loop)
    public void StopSound(bool loop)
    {
        AudioSource audioSource = loop ? loopAudioSource : effectAudioSource;
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log($"Stopped sound (Loop: {loop})");
        }
    }

    // Método para detener todos los sonidos
    public void StopAllSounds()
    {
        if (loopAudioSource.isPlaying)
        {
            loopAudioSource.Stop();
            Debug.Log("Stopped loop audio source");
        }
        if (effectAudioSource.isPlaying)
        {
            effectAudioSource.Stop();
            Debug.Log("Stopped effect audio source");
        }
    }
}
