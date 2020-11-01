using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsPageMgr : Page
{
    #region Variables
    [Header("UI References")]
    [SerializeField] private Slider m_MusicSlider;
    [SerializeField] private Slider m_SFXSlider;
    [SerializeField] private Button m_QualityLevelBtn;

    [Header("AudioMixer")]
    [SerializeField] private AudioMixer m_0AudioMixer;
    #endregion

    private void OnEnable()
    {
        float volumeFromMixer = 0.0f;
        m_0AudioMixer.GetFloat("MusicVolume", out volumeFromMixer);
        m_MusicSlider.SetValueWithoutNotify(volumeFromMixer);

        m_0AudioMixer.GetFloat("SFXVolume", out volumeFromMixer);
        m_SFXSlider.SetValueWithoutNotify(volumeFromMixer);

        QualityBarRefresh_F();
    }

    private void Start()
    {
        m_MusicSlider.onValueChanged.AddListener(Music_SEF);
        m_SFXSlider.onValueChanged.AddListener(SFX_SEF);
    }

    private void QualityBarRefresh_F()
    {
        int qualityLevel = GlobalMgr.s_Instance.m_GlobalData.QualityLevel;

        int aLength = GlobalMgr.s_Instance.m_GlobalData.QUALITYLEVELSCOUNT;
        for (int i = 0; i < aLength; i++)
            m_QualityLevelBtn.transform.GetChild(i).gameObject.SetActive(i <= qualityLevel);
    }
    
    #region UI Event Functions
    public void Music_SEF(float value) => m_0AudioMixer.SetFloat("MusicVolume", value);

    public void SFX_SEF(float value) => m_0AudioMixer.SetFloat("SFXVolume", value);

    public void PageIcon_BEF()
    {
        MainMenuSceneReferences.INSTANCE.mainMenuSceneMgr.PageOpen_F(MainMenuSceneMgr.PagesEN.Main);
    }

    public void QualityBar_BEF()
    {
        GlobalData globalData = GlobalMgr.s_Instance.m_GlobalData;
        globalData.SetQualityLevel_F(globalData.QualityLevel + 1);
        QualityBarRefresh_F();
    }
    #endregion
}
