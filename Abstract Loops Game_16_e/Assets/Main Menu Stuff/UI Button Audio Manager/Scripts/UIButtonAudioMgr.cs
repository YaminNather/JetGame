using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonAudioMgr : MonoBehaviour
{
    public static UIButtonAudioMgr INSTANCE;
    [SerializeField] private AudioSource m_AudioSource;
    
    private void Awake()
    {
        INSTANCE = this;

        //Added AudioSource to the gameObject if not already available.
        if (TryGetComponent(out AudioSource audioSource)) m_AudioSource = audioSource;
        else m_AudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void AudioPlay_F()
    {
        m_AudioSource.Play();
    }
}
