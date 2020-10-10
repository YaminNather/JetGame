using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetAudioMgr : MonoBehaviour
{
    private AudioSource m_AudioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip m_ZoomingPastBlockAC;
    [SerializeField] private AudioClip m_ExplodeAC;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();        
    }

    /// <summary>
    /// Plays Audio using attached AudioSource.
    /// </summary>
    /// <param name="audioClip">audioClipEN to play.</param>
    public void PlayAudio_F(AudioClipsEN audioClip)
    {
        m_AudioSource.PlayOneShot(AudioClipENToAudioClip_F(audioClip));
    }

    /// <summary>
    /// Uses AudioSource static function PlayClipAtPoint to play audio.
    /// </summary>
    /// <param name="audioClip">audioClipEN to play.</param>
    public void PlayAudioWithGlobalAudioSource_F(AudioClipsEN audioClip)
    {
        AudioSource.PlayClipAtPoint(AudioClipENToAudioClip_F(audioClip), transform.position);
    }


    /// <summary>
    /// Get AudioClip using AudioClipEN enum.
    /// </summary>
    /// <param name="audioClipEN">The AudioClipEN enum to get corresponding audio clip for.</param>
    /// <returns></returns>
    private AudioClip AudioClipENToAudioClip_F(AudioClipsEN audioClipEN)
    {
        return audioClipEN switch
        {
            AudioClipsEN.ZoomingPastBlock => m_ZoomingPastBlockAC,
            AudioClipsEN.Explode => m_ExplodeAC,
            _ => null
        };
    }

    public enum AudioClipsEN { ZoomingPastBlock, Explode }    
}
