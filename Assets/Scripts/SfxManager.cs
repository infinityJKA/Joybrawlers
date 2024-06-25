using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundObject;
    public AudioSource musicSource;
    public static SfxManager instance;

    private void Awake(){
        if(instance == null){
            instance = this;
        }
    }

    public void PlaySFX(AudioClip audioClip, Transform spawnTransform, float volume){
        AudioSource audioSource = Instantiate(soundObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject,clipLength);
    }

    public void PlayNewMusic(AudioClip music, float v){
        musicSource.clip = music;
        musicSource.volume = v;
        musicSource.Play();
    }

}
