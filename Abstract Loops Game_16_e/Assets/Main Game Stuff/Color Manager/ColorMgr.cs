using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorMgr : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float m_Hue0;
    public float Hue0
    {
        get => m_Hue0;
        set
        {
            m_Hue0 = value;
            Shader.SetGlobalFloat("_Hue0", value);
        }
    }

    private void Start()
    {
        Hue0 = Random.Range(0f, 1f);
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        Shader.SetGlobalFloat("_Hue0", m_Hue0);        
    }
#endif
}
