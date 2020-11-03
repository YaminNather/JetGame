using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrenciesMgr : MonoBehaviour
{
    #region Variables
    private ParticleSystem m_ParticleSystem;
    private AudioSource m_AudioSource;
    private float m_AudioLastPlayed;
    private const float m_AudioSpacing = 0.5f;
    #endregion

    private void Awake()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void OnCollect_F(Vector3 pos)
    {
        if (Time.realtimeSinceStartup - m_AudioLastPlayed > m_AudioSpacing)
        {
            m_AudioSource.Play();
            m_AudioLastPlayed = Time.realtimeSinceStartup;
        }
        transform.position = pos;
        m_ParticleSystem.Play();
    }
}
