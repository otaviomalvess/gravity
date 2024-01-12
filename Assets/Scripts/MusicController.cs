using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] AudioClip acAmbience;
    [SerializeField] AudioClip acMusic;

    AudioSource asrc;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        asrc = GetComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        asrc.clip = acMusic;
        asrc.Play(); 
    }

    public void Stop()
    {
        asrc.Stop();
    }
}
