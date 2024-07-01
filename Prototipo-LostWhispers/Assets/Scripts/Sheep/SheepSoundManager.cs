using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SheepSoundManager : MonoBehaviour
{
    public List<AudioClip> sheepSounds; // Lista de sonidos de la oveja.
    public AudioSource audioSource; // Componente AudioSource para reproducir los sonidos.

    void Start()
    {
        if (sheepSounds.Count > 0 && audioSource != null)
        {
            StartCoroutine(PlaySheepSound());
        }
    }

      IEnumerator PlaySheepSound()
    {
        while (true)
        {
            // Espera un tiempo aleatorio entre 5 y 10 segundos.
            float waitTime = Random.Range(5f, 10f);
            yield return new WaitForSeconds(waitTime);

            // Selecciona un sonido aleatorio de la lista.
            int randomIndex = Random.Range(0, sheepSounds.Count);
            AudioClip randomSound = sheepSounds[randomIndex];

            // Reproduce el sonido aleatorio.
            audioSource.clip = randomSound;
            audioSource.Play();
        }
    }

    public void stopSound()
    {
        StopAllCoroutines();
    }
}
