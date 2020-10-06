using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrenciesPSMgr : MonoBehaviour
{
    private ParticleSystem m_ParticleSystem;

    private void Awake()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }

    public void BurstDo_F(Vector3 pos)
    {
        transform.position = pos;
        m_ParticleSystem.Play();
    }
}
