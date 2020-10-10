using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuJetMgr : MonoBehaviour
{
    #region Variables
    private JetMovementComponent m_jmc;

    private Dictionary<int, GameObject> m_JetDatasAndGObjs;
    public Dictionary<int, GameObject> JetDatasAndGObjs { get => m_JetDatasAndGObjs; }
    private int m_JetCurID;
    public int JetCurID { get => m_JetCurID; }
    #endregion

    private void Awake()
    {
        m_jmc = GetComponent<JetMovementComponent>();
        m_JetCurID = -1;
    }

    private void Start()
    {        
        StartCoroutine(RandomJetSway_IEF());
    }

    public void JetsInstantiate_F()
    {
        m_JetDatasAndGObjs = new Dictionary<int, GameObject>();
        JetsDatabase jdb = GlobalDatabaseInitializer.INSTANCE.m_JetsDatabase;

        foreach(KeyValuePair<int, JetData> kvp in jdb.m_JetDatas)
        {
            GameObject gObj = jdb.JetOnlyMeshInstantiate_F(kvp.Key);
            m_JetDatasAndGObjs.Add(kvp.Key, gObj);

            if (kvp.Key == GlobalDatabaseInitializer.INSTANCE.m_GlobalData.JetCur)
            {
                JetCurSet_F(kvp.Key);
            }
            else
                gObj.SetActive(false);            
        }
    }

    public void JetCurSet_F(int id)
    {
        if (m_JetCurID != -1)
        {
            m_JetDatasAndGObjs[m_JetCurID].SetActive(false);
            m_JetDatasAndGObjs[m_JetCurID].transform.parent = null;
        }

        m_JetDatasAndGObjs[id].SetActive(true);
        m_JetDatasAndGObjs[id].transform.parent = transform;
        m_JetDatasAndGObjs[id].transform.localPosition = Vector3.zero;
        m_JetCurID = id;
    }

    private void Update()
    {
        m_jmc.InputVectorAdd_F(new Vector3(0f, 0f, 1f));
    }

    private IEnumerator RandomJetSway_IEF()
    {        
        while(true)
        {
            float time = Random.Range(0f, 0.5f);            
            Vector3 input = new Vector3(Random.Range(-1, 2), Random.Range(-1, 2), 0f);
            //Debug.Log($"<color=yellow>Time = {time}, dir = {input}</color>");
            while (time > 0f)
            {
                m_jmc.InputVectorAdd_F(input);
                time -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
