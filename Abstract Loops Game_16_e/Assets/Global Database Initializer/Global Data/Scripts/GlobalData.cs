using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
using UnityEditor;
using System.ComponentModel;
using System.Diagnostics;
#endif

[Serializable]
public partial class GlobalData : MonoBehaviour
{
    #region Variables
    private string m_SaveDir;
    private string m_SaveFileName;
    private string SavePath => m_SaveDir + "/" + m_SaveFileName + ".txt";

    private SaveInfo m_SaveInfo;

    public int Currency { get => m_SaveInfo.Currency; private set => m_SaveInfo.Currency = value; }
    private int m_CurrencyLastGame;
    public int CurrencyLastGame { get => m_CurrencyLastGame; set => m_CurrencyLastGame = value; }
    public int ScoreBest { get => m_SaveInfo.ScoreBest; set => m_SaveInfo.ScoreBest = value; }
    public int ScoreLastGame
    {
        get => m_SaveInfo.ScoreLastGame;
        set
        {
            m_SaveInfo.ScoreLastGame = value;
            if (value > ScoreBest)
            {
                ScoreBest = value;
                m_ScoreLastGameIsBest = true;
            }
        }
    }
    public bool m_ScoreLastGameIsBest;
    public bool ScoreLastGameIsBest { get => m_ScoreLastGameIsBest; set => m_ScoreLastGameIsBest = value; }   

    public List<int> JetsOwned { get => m_SaveInfo.JetsOwned; set => m_SaveInfo.JetsOwned = value; }
    public int JetCur { get => m_SaveInfo.JetCur; set => m_SaveInfo.JetCur = value; }

    public int LoopCur { get => m_SaveInfo.LoopCur; set => m_SaveInfo.LoopCur = value; }

    public readonly int BACKGROUNDMUSICCOUNT = 11;
    public int BackgroundMusicCur 
    {
        get => m_SaveInfo.BackgroundMusicCur; 
        set => m_SaveInfo.BackgroundMusicCur = (value < BACKGROUNDMUSICCOUNT) ? value : 0;
    }

    public readonly int QUALITYLEVELSCOUNT = 3;
    public int QualityLevel { get => m_SaveInfo.QualityLevel; private set => m_SaveInfo.QualityLevel = value; }
    private Vector2 m_ActualScreenResolution;
    public Vector2 ActualScreenResolution => m_ActualScreenResolution;

    private int m_GamesPlayedSinceLastInterstitialAd;
    public int GamesPlayedSinceLastInterstitialAd { get => m_GamesPlayedSinceLastInterstitialAd; set => m_GamesPlayedSinceLastInterstitialAd = value; }

    public bool HomePageTutorialDisplayed { get => m_SaveInfo.HomePageTutorialDisplayed; set => m_SaveInfo.HomePageTutorialDisplayed = value; }
    public bool StoreTutorialDisplayed { get => m_SaveInfo.StoreTutorialDisplayed; set => m_SaveInfo.StoreTutorialDisplayed = value; }

    public bool MainGameTutorialDisplayed { get => m_SaveInfo.MainGameTutorialDisplayed; set => m_SaveInfo.MainGameTutorialDisplayed = value; }

    #endregion

    private void Awake()
    {
        m_ActualScreenResolution = new Vector2(Screen.width, Screen.height);

        //Storing a save path.        
        m_SaveDir = Application.persistentDataPath + "/Saves";
        m_SaveFileName = "TestSaveFile_0";
        
        //Loading Old Data.
        Load_F();


        Currency = 90000;        
        Save_F();
    }

    public void SaveNew_F()
    {
        Debug.Log("<color=cyan>Entered SaveNew_F()</color>");

        //Setting New Save Data
        m_SaveInfo = new SaveInfo();
        Currency = 0;
        ScoreBest = 0;
        ScoreLastGame = 0; 
        JetsOwned.Clear();
        JetsOwned.Add(0);
        JetsOwned.Add(3);
        JetCur = 3;
        LoopCur = -1;
        BackgroundMusicCur = 0;
        QualityLevel = 2;
        HomePageTutorialDisplayed = false;
        StoreTutorialDisplayed = false;
        MainGameTutorialDisplayed = false;

        if (!Directory.Exists(m_SaveDir)) Directory.CreateDirectory(m_SaveDir);
        if (!File.Exists(SavePath))
        {
            using (File.Create(SavePath)) { }
        }        
    }

    public void Save_F()
    {        
        try 
        {
            if (!Directory.Exists(m_SaveDir) || !File.Exists(SavePath) || File.ReadAllText(SavePath) == "")
            {
                SaveNew_F();
            }

            string toWrite = JsonUtility.ToJson(m_SaveInfo);            
            Debug.Log($"<color=cyan>toWrite:\n{toWrite}</color>");
            File.WriteAllText(SavePath, toWrite);
            Debug.Log("<color=cyan>Wrote Successfully to File.</color>");
        }
        catch(Exception e)
        {
            Debug.Log("<color=red>Exception happened in Save_F():\n" + e.Message + "</color>");
        }
    }

    public void Load_F()
    {
        if (!Directory.Exists(m_SaveDir) || !File.Exists(SavePath))
        {
            Save_F();
            return;
        }

        try
        {
            m_SaveInfo = JsonUtility.FromJson<SaveInfo>(File.ReadAllText(SavePath));
            Debug.Log("<color=cyan>Loaded successfully from file.</color>");            
        }
        catch (Exception e)
        {
            Debug.Log("<color=red>Exception happened in Load_F():\n" + e.Message + "</color>");
        }
    }

    public bool JetCheckIfOwned_F(int id) => JetsOwned.Contains(id);

    public void JetsOwnedAddTo_F(int id)
    {
        JetsOwned.Add(id);
    }

    public void CurrencyChange_F(int amount)
    {
        Currency += amount;
        if (Currency < 0) Currency = 0;
    }

    public void SetQualityLevel_F(int qualityLevel)
    {
        Debug.Log($"Setting Quality Level to {qualityLevel}");
        QualityLevel = qualityLevel;
        if (QualityLevel >= QUALITYLEVELSCOUNT) QualityLevel = 0;
        Save_F();

        CurrentQualityLevelApply_F();
    }

    public void CurrentQualityLevelApply_F()
    {
        switch (QualityLevel)
        {
            case 0:
                FindObjectOfType<Volume>(true).gameObject.SetActive(false);
                Screen.SetResolution((int) (m_ActualScreenResolution.x / 2.0f), (int) (m_ActualScreenResolution.y / 2.0f),
                    true);
                break;

            case 1:
                FindObjectOfType<Volume>(true).gameObject.SetActive(true);
                Screen.SetResolution((int) (m_ActualScreenResolution.x / 2.0), (int) (m_ActualScreenResolution.y / 2.0f),
                    true);
                break;

            case 2:
                FindObjectOfType<Volume>(true).gameObject.SetActive(true);
                Screen.SetResolution((int) (m_ActualScreenResolution.x / 1.5f), (int) (m_ActualScreenResolution.y / 1.5f),
                    true);
                break;
        }
    }
}

#if UNITY_EDITOR
public partial class GlobalData
{
    [MenuItem("Custom/Open Save Folder")]
    private static void OpenSaveDataFolder_F()
    {
        try
        {
            Process.Start(@"C:\Users\2001s\AppData\LocalLow\DefaultCompany\Abstract Loops Game0\Saves\");
        }
        catch (Win32Exception win32Exception)
        {
            //The system cannot find the file specified...
            Console.WriteLine(win32Exception.Message);
        }
    }
}
#endif

[Serializable]
public class SaveInfo
{
    #region Variables
    public int Currency;
    public int ScoreBest;
    public int ScoreLastGame;

    public List<int> JetsOwned;
    public int JetCur;
    public int LoopCur;
    public int BackgroundMusicCur;
    public int QualityLevel;

    public bool StoreTutorialDisplayed;
    public bool HomePageTutorialDisplayed;

    public bool MainGameTutorialDisplayed;
    #endregion

    public SaveInfo()
    {
        Currency = 0;
        ScoreBest = 0;
        ScoreLastGame = 0;
        JetsOwned = new List<int>();
        JetCur = 0;
        LoopCur = -1;
        BackgroundMusicCur = 0;
        QualityLevel = 2;
        StoreTutorialDisplayed = false;
        HomePageTutorialDisplayed = false;
        MainGameTutorialDisplayed = false;
    }
}