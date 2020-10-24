using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPageMgr : MonoBehaviour
{
    #region Variables
    [Header("UI References")]
    [SerializeField] private Slider m_MusicSlider;
    [SerializeField] private Slider m_SFXSlider;
    #endregion

    private void Start()
    {
        m_MusicSlider.onValueChanged.AddListener(Music_SEF);
        m_SFXSlider.onValueChanged.AddListener(SFX_SEF);
    }

    #region UI Event Functions
    public void Music_SEF(float value) => Debug.Log($"Music Value = {value}");

    public void SFX_SEF(float value) => Debug.Log($"SFX Value = {value}");
    #endregion
}
