using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ColorMgr : MonoBehaviour
{
    #region Variables
    //[Range(0f, 1f)]
    //[SerializeField] private float m_Hue0;
    //public float Hue0
    //{
    //    get => m_Hue0;
    //    set
    //    {
    //        m_Hue0 = value;
    //        Shader.SetGlobalFloat("_Hue0", value);
    //        Shader.SetGlobalColor("_PrimaryColor", Color.HSVToRGB(value, 1.0f, 1.0f));
    //    }
    //}
    private Color[] m_PrimaryColors;
    #endregion

    private void Awake()
    {
        m_PrimaryColors = GetPrimaryColors_F();
    }

    static private Color[] GetPrimaryColors_F()
    {
        return new Color[]
        {
            Color.yellow,
            Color.cyan,
            Color.green,
            Color.red,
            new Color(1.0f, 0.41f, 0.7f) 
        };
    }

    public void SetRandomColor_F()
    {
        Shader.SetGlobalColor("_PrimaryColor", m_PrimaryColors[Random.Range(0, m_PrimaryColors.Length)]);
    }

#if UNITY_EDITOR
    [MenuItem("Custom/Set Random _PrimaryColor")]
    private static void SetRandomColorContextMenu_F()
    {
        Color[] colors = GetPrimaryColors_F();
        Shader.SetGlobalColor("_PrimaryColor", colors[Random.Range(0, colors.Length)]);
    }

//    private void OnValidate()
//    {
//        Shader.SetGlobalFloat("_Hue0", m_Hue0);
//        Shader.SetGlobalColor("_PrimaryColor", Color.HSVToRGB(m_Hue0, 1.0f, 1.0f));
//    }
#endif
}
