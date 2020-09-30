using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosionPSMgr : MonoBehaviour
{
    private ParticleSystem[] m_ParticleSystems;

    private void Awake()
    {
        m_ParticleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    public void DoBurst_F(Vector3 pos)
    {
        transform.position = pos;
        foreach(ParticleSystem ps in m_ParticleSystems)
        {
            ps.Play();
        }
    }
}
