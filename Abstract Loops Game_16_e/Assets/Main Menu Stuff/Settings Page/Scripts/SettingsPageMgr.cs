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
    }

    private void Start()
    {
        m_MusicSlider.onValueChanged.AddListener(Music_SEF);
        m_SFXSlider.onValueChanged.AddListener(SFX_SEF);
    }

    #region UI Event Functions
    public void Music_SEF(float value) => m_0AudioMixer.SetFloat("MusicVolume", value);

    public void SFX_SEF(float value) => m_0AudioMixer.SetFloat("SFXVolume", value);

    public void PageIcon_BEF()
    {
        MainMenuSceneReferences.INSTANCE.mainMenuSceneMgr.PageOpen_F(MainMenuSceneMgr.PagesEN.Main);
    }
    #endregion
}
