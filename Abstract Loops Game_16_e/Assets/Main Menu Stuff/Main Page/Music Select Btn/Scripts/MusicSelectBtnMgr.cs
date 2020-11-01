using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelectBtnMgr : Button
{
    #region Variables
    private Text m_BackgroundMusicCurLbl;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        m_BackgroundMusicCurLbl = GetComponentInChildren<Text>();
        onClick.AddListener(OnPressed_F);

        Refresh_F();
    }

    private void OnPressed_F()
    {
        GlobalData globalData = GlobalMgr.s_Instance.m_GlobalData;
        globalData.BackgroundMusicCur++;
        globalData.Save_F();

        Refresh_F();
    }

    private void Refresh_F()
    {
        m_BackgroundMusicCurLbl.text = "" + GlobalMgr.s_Instance.m_GlobalData.BackgroundMusicCur;
    }
}
