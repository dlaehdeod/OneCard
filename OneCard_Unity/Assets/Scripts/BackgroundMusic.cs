using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic instance = null;
    private AudioSource audioSource;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MusicOn ()
    {
        audioSource.Play();
    }

    public void MusicOff ()
    {
        audioSource.Stop();
    }
}