using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrenciesMgr : MonoBehaviour
{
    #region Variables
    private ParticleSystem m_ParticleSystem;
    private AudioSource m_AudioSource;
    #endregion

    private void Awake()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void DoBurst_F(Vector3 pos)
    {
        m_AudioSource.Play();
        transform.position = pos;
        m_ParticleSystem.Play();
    }
}
