using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyPSMgr : MonoBehaviour
{
    private ParticleSystem m_ParticleSystem;

    private void Awake()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }

    public void DoBurst_F(Vector3 pos)
    {
        transform.position = pos;
        m_ParticleSystem.Play();
    }
}
