using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneMgr : MonoBehaviour
{
    #region Variables
    private MainMenuSceneReferences mmsr;
    private GlobalDatabaseInitializer gdi;

    private Pages_EN m_PageCur;
    public Pages_EN PageCur { get => m_PageCur; }    
    #endregion

    private void Awake()
    {
        gdi = GlobalDatabaseInitializer.INSTANCE;
        mmsr = MainMenuSceneReferences.INSTANCE;
    }

    private void Start()
    {
        DOTween.To(() => 0f, val => mmsr.colorMgr.Hue0 = val, 1f, 40f).SetLoops(-1).SetEase(Ease.Linear);
        StartCoroutine(Start_IEF());
    }

    private IEnumerator Start_IEF()
    {
        while (gdi.AllLoaded == false) yield return null;

        Debug.Log("FINISHED LOADING");
        MainMenuSceneReferences.INSTANCE.mainMenuJetMgr.JetsInstantiate_F();
        PageOpen_F(Pages_EN.Main);        

        //mmsr.jetDisplayMgr.Init_F();
    }

    public void PageOpen_F(Pages_EN sceneEnum)
    {
        Page pageToOpen = PagesENToPage_F(sceneEnum);
        if (pageToOpen == null) throw new Exception("Cannot open Page Page is null!!!");

        if (m_PageCur == Pages_EN.None) pageToOpen.Open_F();
        else PagesENToPage_F(m_PageCur).Close_F(pageToOpen);

        m_PageCur = sceneEnum;
    }

    public Page PagesENToPage_F(Pages_EN sceneEnum)
    {
        return sceneEnum switch
        {
            Pages_EN.Main => mmsr.mainMenuMgr,
            Pages_EN.JetStore => mmsr.jetStoreMgr,
            Pages_EN.Settings => throw new System.Exception("Cannot convert to Enum to Settings Page"),
            _ => null
        };
    }

    public enum Pages_EN { None, Main, Settings, JetStore } 
}
