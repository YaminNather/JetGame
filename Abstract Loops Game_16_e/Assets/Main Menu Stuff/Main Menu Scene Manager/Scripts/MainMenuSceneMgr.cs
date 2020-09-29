using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneMgr : MonoBehaviour
{
    #region Variables
    private MainMenuSceneReferences mmsr;
    private GlobalDatabaseInitializer gdi;    
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

        switch(gdi.globalData.MenuToOpen)
        {
            case Menus_EN.MainMenu:
                mmsr.mainMenuMgr.Open_F();
                break;

            case Menus_EN.ScoreBoard:
                mmsr.scoreBoardMgr.gameObject.SetActive(true);
                mmsr.scoreBoardMgr.Refresh_F();
                break;
        }
        gdi.globalData.MenuToOpen = Menus_EN.MainMenu;

        mmsr.jetDisplayMgr.Init_F();
    }    
}
