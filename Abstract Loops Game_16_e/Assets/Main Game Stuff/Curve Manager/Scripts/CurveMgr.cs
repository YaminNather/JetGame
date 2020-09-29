using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveMgr : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float m_CurveStrength = 10;

    private void OnValidate()
    {
        Shader.SetGlobalFloat("_CurveStrength", m_CurveStrength);
    }
}
