using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class JetsDatabase : DatabaseBase
{
    #region Variables
    public Dictionary<int, JetData> m_JetDatas;
    #endregion

    public override IEnumerator LoadDatabase_F()
    {
        if(m_JetDatas == null || m_JetDatas.Count == 0)
        {
            //Debug.Log("<color=red><b>-----PLAYER LOADING BEGINS-----</b></color>");
            m_JetDatas = new Dictionary<int, JetData>();

            yield return Addressables.LoadAssetsAsync<JetData>("JetData", x =>
            {
                m_JetDatas.Add(x.ID, x);
            });

            m_IsLoaded = true;
        }
    }

    public GameObject JetInstantiate_F(int id, Vector3 pos = default)
    {
        if (!m_JetDatas.ContainsKey(id)) throw new System.Exception($"Couldnt Instantiate Jet cuz JetData with id {id} not found");

        GameObject r = Instantiate(m_JetDatas[id].JetPawnPrefab);
        r.transform.name = $"Jet {id}";
        r.transform.position = default;
        return r;
    }

    public GameObject JetCurInstantiate_F()
    {
        return JetInstantiate_F(GlobalDatabaseInitializer.s_Instance.globalData.JetCur);
    }
}
