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
            if (value > ScoreBest) ScoreBest = value;
        }
    }

    public List<int> JetsOwned { get => m_SaveInfo.JetsOwned; set => m_SaveInfo.JetsOwned = value; }
    public int JetCur { get => m_SaveInfo.JetCur; set => m_SaveInfo.JetCur = value; }
    #endregion

    private void Awake()
    {
        //Storing a save path.        
        m_SaveDir = Application.persistentDataPath + "/Saves";
        m_SaveFileName = "TestSaveFile_0";
        
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
        ScoreLastGame = 0; 
        JetsOwned.Clear();
        JetsOwned.Add(0);
        JetsOwned.Add(3);
        JetCur = 3;
        
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
}

[Serializable]
public class SaveInfo
{
    #region Variables
    public int Currency;
    public int ScoreBest;
    public int ScoreLastGame;

    public List<int> JetsOwned;
    public int JetCur;
    #endregion

    public SaveInfo()
    {
        Currency = 0;
        ScoreBest = 0;
        ScoreLastGame = 0;
        JetsOwned = new List<int>();
        JetCur = 0;        
    }
}