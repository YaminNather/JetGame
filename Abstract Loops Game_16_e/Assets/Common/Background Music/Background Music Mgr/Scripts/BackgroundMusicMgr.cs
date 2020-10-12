using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BackgroundMusicMgr : MonoBehaviour
{
    private AudioSource m_AudioSource;    

    private void Awake()
    {
        m_AudioSource = gameObject.AddComponent<AudioSource>();
        AudioMixer mixer = Resources.Load("Audio Mixer/0_AudioMixer") as AudioMixer;
        m_AudioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Background Music")[0];
    }

    public void Play_F(AudioClip audioClip)
    {
        if (audioClip == null) throw new System.Exception($"AudioClip is null!!!");

        m_AudioSource.clip = audioClip;
        m_AudioSource.Play();
    }

    public void Pause_F()
    {
        if (m_AudioSource.isPlaying) m_AudioSource.Pause();
    }

    public void Stop_F()
    {
        if (m_AudioSource.isPlaying) m_AudioSource.Stop();
    }    

    public void FadeOut_F(float time = 1, AudioClip audioClip = null)
    {
        if (!m_AudioSource.isPlaying) throw new System.Exception("No Audio is playing ATM!!!");

        TweenCallback Action_0 = () =>
        {
            Stop_F();
            m_AudioSource.volume = 1f;
        };
        if (audioClip != null)
            Action_0 += () => Play_F(audioClip);
        
        m_AudioSource.DOFade(0f, time).OnComplete(Action_0);          
    }
}