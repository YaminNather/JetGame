using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSceneLoadedCaller : MonoBehaviour
{
    private void Awake()
    {
        OnSceneLoadedMonoBehaviour[] oslmbs = FindObjectsOfType<OnSceneLoadedMonoBehaviour>(true);
        foreach(OnSceneLoadedMonoBehaviour oslmb in oslmbs)
        {
            oslmb.OnSceneLoaded_F();
        }
        Destroy(gameObject);
    }
}

public abstract class OnSceneLoadedMonoBehaviour : MonoBehaviour
{
    public abstract void OnSceneLoaded_F();
}
