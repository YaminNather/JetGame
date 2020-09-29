using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GlobalData : MonoBehaviour
{
    #region Variables
    private string m_SaveDir;
    private string m_SaveFileName;
    private string SavePath { get => m_SaveDir + "/" + m_SaveFileName + ".txt"; }

    public Menus_EN MenuToOpen;
    
    private SaveInfo m_SaveInfo;

    public int Currency { get => m_SaveInfo.Currency; private set => m_SaveInfo.Currency = value; }
    private int m_CurrencyLastGame;
    public int CurrencyLastGame { get => m_CurrencyLastGame; set => m_CurrencyLastGame = value; }
    public int ScoreBest { get => m_SaveInfo.ScoreBest; set => m_SaveInfo.ScoreBest = value; }
    private int m_ScoreLastGame;
    public int ScoreLastGame 
    {
        get => m_ScoreLastGame;
        set
        {
            m_ScoreLastGame = value;
            if (value > ScoreBest) ScoreBest = value;
        }
    }

    public List<string> JetsOwned { get => m_SaveInfo.JetsOwned; set => m_SaveInfo.JetsOwned = value; }
    public string JetCur { get => m_SaveInfo.JetCur; set => m_SaveInfo.JetCur = value; }
    #endregion

    private void Awake()
    {        
        //Storing a save path.        
        m_SaveDir = Application.persistentDataPath + "/Saves";
        m_SaveFileName = "TestSaveFile_0";


        //Initializing some data members.
        m_ScoreLastGame = -1;
        
        //Loading Old Data.
        Load_F();
    }

    public void SaveNew_F()
    {
        Debug.Log("<color=cyan>Entered SaveNew_F()</color>");

        //Setting New Save Data
        m_SaveInfo = new SaveInfo();
        Currency = 0;
        ScoreBest = 0;
        JetsOwned.Clear();
        JetsOwned.Add("Test Player 0");
        JetsOwned.Add("Horizon Ripoff");
        JetCur = "Horizon Ripoff";
        
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

    public bool JetCheckIfOwned_F(string jetName) => JetsOwned.Contains(jetName);

    public void JetsOwnedAddTo_F(string jetName)
    {
        JetsOwned.Add(jetName);
    }

    public void CurrencyChange_F(int amount)
    {
        Currency += amount;
        if (Currency < 0) Currency = 0;
    }
}

[Serializable]
public class SaveInfo
{
    #region Variables
    public int Currency;
    public int ScoreBest;

    public List<string> JetsOwned;
    public string JetCur;
    #endregion

    public SaveInfo()
    {
        Currency = 0;
        ScoreBest = 0;
        JetsOwned = new List<string>();
        JetCur = "";        
    }
}

public enum Menus_EN { MainMenu, ScoreBoard}
