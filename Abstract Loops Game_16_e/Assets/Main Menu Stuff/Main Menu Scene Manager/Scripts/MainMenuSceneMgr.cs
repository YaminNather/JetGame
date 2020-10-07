using DG.Tweening;
using Ludiq.PeekCore;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEngine;

public class MainMenuSceneMgr : MonoBehaviour
{
    #region Variables
    private MainMenuSceneReferences mmsr;
    private GlobalDatabaseInitializer gdi;    

    private MainMenuPages_EN m_PageCur;
    public MainMenuPages_EN PageCur { get => m_PageCur; }
    private Page PageCurFromEnum { get => PageFromEnum_F(m_PageCur); }
    #endregion

    private void Awake()
    {
        gdi = GlobalDatabaseInitializer.s_Instance;
        mmsr = MainMenuSceneReferences.s_Instance;
    }

    private void Start()
    {
        DOTween.To(() => 0f, val => mmsr.colorMgr.Hue0 = val, 1f, 40f).SetLoops(-1).SetEase(Ease.Linear);
        StartCoroutine(Start_IEF());
    }

    private IEnumerator Start_IEF()
    {
        if (gdi.AllLoaded == false) yield return null;        
        
        PageOpen_F(gdi.globalData.m_MenuToOpen);
        gdi.globalData.m_MenuToOpen = MainMenuPages_EN.Main;
        
        mmsr.jetDisplayMgr.Init_F();        
    }

    public void PageOpen_F(MainMenuPages_EN pageEnum)
    {
        Page pageToOpen = PageFromEnum_F(pageEnum);
        if (pageToOpen == null) throw new System.Exception("Cannot open Null Page!!");

        if (m_PageCur == MainMenuPages_EN.None) pageToOpen.Open_F();
        else PageCurFromEnum.Close_F(pageToOpen);
        
        m_PageCur = pageEnum;
    }

    private Page PageFromEnum_F(MainMenuPages_EN pageEnum)
    {
        return pageEnum switch
        {
            MainMenuPages_EN.Main => mmsr.mainMenuMgr,
            MainMenuPages_EN.JetStore => mmsr.jetStoreMgr,
            MainMenuPages_EN.Settings => throw new System.Exception("Settings page not implemented yet"),
            MainMenuPages_EN.ScoreBoard => mmsr.scoreBoardMgr,            
            _ => null,
        };
    }
}

public enum MainMenuPages_EN { None, Main, JetStore, ScoreBoard, Settings }
