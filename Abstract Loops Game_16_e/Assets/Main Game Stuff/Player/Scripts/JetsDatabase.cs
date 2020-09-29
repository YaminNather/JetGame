using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class JetsDatabase : DatabaseBase
{
    public Dictionary<string, GameObject> Jets;
    public override IEnumerator LoadDatabase_F()
    {
        if(Jets == null || Jets.Count == 0)
        {
            //Debug.Log("<color=red><b>-----PLAYER LOADING BEGINS-----</b></color>");
            Jets = new Dictionary<string, GameObject>();

            yield return Addressables.LoadAssetsAsync<GameObject>("Player", x =>
            {
                Jets.Add(x.GetComponent<JetData>().PlayerName, x);
            });

            m_IsLoaded = true;
        }
    }
}
